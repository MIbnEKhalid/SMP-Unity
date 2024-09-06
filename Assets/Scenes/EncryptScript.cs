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
#endregion

public class EncryptScript : MonoBehaviour
{ 
     
    public TMP_InputField InputPublicKey;
    public TMP_InputField InputF;
    public TMP_InputField OutputF;
      
    public smp SMP;
    public main mainScript;
     

    void Start()
    {
        OutputF.readOnly = true;
        string publicKey = PlayerPrefs.GetString("LocalRSAPublicKey", "");
        InputPublicKey.text = publicKey;
    } 

    public void EncryptTheText()
    {
        string newGeneratedCustomKey = SMP.GenerateNewCustomKeyNow();
        Debug.Log("newGeneratedCustomKey: " + newGeneratedCustomKey);

        string customKeyRSA = SMP.EncryptRSA(newGeneratedCustomKey, InputPublicKey.text);
        Debug.Log("customKeyRSA:" + customKeyRSA);

        //string ASV = SMP.AssignHexValues(InputF.text);
        //Debug.Log("Old ASV: " + ASV);

        string ASV = SMP.ConvertStringToCustomHex(InputF.text);
        Debug.Log("New ASV: " + ASV);


        string CV = SMP.CompressString(ASV);
        Debug.Log("CV: " + CV);

        string EV = SMP.encrypt(CV, newGeneratedCustomKey);
        Debug.Log("EV: " + EV);

        string formatCustomKey = "<key>\n" + customKeyRSA + "\n</key>\n";
        string formatMessage = "<message>\n" + EV + "\n</message>";
        string message = "<data>\n" + formatCustomKey + formatMessage + "\n</data>";

        OutputF.text = message;
        Debug.Log(message);
        mainScript.ShowCopyMessage("Message Encrypted Sucessfully!");
        newGeneratedCustomKey = null; customKeyRSA = null; ASV = null; CV = null; EV = null; formatMessage = null; formatCustomKey = null; message = null;
    } 
}
