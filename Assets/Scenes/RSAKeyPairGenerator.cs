#region Libaries
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Threading.Tasks;
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

public class RSAKeyPairGenerator : MonoBehaviour
{
    public TMP_InputField publickey_IP;
    public TMP_InputField privatekey_IP;
    public GameObject messageBox;
    public TMP_Text Message;
    private float fadeDuration = 0.5f;  // Adjust duration for fade in/out
    private float fadeDelay = 0.5f;     // Delay between fades
    private CanvasGroup canvasGroup;  // Reference to the CanvasGroup

    private bool isGenerating = false; // Flag to track key generation status

    private void Start()
    {
        // Get or add a CanvasGroup component to the messageBox GameObject
        if (messageBox != null)
        {
            canvasGroup = messageBox.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = messageBox.AddComponent<CanvasGroup>();  // Add CanvasGroup if it doesn't exist
            }
            canvasGroup.alpha = 0f;  // Ensure the initial alpha is 0 (fully transparent)
        }
        else
        {
            Debug.LogError("MessageBox GameObject is not assigned!");
        }

        // Get the stored key values
        string publicKey = PlayerPrefs.GetString("LocalRSAPublicKey", "");
        string privateKey = PlayerPrefs.GetString("LocalRSAPrivateKey", "");

        // Log the values for debugging
        Debug.Log($"PublicKey: {publicKey}");
        Debug.Log($"PrivateKey: {privateKey}");

        // Check if either key is missing
        if (string.IsNullOrEmpty(publicKey) || string.IsNullOrEmpty(privateKey))
        {
            GeneratePair();
        }
        else
        {
            publickey_IP.text = publicKey;
            privatekey_IP.text = privateKey;
        }

    }
    public void Update()
    {
        // PlayerPrefs.SetString("LocalRSAPublicKey", null);
        // PlayerPrefs.SetString("LocalRSAPrivateKey", null);
    }
    public async void GeneratePair()
    {
        // Check if key generation is already in progress
        if (isGenerating)
        {
            Debug.Log("Key generation is already in progress.");
            return;
        }

        try
        {
            isGenerating = true; // Set the flag to indicate that key generation is in progress
            publickey_IP.text = "";
            privatekey_IP.text = "";

            Debug.Log("Starting RSA key generation...");

            // Generate RSA Key Pair Asynchronously
            (string publicKey, string privateKey) = await GenerateKeyPairAsync();

            // Display Keys
            Debug.Log("Public Key: " + publicKey);
            Debug.Log("Private Key: " + privateKey);

            PlayerPrefs.SetString("LocalRSAPublicKey", publicKey);
            PlayerPrefs.SetString("LocalRSAPrivateKey", privateKey);

            publickey_IP.text = publicKey;
            privatekey_IP.text = privateKey;
        }
        finally
        {
            isGenerating = false; // Reset the flag when key generation is complete
            ShowCopyMessage("New RSA Pair Key Generated Successfully");
        }
    }

    public void ShowCopyMessage(string message)
    {
        StartCoroutine(showMessage(message));
    }

    public void CopyTxt(TMP_InputField inputField)
    {
        TextEditor textEditor = new TextEditor();
        textEditor.text = inputField.text;
        textEditor.OnFocus();
        textEditor.SelectAll();
        textEditor.Copy();
    }

    private IEnumerator showMessage(string txt)
    {
        Message.text = txt;
        yield return StartCoroutine(FadeIn());
        yield return new WaitForSeconds(0.7f);
        yield return StartCoroutine(FadeOut());
    }

    private Task<(string, string)> GenerateKeyPairAsync()
    {
        return Task.Run(() =>
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
            {
                try
                {
                    // Export the public key as an XML string
                    string publicKey = rsa.ToXmlString(false);

                    // Export the private key as an XML string
                    string privateKey = rsa.ToXmlString(true);

                    return (publicKey, privateKey);
                }
                finally
                {
                    // Clear the RSA key to avoid any potential memory leaks
                    rsa.PersistKeyInCsp = false;
                }
            }
        });
    }

    public void StartFadeOut()
    {
        if (canvasGroup != null)
            StartCoroutine(FadeOut());
    }

    public void StartFadeIn()
    {
        if (canvasGroup != null)
            StartCoroutine(FadeIn());
    }

    private IEnumerator FadeOut()
    {
        float startAlpha = canvasGroup.alpha;  // Get the current alpha
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeDuration);  // Lerp to alpha = 0
            yield return null;
        }

        // Ensure the final alpha value is set to 0
        canvasGroup.alpha = 0f;
        yield return new WaitForSeconds(fadeDelay);  // Optional delay after fading out
    }

    private IEnumerator FadeIn()
    {
        float startAlpha = canvasGroup.alpha;  // Get the current alpha
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, elapsedTime / fadeDuration);  // Lerp to alpha = 1
            yield return null;
        }

        // Ensure the final alpha value is set to 1
        canvasGroup.alpha = 1f;
        yield return new WaitForSeconds(fadeDelay);  // Optional delay after fading in
    }
}
