using UnityEngine;
using UnityEngine.UI;

public class FastForwardButton : MonoBehaviour
{
    public SimulationSpeedManager simulationSpeedManager;
    public Image icon;
    public Sprite speed1;
    public Sprite speed2;
    public Sprite speed3;

    public void IterateFastForwardButton()
    {
        simulationSpeedManager.PressedFastForwardButton();
        int speed = simulationSpeedManager.GetSpeed();
        if (speed == 1)
        {
            icon.sprite = speed1;
        }
        else if (speed == 2)
        {
            icon.sprite = speed2;
        }
        else
        {
            icon.sprite = speed3;
        }
    }
}