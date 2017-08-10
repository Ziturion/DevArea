using System;
using UnityEngine;
using UnityEngine.UI;

public class CParameterUI : MonoBehaviour
{
    public Text ParameterName;
    public Slider ParameterValueSlider;
    public event Action<float> OnSliderValueChanged;

    public void Initialize(string parameterName, float startValue)
    {
        ParameterName.text = parameterName;
        ParameterValueSlider.value = startValue;
        ParameterValueSlider.onValueChanged.AddListener(SliderValueChanged);
    }

    public float GetValue()
    {
        return ParameterValueSlider.value;
    }

    public void SetInactive()
    {
        ParameterValueSlider.enabled = false;
    }

    private void SliderValueChanged(float value)
    {
        if(OnSliderValueChanged != null)
            OnSliderValueChanged.Invoke(value);
    }
}
