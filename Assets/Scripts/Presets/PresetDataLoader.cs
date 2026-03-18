using UnityEngine;
using System.IO;

public class PresetDataLoader
{
    public static void SavePreset(SimulationParameters preset, string presetName)
    {
        string json = JsonUtility.ToJson(preset, true);

        string folderPath = Path.Combine(Application.persistentDataPath, "presets");
        Directory.CreateDirectory(folderPath);
        string path = Path.Combine(folderPath, presetName + ".json");
        File.WriteAllText(path, json);
        Debug.Log($"Preset {presetName} saved to {path}");
    }

    public static SimulationParameters LoadPreset(string presetName)
    {
        string path = Path.Combine(Application.persistentDataPath, "presets", presetName + ".json");
        if (!File.Exists(path))
        {
            Debug.LogError($"Preset {presetName} not found!");
            return null;
        }

        string json = File.ReadAllText(path);

        return JsonUtility.FromJson<SimulationParameters>(json);
    }
}