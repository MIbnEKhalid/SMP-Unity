#region Libaries
using System.Security.Cryptography;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;
#endregion

public class landingscriptWebGL : MonoBehaviour
{
    private bool isGenerating = false; // Flag to track key generation status

    public CanvasGroup canvasGroup;  // Reference to the CanvasGroup
    private float fadeDuration = 0.5f;  // Adjust duration for fade in/out
    private float fadeDelay = 0.5f;
    public bool removeSaveKey;

    public TMP_Text text;
    private IEnumerator Start()
    {
        if (!PlayerPrefs.HasKey("CustomKeyBitRate"))
        {
            PlayerPrefs.SetInt("CustomKeyBitRate", 256);
        }

        // Check if "RsaKeyBitRate" key exists, if not, set default value
        if (!PlayerPrefs.HasKey("RsaKeyBitRate"))
        {
            PlayerPrefs.SetInt("RsaKeyBitRate", 512);
        }

        PlayerPrefs.SetInt("RsaKeyBitRate", 512);
        PlayerPrefs.SetInt("CustomKeyBitRate", 256);

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
            yield return StartCoroutine(FadeIn());
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void Update()
    {
        if (removeSaveKey)
        {
            PlayerPrefs.SetString("LocalRSAPublicKey", null);
            PlayerPrefs.SetString("LocalRSAPrivateKey", null);
            Debug.Log("Removed");
        }
    }

    public void GeneratePair()
    {
        // Check if key generation is already in progress
        if (isGenerating)
        {
            Debug.Log("Key generation is already in progress.");
            return;
        }

        try
        {
            isGenerating = true;

            Debug.Log("Starting RSA key generation...");
            text.text = "Generating RSA Key Pair...";
            // Generate RSA Key Pair Synchronously
            (string publicKey, string privateKey) = GenerateKeyPair();

            // Display Keys
            Debug.Log("Public Key: " + publicKey);
            Debug.Log("Private Key: " + privateKey);

            PlayerPrefs.SetString("LocalRSAPublicKey", publicKey);
            PlayerPrefs.SetString("LocalRSAPrivateKey", privateKey);
        }
        finally
        {
            StartFadeIn();
            SceneManager.LoadScene("MainMenu");
            isGenerating = false; // Reset the flag when key generation is complete
        }
    }

    private (string, string) GenerateKeyPair()
    {
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(512))
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