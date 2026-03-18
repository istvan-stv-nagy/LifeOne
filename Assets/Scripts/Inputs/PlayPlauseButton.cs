using UnityEngine;
using UnityEngine.UI;

public class PlayPauseButton : MonoBehaviour
{
    public SimulationSpeedManager simulationSpeedManager;
    public Image icon;
    public Sprite playSprite;
    public Sprite pauseSprite;

    public void TogglePlayPause()
    {
        simulationSpeedManager.PressedPlayPausedButton();
        SimulationSpeed state = simulationSpeedManager.GetState();
        if (state == SimulationSpeed.PAUSED)
        {
            icon.sprite = pauseSprite;
        }
        else
        {
            icon.sprite = playSprite;
        }
    }
}