using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

public class CloudSaveScript : MonoBehaviour
{
    async void Start()
    {
        try
        {
            Debug.Log("TEST1");
            await UnityServices.InitializeAsync();
            Debug.Log("TEST2");

            // Sign in the user before attempting to save data
            await SignInWithUsernamePasswordAsync("mbkhalid", "abcABC@1@");
            Debug.Log("TEST3 - User signed in, proceeding with Cloud Save.");

            // Now that we're authenticated, we can use Cloud Save
            var data = new Dictionary<string, object> { { "MySaveKey", "HelloWorld" } };
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);

            Debug.Log("Cloud save was successful!");
        }
        catch (CloudSaveException ex)
        {
            Debug.LogError("Cloud Save failed: " + ex.Message);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogError("Request failed: " + ex.Message);
        }
    }

    async Task SignInWithUsernamePasswordAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            Debug.Log("SignIn is successful.");
        }
        catch (Unity.Services.Authentication.AuthenticationException ex)  // Fully qualify the Unity Authentication exception
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }
}
