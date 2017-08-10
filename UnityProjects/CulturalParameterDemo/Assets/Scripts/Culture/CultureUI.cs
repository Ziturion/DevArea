using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CultureUI : MonoBehaviour
{
    public Text CultureName;
    public CParameterUI ParameterPrefab;
    public Image BackgroundImage;
    public event Action<string, float> OnParameterValueChanged; 

    private readonly List<CParameterUI> _allParameters = new List<CParameterUI>();

    public void Initialize(string cultureName, Color cultureColor, CultureParameter[] parameters)
    {
        CultureName.text = cultureName;
        BackgroundImage.color = new Color(cultureColor.r, cultureColor.g, cultureColor.b, 0.5f);

        foreach (CultureParameter parameter in parameters)
        {
            CParameterUI uiParam = Instantiate(ParameterPrefab, transform);
            uiParam.Initialize(parameter.Name,parameter.Value);
            uiParam.OnSliderValueChanged += (t) => ParameterValueChanged(t, parameter.Name);
            _allParameters.Add(uiParam);
        }
    }

    public void SetInactive()
    {
        CultureName.color = Color.gray;
        BackgroundImage.color = new Color(205,205,205,255);

        foreach (CParameterUI parameter in _allParameters)
        {
            parameter.SetInactive();
        }
    }

    private void ParameterValueChanged(float value, string name)
    {
        if(OnParameterValueChanged != null)
            OnParameterValueChanged.Invoke(name, value);
    }
}
