using System.Threading.Tasks; // Add this line
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using Unity.Services.CloudSave.Models.Data.Player;
using SaveOptions = Unity.Services.CloudSave.Models.Data.Player.SaveOptions;
using System.Collections.Generic;
using TMPro;

public class AuthCloudSaveManager : MonoBehaviour
{
    public TMP_Text statusText;
    public TMP_InputField UserName_IP;
    public TMP_InputField Password_IP;

    private async void Awake()
    {
        // Initialize Unity services and sign in anonymously
        await UnityServices.InitializeAsync();
        //await SignInWithUsernamePasswordAsync("mbkhalid", "abcABC@1@");
        await SavePublicData();

    }
    public async void testsc()
    {
        Debug.Log("");
        RegisterWithUser();
    }
    public async Task RegisterWithUser()
    {
        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(UserName_IP.text, Password_IP.text);
            Debug.Log("SignUp is successful.");
            statusText.text = "SignUp is successful.";
        }
        catch (Unity.Services.Authentication.AuthenticationException ex)
        {
            Debug.LogException(ex);
            //   statusText.text = ex;

        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
            //   statusText.text = ex;

        }
    }

    async Task SignInWithUsernamePasswordAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            Debug.Log("SignIn is successful.");
        }
        catch (Unity.Services.Authentication.AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }
    // Method to save private player data
    public async void SaveData()
    {
        var playerData = new Dictionary<string, object>
        {
            {"firstKeyName", "a text value"},
            {"secondKeyName", 123}
        };
        await CloudSaveService.Instance.Data.Player.SaveAsync(playerData);
        Debug.Log($"Saved private data: {string.Join(", ", playerData)}");
    }

    // Method to save public data
    public async Task SavePublicData()
    {
        var publicData = new Dictionary<string, object>
        {
            {"publicKeyName", "public value"},
            {"anotherPublicKey", 456}
        };

        // Use PublicWriteAccessClassOptions to allow public access
        var saveOptions = new SaveOptions(new PublicWriteAccessClassOptions());

        await CloudSaveService.Instance.Data.Player.SaveAsync(publicData, saveOptions);
        Debug.Log($"Saved public data: {string.Join(", ", publicData)}");
    }

    // Method to load private player data
    public async void LoadData()
    {
        var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string>
        {
            "firstKeyName", "secondKeyName"
        });

        if (playerData.TryGetValue("firstKeyName", out var firstKey))
        {
            Debug.Log($"firstKeyName value: {firstKey.Value.GetAs<string>()}");
        }

        if (playerData.TryGetValue("secondKeyName", out var secondKey))
        {
            Debug.Log($"secondKey value: {secondKey.Value.GetAs<int>()}");
        }
    }

    // Method to load public data
    public async void LoadPublicData()
    {
        var publicKeys = new HashSet<string>
        {
            "publicKeyName", "anotherPublicKey"
        };

        // Assuming you have a method to get public data (implement as needed)
        var publicData = await CloudSaveService.Instance.Data.Player.LoadAsync(publicKeys);

        if (publicData.TryGetValue("publicKeyName", out var publicValue))
        {
            Debug.Log($"publicKeyName value: {publicValue.Value.GetAs<string>()}");
        }

        if (publicData.TryGetValue("anotherPublicKey", out var anotherPublicValue))
        {
            Debug.Log($"anotherPublicKey value: {anotherPublicValue.Value.GetAs<int>()}");
        }
    }
}
