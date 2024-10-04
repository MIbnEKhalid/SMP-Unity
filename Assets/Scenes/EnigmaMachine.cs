using System;
using System.Collections.Generic;
using UnityEngine;

public class EnigmaMachine : MonoBehaviour
{
    private string alphabet = "abcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-_=+[]{};:'\",.<>?/|\\`~";
    private List<int> rotor1 = new List<int>() { 4, 10, 12, 0, 3, 7, 8, 6, 9, 2, 1, 5, 11, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
    private List<int> rotor2 = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
    private List<int> rotor3 = new List<int>() { 25, 24, 23, 22, 21, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };
    private int[] rotorPositions = { 0, 0, 0 }; // Positions for rotors

    public void Start()
    {
        string plaintext = "hello123!";
        string encrypted = Encrypt(plaintext);
        string decrypted = Decrypt(encrypted);
        Debug.Log($"Encrypted: {encrypted}");
        Debug.Log($"Decrypted: {decrypted}");
    }

    public string Encrypt(string input)
    {
        return ProcessInput(input, true);
    }

    public string Decrypt(string input)
    {
        return ProcessInput(input, false);
    }

    private string ProcessInput(string input, bool isEncrypting)
    {
        string output = "";

        foreach (char c in input)
        {
            if (alphabet.Contains(c))
            {
                int index = alphabet.IndexOf(c);
                index = isEncrypting ? EncryptCharacter(index) : DecryptCharacter(index);
                output += alphabet[index];
                RotateRotors();
            }
            else
            {
                output += c; // Preserve characters not in the alphabet
            }
        }

        return output;
    }

    private int EncryptCharacter(int index)
    {
        return TransformCharacter(index);
    }

    private int DecryptCharacter(int index)
    {
        return ReverseTransformCharacter(index);
    }

    private int TransformCharacter(int index)
    {
        // Pass through rotors
        index = (index + rotorPositions[0]) % alphabet.Length;
        index = rotor1[index];
        index = (index + rotorPositions[1]) % alphabet.Length;
        index = rotor2[index];
        index = (index + rotorPositions[2]) % alphabet.Length;
        index = rotor3[index];
        return index;
    }

    private int ReverseTransformCharacter(int index)
    {
        // Pass back through rotors in reverse order
        index = rotor3.IndexOf(index);
        index = (index - rotorPositions[2] + alphabet.Length) % alphabet.Length;
        index = rotor2.IndexOf(index);
        index = (index - rotorPositions[1] + alphabet.Length) % alphabet.Length;
        index = rotor1.IndexOf(index);
        index = (index - rotorPositions[0] + alphabet.Length) % alphabet.Length;
        return index;
    }

    private void RotateRotors()
    {
        rotorPositions[0] = (rotorPositions[0] + 1) % alphabet.Length;
        if (rotorPositions[0] == 0) // Rotate second rotor on full rotation of first
        {
            rotorPositions[1] = (rotorPositions[1] + 1) % alphabet.Length;
            if (rotorPositions[1] == 0) // Rotate third rotor on full rotation of second
            {
                rotorPositions[2] = (rotorPositions[2] + 1) % alphabet.Length;
            }
        }
    }
}
