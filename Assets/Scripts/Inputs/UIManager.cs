using UnityEngine;

public enum UIState
{
    MAIN,
    PRESETS,
    CUSTOMIZE,
    EXPORT
}

public class UIManager : MonoBehaviour
{
    public GameObject presetPanel;
    public GameObject customizePanel;
    public GameObject exportPanel;

    private UIState state;

    void Start()
    {
        state = UIState.MAIN;
    }

    public void UpdateUI()
    {
        presetPanel.SetActive(state == UIState.PRESETS);
        customizePanel.SetActive(state == UIState.CUSTOMIZE);
        exportPanel.SetActive(state == UIState.EXPORT);
    }

    public void PressedPresetsButton()
    {
        if (state == UIState.PRESETS)
            state = UIState.MAIN;
        else
            state = UIState.PRESETS;
        
        UpdateUI();
    }

    public void PressedCustomizeButton()
    {
        if (state == UIState.CUSTOMIZE)
            state = UIState.MAIN;
        else
            state = UIState.CUSTOMIZE;
        
        UpdateUI();
    }

    public void PressedExportButton()
    {
        if (state == UIState.EXPORT)
            state = UIState.MAIN;
        else
            state = UIState.EXPORT;
        UpdateUI();
    }

    public void PressedQuitButton()
    {
        Application.Quit();
    }
}
