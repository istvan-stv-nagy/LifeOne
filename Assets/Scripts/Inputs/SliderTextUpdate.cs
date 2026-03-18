using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderTextUpdate : MonoBehaviour
{
    public Slider slider;
    public TMP_Text text;
    public bool isInteger;
    public float multiplier = 1.0f;

    void Start()
    {
        UpdateText(slider.value);
        slider.onValueChanged.AddListener(UpdateText);
    }

    void UpdateText(float value)
    {
        value *= multiplier;
        if (isInteger)
        {
            int intValue = Mathf.RoundToInt(value);
            text.text = intValue.ToString();
        }
        else
        {
            text.text = value.ToString("0.00");
        }
    }

    void OnDestroy()
    {
        slider.onValueChanged.RemoveListener(UpdateText);
    }

    public float GetValueWithMultiplier()
    {
        return slider.value * multiplier;
    }
}
