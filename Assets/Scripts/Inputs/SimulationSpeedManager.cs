using UnityEngine;

public enum SimulationSpeed
{
    PAUSED,
    NORMAL
}

public class SimulationSpeedManager : MonoBehaviour
{
    private SimulationSpeed state;
    private int simulationTimeSpeed = 1;

    public SimulationManager simulationManager;

    void Start()
    {
        state = SimulationSpeed.NORMAL;
        simulationTimeSpeed = 1;
    }

    public SimulationSpeed GetState()
    {
        return state;
    }

    public int GetSpeed()
    {
        return simulationTimeSpeed;
    }

    public void UpdateSimulationSpeed()
    {
        if (state == SimulationSpeed.NORMAL)
            simulationManager.SetSimulationSpeed(simulationTimeSpeed);
        else if (state == SimulationSpeed.PAUSED)
            simulationManager.SetSimulationSpeed(0f);
    }

    public void PressedPlayPausedButton()
    {
        if (state == SimulationSpeed.PAUSED)
            state = SimulationSpeed.NORMAL;
        else if (state == SimulationSpeed.NORMAL)
            state = SimulationSpeed.PAUSED;
        UpdateSimulationSpeed();
    }

    public void PressedFastForwardButton()
    {
        simulationTimeSpeed = simulationTimeSpeed % 3 + 1;
        UpdateSimulationSpeed();
    }
}