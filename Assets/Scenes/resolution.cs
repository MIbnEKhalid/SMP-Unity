using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class resolution : MonoBehaviour
{ 
    public TMP_Dropdown resolutionDropdown; // Reference to the dropdown UI element
      private Resolution[] resolutions;    // Array of available screen resolutions

    void Start()
    {
        // Fetch all available screen resolutions
        resolutions = Screen.resolutions;

        // Clear existing options
        resolutionDropdown.ClearOptions();

        // Create a list of options in string format
        var options = new System.Collections.Generic.List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            // Filter resolutions to only include those with a 165Hz refresh rate
            if (resolutions[i].refreshRate == 165)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);

                // Check if this resolution is the current one
                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height &&
                    resolutions[i].refreshRate == Screen.currentResolution.refreshRate)
                {
                    currentResolutionIndex = options.Count - 1; // Set to current resolution index
                }
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

    // Method to change the screen resolution based on selected dropdown option
    void SetResolution(int resolutionIndex)
    {
        // Only consider resolutions with 165Hz refresh rate
        Resolution[] filteredResolutions = System.Array.FindAll(resolutions, res => res.refreshRate == 165);

        if (resolutionIndex >= 0 && resolutionIndex < filteredResolutions.Length)
        {
            Resolution resolution = filteredResolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode, resolution.refreshRate);
        }
    }








    /*
    public TMP_Dropdown resolutionDropdown; // Reference to the dropdown UI element

    private Resolution[] resolutions;
    // Start is called before the first frame update

    void Start()
    {
        // Fetch all available screen resolutions
        resolutions = Screen.resolutions;

        // Clear existing options
        resolutionDropdown.ClearOptions();

        // Create a list of options in string format
        var options = new System.Collections.Generic.List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            // Filter resolutions to only include those with a 165Hz refresh rate
            if (resolutions[i].refreshRate == 165)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);

                // Check if this resolution is the current one
                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height &&
                    resolutions[i].refreshRate == Screen.currentResolution.refreshRate)
                {
                    currentResolutionIndex = options.Count - 1; // Set to current resolution index
                }
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

    // Method to change the screen resolution based on selected dropdown option
    void SetResolution(int resolutionIndex)
    {
        // Only consider resolutions with 165Hz refresh rate
        Resolution[] filteredResolutions = System.Array.FindAll(resolutions, res => res.refreshRate == 165);

        if (resolutionIndex >= 0 && resolutionIndex < filteredResolutions.Length)
        {
            Resolution resolution = filteredResolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode, resolution.refreshRate);
        }
    }*/
}
