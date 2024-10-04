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
#if UNITY_EDITOR
using static UnityEditor.PlayerSettings;
#endif
using System.Text.RegularExpressions;
#endregion

public class DecryptScript : MonoBehaviour
{

    public TMP_InputField PrivateKey_IP;
    public TMP_InputField InputF;
    public TMP_InputField OutputF;
    public smp SMP;
    public main MainScript;


    void Start()
    {
        OutputF.readOnly = true;
        string privateKey = PlayerPrefs.GetString("LocalRSAPrivateKey", "");
        PrivateKey_IP.text = privateKey;
    }


    public void DecryptTheText()
    {
        string keyPattern = @"<key>\s*(.*?)\s*<\/key>";
        string messagePattern = @"<message>\s*(.*?)\s*<\/message>";

        string encryptedCustomKey = Regex.Match(InputF.text, keyPattern).Groups[1].Value;
        string encryptedMessage = Regex.Match(InputF.text, messagePattern).Groups[1].Value;

        //      Debug.Log("k: " + encryptedCustomKey);
        //      Debug.Log("m: " + encryptedMessage);

        Debug.Log("Decrypt Input: " + PrivateKey_IP.text);

        string customKey = SMP.DecryptRSA(encryptedCustomKey, PrivateKey_IP.text);
        Debug.Log("customKey: " + customKey);

        string DEV = SMP.Decrypt(encryptedMessage, customKey);
        Debug.Log("DEV: " + DEV);

        string DCV = SMP.DecompressString(DEV);
        Debug.Log("DCV: " + DCV);

        string RSV = SMP.ReconvertCustomHexToString(DCV);
        Debug.Log("New RSV: " + RSV);

        //  string RSV = SMP.ReconvertHexString(DCV);
        //  Debug.Log("Old RSV: " + RSV);
        string reconverted = RSV.Replace(" <tag>newline1</tag> ", Environment.NewLine).Replace(" <tag>newline2</tag> ", "\n");

        OutputF.text = reconverted;
        MainScript.ShowCopyMessage("Message Decrypted Sucessfully!");
        encryptedCustomKey = null; encryptedMessage = null; customKey = null; DEV = null; DCV = null; RSV = null;
    }

}
