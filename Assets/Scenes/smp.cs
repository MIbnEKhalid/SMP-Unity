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

public class smp : MonoBehaviour
{
    int SetBufferSize = 100 * 1024 * 1024;
    int customKeysize;

    private void Start()
    {
        customKeysize = PlayerPrefs.GetInt("CustomKeyBitRate");
        Main();
    }

    private RSACryptoServiceProvider rsa;

    #region Assign ReAssign Hexa Decimal

    private static readonly Dictionary<char, string> CharToHexMap = new Dictionary<char, string>
{
    { 'a', "j" }, { 'b', "b" }, { 'c', "a" }, { 'd', "g" }, { 'e', "h" },
    { 'f', "f" }, { 'g', "c" }, { 'h', "e" }, { 'i', "i" }, { 'j', "d" },
    { 'k', "k" }, { 'l', "l" }, { 'm', "m" }, { 'n', "n" }, { 'o', "o" },
    { 'p', "p" }, { 'q', "q" }, { 'r', "r" }, { 's', "s" }, { 't', "t" },
    { 'u', "u" }, { 'v', "v" }, { 'w', "w" }, { 'x', "x" }, { 'y', "y" },
    { 'z', "z" },

    { 'A', "C" }, { 'B', "J" }, { 'C', "H" }, { 'D', "G" }, { 'E', "K" },
    { 'F', "F" }, { 'G', "D" }, { 'H', "A" }, { 'I', "I" }, { 'J', "B" },
    { 'K', "L" }, { 'L', "M" }, { 'M', "N" }, { 'N', "O" }, { 'O', "P" },
    { 'P', "Q" }, { 'Q', "R" }, { 'R', "S" }, { 'S', "T" }, { 'T', "U" },
    { 'U', "V" }, { 'V', "W" }, { 'W', "X" }, { 'X', "Y" }, { 'Y', "Z" },
    { 'Z', "E" },

    { '0', "3" }, { '2', "2" }, { '3', "4" }, { '4', "0" },
    { '5', "5" }, { '6', "6" }, { '7', "9" }, { '8', "8" }, { '9', "7" },

    { ' ', " " }, { '!', "!" }, { '"', "\"" }, { '#', "#" }, { '$', "$" },
    { '%', "%" }, { '&', "&" }, { '\'', "'" }, { '(', "(" }, { ')', ")" },
    { '*', "*" }, { '+', "+" }, { ',', "," }, { '-', "-" }, { '.', "." },
    { '/', "/" }, { ':', ":" }, { ';', ";" }, { '<', "<" }, { '=', "=" },
    { '>', ">" }, { '?', "?" }, { '@', "@" }, { '[', "[" }, { '\\', "\\" },
    { ']', "]" }, { '{', "{" }, { '}', "}" }, { '|', "|" },

    // Additional symbols
    { '€', "€" }, { '£', "£" }, { '¥', "¥" }, { '©', "©" }, { '®', "®" },
    { '™', "™" }, { '°', "°" }, { '±', "±" }, { '§', "§" }, { '•', "•" },
    { '∞', "∞" }, { 'π', "π" }, { '√', "√" }
};


    private static readonly Dictionary<string, char> HexToCharMap;

    static smp()
    {
        HexToCharMap = new Dictionary<string, char>();
        foreach (var pair in CharToHexMap)
        {
            // Ensure each hex value only maps back to one character
            HexToCharMap[pair.Value] = pair.Key;
        }
    }

    private static void CheckForDuplicateCharacters(Dictionary<char, string> map)
    {
        var keys = new HashSet<char>();
        var values = new HashSet<string>();
        var duplicateKeys = new HashSet<char>();
        var duplicateValues = new HashSet<string>();

        foreach (var entry in map)
        {
            // Check for duplicate keys
            if (!keys.Add(entry.Key))
            {
                duplicateKeys.Add(entry.Key);
            }

            // Check for duplicate values
            if (!values.Add(entry.Value))
            {
                duplicateValues.Add(entry.Value);
            }
        }

        // Log all duplicate keys
        if (duplicateKeys.Count > 0)
        {
            Debug.Log("Duplicate keys found:");
            foreach (var duplicateKey in duplicateKeys)
            {
                Debug.Log($"Duplicate key: {duplicateKey}");
            }
        }
        else
        {
            Debug.Log("No duplicate keys found.");
        }

        // Log all duplicate values
        if (duplicateValues.Count > 0)
        {
            Debug.Log("Duplicate values found:");
            foreach (var duplicateValue in duplicateValues)
            {
                Debug.Log($"Duplicate value: {duplicateValue}");
            }
        }
        else
        {
            Debug.Log("No duplicate values found.");
        }
    }

    public string ConvertStringToCustomHex(string input)
    {
        StringBuilder result = new StringBuilder();

        foreach (char c in input)
        {
            if (CharToHexMap.TryGetValue(c, out string hexValue))
            {
                result.Append(hexValue); // Append the corresponding hex value
            }
            else
            {
                // Append the original character and log a warning
                result.Append(c); // Keep the original character
                Debug.LogWarning($"Warning: No replacement found for character '{c}'. Using original character.");

                // Handle characters not found in the dictionary
                // throw new ArgumentException($"Invalid character '{c}' in input string.");
            }
        }

        return result.ToString();
    }

    public string ReconvertCustomHexToString(string input)
    {
        StringBuilder result = new StringBuilder();
        int i = 0;

        while (i < input.Length)
        {
            // Read single character from input
            string hex = input.Substring(i, 1);
            if (HexToCharMap.TryGetValue(hex, out char character))
            {
                result.Append(character);
            }
            else
            {
                result.Append(hex); // Keep the original character
                Debug.LogWarning($"Warning: No replacement found for character '{hex}'. Using original character.");

                // Handle unknown characters
                // result.Append('?'); // Placeholder for unknown characters
                // throw new ArgumentException($"Invalid hex character '{hex}' in input string.");
            }
            i += 1; // Move to the next character
        }

        return result.ToString();
    }


    public void Main()
    {
        /*
        string input = "Hello World!,.";
        string hexOutput = ConvertStringToCustomHex(input);
        string reconvertedText = ReconvertCustomHexToString(hexOutput);
        Debug.Log("input: " + input);
        Debug.Log("hexOutput: " + hexOutput);
        Debug.Log("Reconverted Text: " + reconvertedText);

        CheckForDuplicateCharacters(CharToHexMap);









        System.Random rand = new System.Random();
        var values = new List<string>(CharToHexMap.Values);
        var shuffledValues = new List<string>(values);

        // Shuffle the values
        for (int i = shuffledValues.Count - 1; i > 0; i--)
        {
            int j = rand.Next(i + 1);
            // Swap
            var temp = shuffledValues[i];
            shuffledValues[i] = shuffledValues[j];
            shuffledValues[j] = temp;
        }

        // Create a new dictionary with the shuffled values
        var shuffledDict = new Dictionary<char, string>();
        int index = 0;
        foreach (var key in CharToHexMap.Keys)
        {
            shuffledDict[key] = shuffledValues[index++];
        }
        string s="";
        // Output the shuffled dictionary
        foreach (var kvp in shuffledDict)
        {
            s=s+$"{{ '{kvp.Key}', \"{kvp.Value}\" }},";
        }
        Debug.Log(s);

        */
    } 

    #endregion


    #region AES Encrypt Decrypt

    public string encrypt(string encryptString, string DefualtEncryptionKey)
    {
        byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(DefualtEncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    int bufferSize = SetBufferSize; // 1 MB buffer
                    byte[] buffer = new byte[bufferSize];
                    int bytesRead = 0;
                    while (bytesRead < clearBytes.Length)
                    {
                        int bytesToRead = Math.Min(bufferSize, clearBytes.Length - bytesRead);
                        Buffer.BlockCopy(clearBytes, bytesRead, buffer, 0, bytesToRead);
                        cs.Write(buffer, 0, bytesToRead);
                        bytesRead += bytesToRead;
                    }
                    cs.Close();
                }
                string encryptedString = Convert.ToBase64String(ms.ToArray());
                return encryptedString;
            }
        }
    }

    public string Decrypt(string encryptedString, string DefualtEncryptionKey)
    {
        byte[] cipherBytes = Convert.FromBase64String(encryptedString);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(DefualtEncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    int bufferSize = SetBufferSize; // 1 MB buffer
                    byte[] buffer = new byte[bufferSize];
                    int bytesRead = 0;
                    while (bytesRead < cipherBytes.Length)
                    {
                        int bytesToRead = Math.Min(bufferSize, cipherBytes.Length - bytesRead);
                        Buffer.BlockCopy(cipherBytes, bytesRead, buffer, 0, bytesToRead);
                        cs.Write(buffer, 0, bytesToRead);
                        bytesRead += bytesToRead;
                    }
                    cs.Close();
                }
                string decryptedString = Encoding.Unicode.GetString(ms.ToArray());
                return decryptedString;
            }
        }
    }

    #endregion


    #region Compres Decompres 

    public string CompressString(string input)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);

        using (MemoryStream outputStream = new MemoryStream())
        {
            using (DeflateStream compressionStream = new DeflateStream(outputStream, CompressionMode.Compress))
            {
                int bufferSize = SetBufferSize; // 1 MB buffer
                byte[] buffer = new byte[bufferSize];
                int bytesRead = 0;
                while (bytesRead < inputBytes.Length)
                {
                    int bytesToRead = Math.Min(bufferSize, inputBytes.Length - bytesRead);
                    Buffer.BlockCopy(inputBytes, bytesRead, buffer, 0, bytesToRead);
                    compressionStream.Write(buffer, 0, bytesToRead);
                    bytesRead += bytesToRead;
                }
            }
            byte[] compressedBytes = outputStream.ToArray();
            string compressedString = Convert.ToBase64String(compressedBytes);

            return compressedString;
        }
    }

    public string DecompressString(string input)
    {
        byte[] compressedBytes = Convert.FromBase64String(input);
        using (MemoryStream compressedStream = new MemoryStream(compressedBytes))
        {
            using (DeflateStream decompressionStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
            {
                using (StreamReader sr = new StreamReader(decompressionStream))
                {
                    int bufferSize = SetBufferSize; // 1 MB buffer
                    char[] buffer = new char[bufferSize];
                    int bytesRead = 0;
                    StringBuilder sb = new StringBuilder();
                    while ((bytesRead = sr.Read(buffer, 0, bufferSize)) > 0)
                    {
                        sb.Append(buffer, 0, bytesRead);
                    }
                    string decompressedString = sb.ToString();
                    return decompressedString;
                }
            }
        }
    }

    #endregion


    #region RSA Encrypt Decrypt 

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
            UnityEngine.Debug.LogError("RSA Encryption failed: " + ex.Message);
            return "RSA Encryption failed: " + ex.Message;
        }
    }

    public string DecryptRSA(string txt, string pass)
    {
        try
        {
            if (rsa == null)
            {
                rsa = new RSACryptoServiceProvider();
            }

            //  rsa.FromXmlString(PrivateKey_IP.text);
            rsa.FromXmlString(pass);
            byte[] encryptedBytes = Convert.FromBase64String(txt);
            // Decrypt using PKCS#1 v1.5 padding
            byte[] decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.Pkcs1);
            return Encoding.UTF8.GetString(decryptedBytes);
        }
        catch (CryptographicException)
        {
            UnityEngine.Debug.LogError("Decryption failed. The data might be invalid or the key does not match.");
            return "";
        }
        catch (FormatException ex)
        {
            UnityEngine.Debug.LogError("The input is not a valid Base-64 string: " + ex.Message);
            return "";
        }
    }

    #endregion


    public string GenerateNewCustomKeyNow()
    {
        using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
        {
            int ckey = PlayerPrefs.GetInt("CustomKeyBitRate");
            int ckeyInBytes = ckey / 8;
            //   Debug.Log(ckeyInBytes.ToString());
            byte[] key = new byte[ckeyInBytes]; // 256 bits = 32 bytes
            rng.GetBytes(key);
            // Convert the key to a base64 string for easy storage or display
            return Convert.ToBase64String(key);
        }
    }


    #region Old Assign ReAssign Char 

    static Dictionary<char, string> char_values = new Dictionary<char, string>(){
    {'a', "00"}, {'A', "10"}, {'b', "01"}, {'B', "11"},
    {'c', "02"}, {'C', "12"}, {'d', "03"}, {'D', "13"},
    {'e', "04"}, {'E', "14"}, {'f', "05"}, {'F', "15"},
    {'g', "06"}, {'G', "16"}, {'h', "07"}, {'H', "17"},
    {'i', "08"}, {'I', "18"}, {'j', "09"}, {'J', "19"},
    {'k', "20"}, {'K', "30"}, {'l', "21"}, {'L', "31"},
    {'m', "22"}, {'M', "32"}, {'n', "23"}, {'N', "33"},
    {'o', "24"}, {'O', "34"}, {'p', "25"}, {'P', "35"},
    {'q', "26"}, {'Q', "36"}, {'r', "27"}, {'R', "37"},
    {'s', "28"}, {'S', "38"}, {'t', "29"}, {'T', "39"},
    {'u', "40"}, {'U', "50"}, {'v', "41"}, {'V', "51"},
    {'w', "42"}, {'W', "52"}, {'x', "43"}, {'X', "53"},
    {'y', "44"}, {'Y', "54"}, {'z', "45"}, {'Z', "55"},
    {' ', "56"}, {'.', "57"}, {',', "58"}, {':', "59"},
    {';', "60"}, {'_', "61"}, {'-', "62"}, {'(', "63"},
    {')', "64"}, {'@', "65"}, {'#', "66"}, {'%', "67"},
    {'!', "68"}, {'$', "69"}, {'=', "70"}, {'*', "71"},
    {'1', "72"}, {'2', "73"}, {'3', "74"}, {'4', "75"},
    {'5', "76"}, {'6', "77"}, {'7', "78"}, {'8', "79"},
    {'9', "80"}, {'0', "81"}, {'/', "82"},

    {'\"', "83"},

    {'\\', "84"}, {'\'', "85"},
    {'?', "86"},{'&', "87"},{'+', "88"},{'\r', "89"},{'\n', "90"},{'{', "91"},{'}', "92"},{'[', "93"},{']', "94"},{'<',"95"},{'>',"96"}

    };

    public string AssignHexValues(string s)
    {
        StringBuilder result = new StringBuilder(s.Length * 2);
        foreach (char c in s)
        {
            if (char_values.TryGetValue(c, out string hexValue))
            {
                result.Append(hexValue);
            }
            else
            {
                throw new ArgumentException($"Invalid character '{c}' in input string.");
            }
        }
        return result.ToString();
    }

    static Dictionary<string, char> hex_values = new Dictionary<string, char>()
        {
    {"00", 'a'}, {"10", 'A'}, {"01", 'b'}, {"11", 'B'},
    {"02", 'c'}, {"12", 'C'}, {"03", 'd'}, {"13", 'D'},
    {"04", 'e'}, {"14", 'E'}, {"05", 'f'}, {"15", 'F'},
    {"06", 'g'}, {"16", 'G'}, {"07", 'h'}, {"17", 'H'},
    {"08", 'i'}, {"18", 'I'}, {"09", 'j'}, {"19", 'J'},
    {"20", 'k'}, {"30", 'K'}, {"21", 'l'}, {"31", 'L'},
    {"22", 'm'}, {"32", 'M'}, {"23", 'n'}, {"33", 'N'},
    {"24", 'o'}, {"34", 'O'}, {"25", 'p'}, {"35", 'P'},
    {"26", 'q'}, {"36", 'Q'}, {"27", 'r'}, {"37", 'R'},
    {"28", 's'}, {"38", 'S'}, {"29", 't'}, {"39", 'T'},
    {"40", 'u'}, {"50", 'U'}, {"41", 'v'}, {"51", 'V'},
    {"42", 'w'}, {"52", 'W'}, {"43", 'x'}, {"53", 'X'},
    {"44", 'y'}, {"54", 'Y'}, {"45", 'z'}, {"55", 'Z'},
    {"56", ' '}, {"57", '.'}, {"58", ','}, {"59", ':'},
    {"60", ';'}, {"61", '_'}, {"62", '-'}, {"63", '('},
    {"64", ')'}, {"65", '@'}, {"66", '#'}, {"67", '%'},
    {"68", '!'}, {"69", '$'}, {"70", '='}, {"71", '*'},
    {"72", '1'}, {"73", '2'}, {"74", '3'}, {"75", '4'},
    {"76", '5'}, {"77", '6'}, {"78", '7'}, {"79", '8'},
    {"80", '9'}, {"81", '0'}, {"82", '/'}, {"83", '\"'},
    {"84", '\\'}, {"85", '\''},
            {"86", '?'},{"87", '&'},{"88", '+'},{"89", '\r'},{"90", '\n'},{"91", '{'},{"92", '}'},{"93", '['},{"94", ']'},{"95", '<'},{"96", '>'}
    };

    public string ReconvertHexString(string s)
    {
        StringBuilder result = new StringBuilder(s.Length / 2);
        for (int i = 0; i < s.Length; i += 2)
        {
            string hexPair = s.Substring(i, 2);
            if (hex_values.TryGetValue(hexPair, out char character))
            {
                result.Append(character);
            }
            else
            {
                throw new ArgumentException($"Invalid hex pair '{hexPair}' in input string.");
            }
        }
        return result.ToString();
    }

    #endregion


}
