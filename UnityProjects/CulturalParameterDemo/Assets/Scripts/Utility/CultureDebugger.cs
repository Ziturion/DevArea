using UnityEngine;
using UnityEngine.UI;

public class CultureDebugger : MonoBehaviour
{
    public Text CultureName, PopulationSize, Reputation, Production, Territory, RepRate, EscRate, InterRate;

    private Culture _currentCultureRef;

    public void UpdateCultureTexts(Culture culture = null)
    {
        if (culture == null)
            culture = _currentCultureRef;
        if (culture == null)
            return;

        CultureName.text = culture.Name;
        PopulationSize.text = culture.Variables.PopulationSize.ToString();
        Reputation.text = culture.Variables.Reputation.ToString();
        Production.text = culture.Variables.Production.ToString();
        Territory.text = culture.Variables.TerritorySize.ToString();
        RepRate.text = culture.Variables.ReproductionRate.ToString();
        EscRate.text = culture.Variables.EscalationRate.ToString();
        InterRate.text = culture.Variables.InteractionRate.ToString();
    }

    public void SetCulture(Culture culture)
    {
        _currentCultureRef = culture;

        UpdateCultureTexts();
    }
}
