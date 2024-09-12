using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


public class testing : MonoBehaviour
{
    private RSACryptoServiceProvider rsa;

    async void Start()
    {
        // string pubickey = "<RSAKeyValue><Modulus>kYbwMo7AjpApBeF04FGmalWBGdm36Mo3onvv7RdU3rfsWdatKZxlGXm/iN4lKJJjiaXAG4VJwazb0TQ+BzwUk1UWa5ZmEqs59aQdlGyyUNECxHBOstx1kkS69UXK62B20J4KlpMaklAV3E0YXGQh8qYNxGolz2mA/Yaub8bU9hU=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        // Debug.Log("RSA Enc:  " + EncryptRSA(GenerateNewCustomKeyNow(704), pubickey));
        // With RSACryptoServiceProvider(1024) i was able to encrypt maximum encrypt 703 bit customkey, i want to know for 704 bit what exact RSACryptoServiceProvider length will require 



    }


    public string EncryptRSA(string txt, string key)
    {
        try
        {
            // Initialize the RSA provider
            if (rsa == null)
            {
                rsa = new RSACryptoServiceProvider();
            }

            rsa.FromXmlString(key);
            byte[] dataBytes = Encoding.UTF8.GetBytes(txt);
            // Using PKCS#1 v1.5 padding
            byte[] encryptedBytes = rsa.Encrypt(dataBytes, RSAEncryptionPadding.Pkcs1);
            return Convert.ToBase64String(encryptedBytes);
        }
        catch (Exception ex)
        {
            Debug.LogError("RSA Encryption failed: " + ex.Message);
            return "RSA Encryption failed: " + ex.Message;
        }
    }
    bool isGenerating;
    public string GenerateNewCustomKeyNow(int txtsize)
    {
        using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
        {
            int ckeyInBytes = txtsize / 8;
            Debug.Log(ckeyInBytes.ToString());
            byte[] key = new byte[ckeyInBytes]; // 256 bits = 32 bytes
            rng.GetBytes(key);
            // Convert the key to a base64 string for easy storage or display
            Debug.Log("wtf" + Convert.ToBase64String(key));
            return Convert.ToBase64String(key);
        }
    }


    public async Task GeneratePair(int keySizeBits)
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

            //   Debug.Log($"Starting RSA key generation for key size {keySizeBits} bits...");

            try
            {
                // Generate RSA Key Pair Asynchronously
                (string publicKey, string privateKey) = await GenerateKeyPairAsync(keySizeBits);

                // Display Keys
                Debug.Log("Public Key: " + publicKey);
            }
            catch (CryptographicException ex)
            {
                // Handle specific CryptographicException for invalid key size
                // Debug.LogError($"Failed to generate RSA key pair for key size {keySizeBits} bits: {ex.Message}");
            }
        }
        finally
        {
            isGenerating = false; // Reset the flag when key generation is complete
        }
    }


    private Task<(string, string)> GenerateKeyPairAsync(int keySizeBits)
    {
        return Task.Run(() =>
        {
            try
            {
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(keySizeBits))
                {
                    // Export the public key as an XML string
                    string publicKey = rsa.ToXmlString(false);

                    // Export the private key as an XML string
                    string privateKey = rsa.ToXmlString(true);

                    return (publicKey, privateKey);
                }
            }
            catch (CryptographicException ex)
            {
                // Rethrow to be caught in the calling method
                throw new CryptographicException($"Key generation failed for key size {keySizeBits} bits.", ex);
            }
        });
    }

}
