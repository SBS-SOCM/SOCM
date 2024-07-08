using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphiccManager : MonoBehaviour
{
    public Toggle toggle;
    public Dropdown dropdown;

    private List<string> resolutions = new List<string>();

    bool isMax = true;

    private void Awake()
    {
        resolutions = new List<string>();
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            resolutions.Add($"{Screen.resolutions[i].width}x{Screen.resolutions[i].height}");
        }
        dropdown.AddOptions(resolutions);
    }

    public void ClickToggle()
    {
        if (toggle.isOn)
        {
            isMax = true;
        }
        else
        {
            isMax = true;
        }
    }

    public void ChangeGraphic()
    {
        int resolutionIndex = dropdown.value;
        int width = Screen.resolutions[resolutionIndex].width;
        int height = Screen.resolutions[resolutionIndex].height;
        Screen.SetResolution(width, height, isMax);
    }

}
