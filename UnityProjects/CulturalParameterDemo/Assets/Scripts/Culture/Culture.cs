using System.Linq;
using UnityEngine;

[System.Serializable]
public class Culture
{
    public string Name;
    public CultureParameter[] Parameter;
    public Color CultureColor;
    public Vector2 StartPosition;

    public Culture(string cultureName, Color cultureColor, Vector2 position, params CultureParameter[] parameter)
    {
        Name = cultureName;
        AddParameter(parameter);
        CultureColor = cultureColor;
        StartPosition = position;
    }

    public void AddParameter(params CultureParameter[] parameter)
    {
        if (Parameter != null)
        {
            CultureParameter[] newParams = new CultureParameter[Parameter.Length + parameter.Length];
            for (int i = 0; i < Parameter.Length; i++)
            {
                newParams[i] = new CultureParameter(Parameter[i].Name, Parameter[i].Value);
            }
            for (int i = 0; i < parameter.Length; i++)
            {
                newParams[i + Parameter.Length] = new CultureParameter(parameter[i].Name, parameter[i].Value);
            }
            Parameter = newParams;
        }
        else
        {
            Parameter = new CultureParameter[parameter.Length];
            for (int i = 0; i < Parameter.Length; i++)
            {
                Parameter[i] = new CultureParameter(parameter[i].Name, parameter[i].Value);
            }
        }

    }

    public void ChangeParameterValue(string paramName, float newValue)
    {
        if (Parameter.Any(t => t.Name == paramName))
        {
            Parameter.First(t => t.Name == paramName).Value = newValue;
        }
    }

    /// <summary>
    /// Gets the Value from a Parameter by name.
    /// </summary>
    /// <param name="paramName">the name of the parameter</param>
    /// <returns>Returns -1 if the paramName didnt find a match</returns>
    public float GetParameterValue(string paramName)
    {
        if (Parameter.Any(t => t.Name == paramName))
        {
            return Parameter.First(t => t.Name == paramName).Value;
        }
        return -1;
    }

    public void Update()
    {
        
    }
}

[System.Serializable]
public class CultureParameter
{
    public string Name;
    public float Value;

    public CultureParameter(string name, float value)
    {
        Name = name;
        Value = value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
