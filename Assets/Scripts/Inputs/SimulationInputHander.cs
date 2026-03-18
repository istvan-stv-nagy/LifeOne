using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class SimulationInputHander : MonoBehaviour
{
    [SerializeField]
    private SimulationManager simulationManager;

    [SerializeField]
    private ParticleRenderer particleRenderer;

    [SerializeField]
    private Slider numTypesSlider;

    [SerializeField]
    private Slider numParticlesSlider;

    [SerializeField]
    private Slider attractionSlider;

    [SerializeField]
    private Slider interactionRadiusSlider;

    [SerializeField]
    private TMP_InputField presetNameField;

    [SerializeField]
    private TMP_Dropdown presetDropdown;

    private SimulationControls controls;

    private void Awake()
    {
        controls = new SimulationControls();
    }

    private void OnEnable()
    {
        controls.Simulation.Enable();
    }

    private void OnDisable()
    {
        controls.Simulation.Disable();
    }

    public void OnSelectRandom()
    {
        int numTypesSelected = (int)numTypesSlider.GetComponent<SliderTextUpdate>().GetValueWithMultiplier();
        int numParticlesSelected = (int)numParticlesSlider.GetComponent<SliderTextUpdate>().GetValueWithMultiplier();
        float attractionStrength = attractionSlider.GetComponent<SliderTextUpdate>().GetValueWithMultiplier();
        float interactionRadius = interactionRadiusSlider.GetComponent<SliderTextUpdate>().GetValueWithMultiplier();
        simulationManager.SelectRandomized(numTypesSelected, numParticlesSelected, attractionStrength, interactionRadius);
    }

    public void OnCluster()
    {
        particleRenderer.SetVisuType(1);
        simulationManager.Cluster();
    }

    public void OnVisualizeParticleTypes()
    {
        particleRenderer.SetVisuType(0);
        simulationManager.VisualizeParticleTypes();
    }

    public void OnSavePreset()
    {
        simulationManager.SavePreset(presetNameField.text);
    }

    public void OnLoadPreset()
    {
        string selectedPresetName = presetDropdown.captionText.text;
        SimulationParameters preset = PresetDataLoader.LoadPreset(selectedPresetName);
        simulationManager.LoadPreset(preset);
    }
}
