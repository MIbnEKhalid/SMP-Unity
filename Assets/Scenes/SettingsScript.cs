using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


public class SettingsScript : MonoBehaviour
{
    public TMP_InputField ck_IP;
    public TMP_InputField Rk_IP;

    void Start()
    {
        int rkey = PlayerPrefs.GetInt("RsaKeyBitRate");
        int ckey = PlayerPrefs.GetInt("CustomKeyBitRate");

        ck_IP.text = ckey.ToString();
        Rk_IP.text = rkey.ToString();
        string pubickey = "<RSAKeyValue><Modulus>0QfChGRq16w4UDFe0V2pjsu7BCoDifQ+q4iR4DBO1lafxYLWVbyYWQLCdKtpDXHgAX0/XICFxUyLwUqVRgqVc5FxjbThgamQqBqcXUzpFCSdoqLwqINu+krgRp3BqxiZAVimqJkA4x6J+NY4BYCbsJDA+BXgrDqA7wkeNyGHzTE=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        Debug.Log("RSA Enc:  " + EncryptRSA(GenerateNewCustomKeyNow(703), pubickey));
    }

    private bool isFullscreen = true;

    public void ToggleFullscreen()
    {
        isFullscreen = !isFullscreen;
        Screen.fullScreen = isFullscreen;
        Debug.Log(isFullscreen);
    }
    bool IsValidRSAKeyLength(int bitLength)
    {
        // Check if the bit length is a power of two and greater than or equal to 1024
        bool isPowerOfTwo = (bitLength >= 1024) && (bitLength & (bitLength - 1)) == 0;

        // Check if the bit length is a multiple of 1024 and greater than or equal to 1024
        bool isMultipleOf1024 = (bitLength >= 1024) && (bitLength % 1024 == 0);

        // Check for specific valid lengths like 3072
        bool isSpecialCase = (bitLength == 3072);

        bool twofactor = (bitLength > 0) && ((bitLength & (bitLength - 1)) == 0);

        // Return true if any condition is met
        return isPowerOfTwo || isSpecialCase || isMultipleOf1024 || twofactor;
    }
    public async void UpdateCustomkeyBit()
    {
        string publicKey = await GenerateKeyPairAsync();

        int.TryParse(ck_IP.text, out int result);
        if (IsValidRSAKeyLength(result))
        {
            bool works = true;
            string ckey = GenerateNewCustomKeyNow(result);
            string output = EncryptRSA(ckey, publicKey);
            if (output.StartsWith("RSA Encryption failed: The data to be encrypted exceeds the maximum for this modulus of 117 bytes."))
            {
                works = false;

            }
            if (works)
            {
                Debug.Log("Custom key Bit Rate Value Updated!");
                PlayerPrefs.SetInt("CustomKeyBitRate", result);
            }
            else
            {
                Debug.Log("You either need to decrease custom key or increase RSA key size");
            }
        }
        else
        {
            Debug.Log("Please Use Correct Bit Rate (any number that is power of 2)");

        }
    }
    public void UpdateRSAkeyBit()
    {
        int.TryParse(Rk_IP.text, out int result);
        if (IsValidRSAKeyLength(result))
        {
            Debug.Log("RSA key Bit Rate Value Updated!");
            PlayerPrefs.SetInt("RsaKeyBitRate", result);

        }
        else
        {
            Debug.Log("Please Use Correct Bit Rate (any number that is power of 2)");

        }
    }




    private RSACryptoServiceProvider rsa;

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
            // int ckey = PlayerPrefs.GetInt("CustomKeyBitRate");
            int ckeyInBytes = txtsize / 8;
            Debug.Log(ckeyInBytes.ToString());
            byte[] key = new byte[ckeyInBytes]; // 256 bits = 32 bytes
            rng.GetBytes(key);
            // Convert the key to a base64 string for easy storage or display
            Debug.Log("wtf" + Convert.ToBase64String(key));
            return Convert.ToBase64String(key);
        }
    }

    private async Task<string> GenerateKeyPairAsync()
    {
        return await Task.Run(() =>
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024))
            {
                try
                {
                    // Export the public key as an XML string
                    string publicKey = rsa.ToXmlString(false);

                    return publicKey;
                }
                finally
                {
                    // Clear the RSA key to avoid any potential memory leaks
                    rsa.PersistKeyInCsp = false;
                }
            }
        });
    }

}
