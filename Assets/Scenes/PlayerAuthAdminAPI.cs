using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using Unity.Services.Core;

public class PlayerAuthAdminAPI : MonoBehaviour
{
    // Replace with your actual project ID and service account token
    private string projectId = "55b72e93-03f4-49e1-aadd-10b0ba50fe9a";
    private string serviceAccountToken = "wSt0C6OsJEfjWpZ9kMzthoIPLNkJz1_3";

    void Start()
    { 
         
        string username = "mbkhalid";
        string password = "abcABC@1@";  

        // Start fetching player usernames
        StartCoroutine(FetchPlayerUsernames());
    }

    IEnumerator FetchPlayerUsernames()
    {
        // API endpoint to list players
        string url = $"https://services.docs.unity.com/player-identity/v1/projects/{projectId}/users?limit=100";

        // Create a UnityWebRequest with the appropriate headers
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", "Basic " + serviceAccountToken);
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request and wait for the response
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error: {request.error}");
        }
        else
        {
            // Successfully received response
            string jsonResponse = request.downloadHandler.text;

            // Parse the JSON response using Newtonsoft.Json
            JObject json = JObject.Parse(jsonResponse);

            // Loop through the results and print out player information
            foreach (var player in json["results"])
            {
                string playerId = player["id"].ToString();
                string username = player["username"]?.ToString() ?? "No username";

                Debug.Log($"Player ID: {playerId}, Username: {username}");
            }

            // Handle pagination if needed (look for "next" token)
            string nextPage = json["next"]?.ToString();
            if (!string.IsNullOrEmpty(nextPage))
            {
                Debug.Log($"Next page token: {nextPage}");
                // Optionally, you could implement pagination handling here
            }
        }
    }
}
