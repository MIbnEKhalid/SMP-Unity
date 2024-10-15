#region Libaries
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO.Compression;
using System.Globalization;
using System.Collections;
using UnityEngine.Video;
using System.Numerics;
using UnityEngine.UI;
using System.Text;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using TMPro;
using static UnityEngine.UI.Image;
using static System.Net.Mime.MediaTypeNames;
#endregion

public class EncryptScript : MonoBehaviour
{

    public TMP_InputField InputPublicKey;
    public TMP_InputField InputF;
    public TMP_InputField OutputF;

    public smp SMP;
    public main mainScript;

    public GameObject EncryptPage;
    public GameObject DecryptPage;

    public bool EnableDebugLogs = true;

    void Start()
    {
        string publicKey = PlayerPrefs.GetString("LocalRSAPublicKey", "");

        if (EncryptPage != null && EncryptPage.activeSelf)
        {
            OutputF.readOnly = true;
            InputPublicKey.text = publicKey;
            DebugLog("right");
        }

        if (DecryptPage != null && DecryptPage.activeSelf)
        {
            OutputF.readOnly = true;
            InputPublicKey.text = publicKey;
            DebugLog("left");
        }


    }

    void DebugLog(string log)
    {
        if (EnableDebugLogs)
        {
            UnityEngine.Debug.Log(log);
        }
    }
 
    public void EncryptTheText()
    {
        bool test1 = false;
        string newGeneratedCustomKey = SMP.GenerateNewCustomKeyNow();
        DebugLog("newGeneratedCustomKey: " + newGeneratedCustomKey);

        string customKeyRSA = SMP.EncryptRSA(newGeneratedCustomKey, InputPublicKey.text);
        if (customKeyRSA.StartsWith("RSA Encryption failed: "))
        {
            mainScript.ShowCopyMessage("RSA Key Encryption failed");
            OutputF.text = "RSA Key Encryption failed";
            if (customKeyRSA.StartsWith("RSA Encryption failed: The data to be encrypted exceeds the maximum for this modulus of 117 bytes."))
            {
                mainScript.ShowCopyMessage("RSA Key Encryption failed");
            }
            mainScript.ShowCopyMessage("RSA Key Encryption failed Due to lower bit RSA Key");
            OutputF.text = "RSA Key Encryption failed Due to lower bit RSA Key";
        }
        else
        {
            test1 = true;
        }
        DebugLog("customKeyRSA:" + customKeyRSA);

        //string ASV = SMP.AssignHexValues(InputF.text);
        //Debug.Log("Old ASV: " + ASV); 


        DebugLog("Message Input: " + InputF.text);

        //This string help to deal with newlinws in input, new line is replace by tag which then replace by new line in decryption.
        //This is neccessary to add when using function ConvertStringToCustomHex.
        string noNewLines = InputF.text.Replace(Environment.NewLine, " <tag>newline1</tag> ").Replace("\n", " <tag>newline2</tag> ");
 
        string ASV = SMP.ConvertStringToCustomHex(noNewLines);
        DebugLog("New ASV: " + ASV);
        Debug.Log("New ASV: " + ASV);

        string CV = SMP.CompressString(ASV);
        DebugLog("CV: " + CV);

        string EV = SMP.encrypt(CV, newGeneratedCustomKey);
        DebugLog("EV: " + EV);

        string formatCustomKey = "<key>\n" + customKeyRSA + "\n</key>\n";
        string formatMessage = "<message>\n" + EV + "\n</message>";
        string message = "<data>\n" + formatCustomKey + formatMessage + "\n</data>";

        if (test1)
        {
            mainScript.fadeDuration = 0.5f;
            mainScript.fadeDelay = 0.5f;
            OutputF.text = message;
            DebugLog(message);
            mainScript.ShowCopyMessage("Message Encrypted Sucessfully!");
        }
        else
        {
            DebugLog("Failed");
        }
        newGeneratedCustomKey = null; customKeyRSA = null; ASV = null; CV = null; EV = null; formatMessage = null; formatCustomKey = null; message = null;
    }
}