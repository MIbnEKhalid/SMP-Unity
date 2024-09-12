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

public class main : MonoBehaviour
{
    public TMP_Text fpsText;
    private float updateInterval = 0.5f; // Update every 0.5 seconds
    private float accum = 0.0f; // FPS accumulated over the interval
    private int frames = 0; // Frames drawn over the interval
    private float timeLeft;

    public CanvasGroup canvasGroup;  // Reference to the CanvasGroup
    public float fadeDuration = 0.5f;  // Adjust duration for fade in/out
    public float fadeDelay = 0.5f;
    public TMP_Text Message;

    #region Update

    void Update()
    {

        #region Fps
        timeLeft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        frames++;
        if (timeLeft <= 0.0)
        {
            float fps = accum / frames;
            fpsText.text = "FPS: " + Mathf.Round(fps).ToString();
            timeLeft = updateInterval;
            accum = 0.0f;
            frames = 0;
        }
        #endregion

        string currentSceneName = SceneManager.GetActiveScene().name;

        if (Input.GetKey(KeyCode.Escape) && currentSceneName != "MainMenu")
        {
            if (currentSceneName != "main")
            {
                SceneManager.LoadScene("MainMenu");
                Debug.Log("Escape pressed");
            }
        }

    }

    #endregion

    public void SceneTelepoter(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }
    public void QuitTheGame()
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            Application.Quit();
        }
        Debug.Log("Quiting");
    }

    private IEnumerator Start()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "MainMenu")
        {
            yield return StartCoroutine(FadeOut());

        }

        // Program.Main();
    }

    public void OpenLink(string link)
    {
        Application.OpenURL(link);
    }

    #region Copy/Paste/Clear/SceneTelepoter

    public void PasteTextToInput(TMP_InputField inputField)
    {
        string clipboardText = GUIUtility.systemCopyBuffer;
        inputField.text = clipboardText;
    }

    public void CopyTxt(TMP_InputField inputField)
    {
        TextEditor textEditor = new TextEditor();
        textEditor.text = inputField.text;
        textEditor.OnFocus();
        textEditor.SelectAll();
        textEditor.Copy();
    }

    public void ClearOuput(TMP_InputField inputField)
    {
        inputField.text = "";
    }

    #endregion

    public void ShowCopyMessage(string message)
    {
        StartCoroutine(showMessage(message));
    }
    private IEnumerator showMessage(string txt)
    {
        Message.text = txt;
        yield return StartCoroutine(FadeIn());
        yield return new WaitForSeconds(0.7f);
        yield return StartCoroutine(FadeOut());
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