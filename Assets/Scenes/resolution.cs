using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResolutionManager : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown; // Reference to the dropdown UI element

    private Resolution[] resolutions; // All available screen resolutions

    void Start()
    {
        // Fetch all available screen resolutions
        resolutions = Screen.resolutions;

        // Clear existing options
        resolutionDropdown.ClearOptions();

        // Create a list of options in string format
        var options = new List<string>();
        int currentResolutionIndex = 0;

        // Iterate through all available resolutions
        for (int i = 0; i < resolutions.Length; i++)
        {
            // Create a string for each resolution, including only width and height
            string option = resolutions[i].width + " x " + resolutions[i].height;

            // Add only unique resolutions to avoid duplicates in the dropdown
            if (!options.Contains(option))
            {
                options.Add(option);
            }

            // Check if this resolution is the current one
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = options.IndexOf(option); // Update index for the current resolution
            }
        }

        // Add options to the dropdown
        resolutionDropdown.AddOptions(options);

        // Set the dropdown to the currently used resolution
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Add listener for when the dropdown value changes
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    // Method to change the screen resolution based on the selected dropdown option
    void SetResolution(int resolutionIndex)
    {
        if (resolutionIndex >= 0 && resolutionIndex < resolutions.Length)
        {
            // Get selected resolution width and height
            string[] selectedOption = resolutionDropdown.options[resolutionIndex].text.Split('x');
            int width = int.Parse(selectedOption[0].Trim());
            int height = int.Parse(selectedOption[1].Trim());

            // Find the closest matching resolution with the same width and height
            Resolution resolution = System.Array.Find(resolutions, res => res.width == width && res.height == height);

            if (resolution.width > 0 && resolution.height > 0)
            {
                // Set the resolution to the closest match found with the desired width and height
                Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode, resolution.refreshRate);
            }
        }
    }
}
