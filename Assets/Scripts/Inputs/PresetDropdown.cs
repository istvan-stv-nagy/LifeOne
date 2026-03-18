using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using System.Collections.Generic;

public class PresetDropdown : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    void Start()
    {
        UpdatePresetList();
    }

    public string GetText()
    {
        return dropdown.captionText.text;
    }

    public void UpdatePresetList()
    {
        List<string> presetNames = new List<string>();

        string[] files = Directory.GetFiles(Path.Combine(Application.persistentDataPath, "presets"), "*.json");
        foreach (string file in files)
        {
            string name = Path.GetFileNameWithoutExtension(file);
            presetNames.Add(name);  
        }

        dropdown.ClearOptions();
        dropdown.AddOptions(presetNames);
    }
}