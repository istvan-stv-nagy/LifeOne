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

        string persistentPath = Path.Combine(Application.persistentDataPath, "presets");

        if (!Directory.Exists(persistentPath))
        {
            Directory.CreateDirectory(persistentPath);
            string streamPath = Path.Combine(Application.streamingAssetsPath, "presets");
            string[] originalFiles = Directory.GetFiles(streamPath, "*.json");
            foreach (string file in originalFiles)
            {
                File.Copy(file, Path.Combine(persistentPath, Path.GetFileName(file)));
            }
        }

        string[] files = Directory.GetFiles(persistentPath, "*.json");
        foreach (string file in files)
        {
            string name = Path.GetFileNameWithoutExtension(file);
            presetNames.Add(name);  
        }

        dropdown.ClearOptions();
        dropdown.AddOptions(presetNames);
    }
}