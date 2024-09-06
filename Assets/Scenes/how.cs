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
public class how : MonoBehaviour
{
    public TMP_InputField inputField;
    private void Start()
    {
        inputField.text = "<size=30><b>Instructions for Using the Encryption and Decryption App</b></size>\r\n\r\n<indent=10><size=25><b>Generating a Key Pair</b></size></indent>\r\n\r\n<indent=20><size=20>• <i>Objective</i>: Before you can encrypt or decrypt messages, you need to generate a pair of keys: a public key and a private key.</size></indent>\r\n<indent=20><size=20>• <i>Steps</i>:</size></indent>\r\n<indent=30><size=20>1. Navigate to the <b>\"Generate Key Pair\"</b> page.</size></indent>\r\n<indent=30><size=20>2. Click on the <b>\"Generate Keys\"</b> button.</size></indent>\r\n<indent=30><size=20>3. The app will create a public key and a private key for you:</size></indent>\r\n<indent=40><size=20>• <b>Public Key</b>: Share this with anyone who wants to send you encrypted messages.</size></indent>\r\n<indent=40><size=20>• <b>Private Key</b>: Keep this safe and secure; it is only for you to decrypt messages sent to you.</size></indent>\r\n\r\n<indent=10><size=25><b>Encrypting a Message</b></size></indent>\r\n\r\n<indent=20><size=20>• <i>Objective</i>: To send an encrypted message, you need the recipient’s public key.</size></indent>\r\n<indent=20><size=20>• <i>Steps</i>:</size></indent>\r\n<indent=30><size=20>1. Go to the <b>\"Encrypt\"</b> page.</size></indent>\r\n<indent=30><size=20>2. Enter your message in the <b>\"Message\"</b> field.</size></indent>\r\n<indent=30><size=20>3. Paste the recipient’s public key into the <b>\"Recipient’s Public Key\"</b> field.</size></indent>\r\n<indent=30><size=20>4. Click on the <b>\"Encrypt\"</b> button.</size></indent>\r\n<indent=30><size=20>5. The app will create an encrypted message with two components:</size></indent>\r\n<indent=40><size=20>• <b>The Encrypted Custom Key</b>: Encrypted with the recipient’s public key.</size></indent>\r\n<indent=40><size=20>• <b>The Encrypted Message</b>: Encrypted using the custom key.</size></indent>\r\n<indent=30><size=20>6. Copy the encrypted message and send it to the recipient.</size></indent>\r\n\r\n<indent=10><size=25><b>Decrypting a Message</b></size></indent>\r\n\r\n<indent=20><size=20>• <i>Objective</i>: To read an encrypted message, you need your private key.</size></indent>\r\n<indent=20><size=20>• <i>Steps</i>:</size></indent>\r\n<indent=30><size=20>1. Go to the <b>\"Decrypt\"</b> page.</size></indent>\r\n<indent=30><size=20>2. Paste the encrypted message into the <b>\"Encrypted Message\"</b> field.</size></indent>\r\n<indent=30><size=20>3. Enter your private key into the <b>\"Private Key\"</b> field.</size></indent>\r\n<indent=30><size=20>4. Click on the <b>\"Decrypt\"</b> button.</size></indent>\r\n<indent=30><size=20>5. The app will use your private key to decrypt the custom key and then use this key to decrypt the message.</size></indent>\r\n<indent=30><size=20>6. You will see the original message displayed.</size></indent>\r\n\r\n<indent=10><size=25><b>Important Notes</b></size></indent>\r\n\r\n<indent=20><size=20>• <b>Keep Your Private Key Safe</b>: Never share your private key. If someone else obtains it, they can read all your encrypted messages.</size></indent>\r\n<indent=20><size=20>• <b>Share Your Public Key</b>: Feel free to share your public key with anyone who wants to send you encrypted messages.</size></indent>\r\n";

    }
}
