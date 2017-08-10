using System;
using System.Linq;
using UnityEngine;
using Ziturion.BehaviourTree;
using Random = UnityEngine.Random;

[System.Serializable]
public class Culture
{
    public string Name;
    public CultureParameter[] Parameter;
    public Color CultureColor;
    public Vector2 StartPosition;
    public CultureVariables Variables;
    public BehaviourTree BT_Culture;
    public event Action<Culture> OnPopulationEnd;
    public event Action<Culture> OnTerritoryEnd;

    public Culture(string cultureName, Color cultureColor, Vector2 position, params CultureParameter[] parameter)
    {
        Name = cultureName;
        AddParameter(parameter);
        CultureColor = cultureColor;
        StartPosition = position;

        BT_Culture = new BehaviourTree(this);

        //example Behaviour Tree
        BT_Selector selector = new BT_WeightedSelector("Start Selector",
            new BT_WeightedSelector.WeightedPair(
                GetParameterValue("Behaviour"),
                new BT_Selector("Behaviour", new BT_Kill("Kill"), new BT_Claim("Claim: Behaviour", CultureType.Behaviour))),
            new BT_WeightedSelector.WeightedPair(
                GetParameterValue("Communication"),
                new BT_Selector("Communication", new BT_Trade("Trade"), new BT_Reputation("Reputation: Communication", CultureType.Communication), new BT_Claim("Claim: Communication", CultureType.Communication))),
            new BT_WeightedSelector.WeightedPair(
                GetParameterValue("Economics"),
                new BT_Selector("Economics",  new BT_Reputation("Reputation: Economics", CultureType.Economics), new BT_Claim("Claim: Economics", CultureType.Economics))),
            new BT_WeightedSelector.WeightedPair(
                GetParameterValue("Sociology"),
                new BT_Selector("Sociology", new BT_Reputation("Reputation: Sociology", CultureType.Sociology))));
        BT_Culture.Add(selector);
        //example Behaviour Tree

        Variables.PopulationSize = Random.Range(15, 30); //adding Start Population
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

        //This is not the optimal solution
        BT_WeightedSelector selector = BT_Culture.GetChild(0) as BT_WeightedSelector;
        if (selector == null)
            return;
        int index = paramName == "Behaviour" ? 0 : paramName == "Communication" ? 1 : paramName == "Economics" ? 2 : paramName == "Sociology" ? 3 : -1;

        selector.ChangeWeight(index, newValue);
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
        BT_Culture.Run();
        UpdateVariables();
    }

    public static bool operator ==(Culture a, Culture b)
    {
        // If both are null, or both are same instance, return true.
        if (ReferenceEquals(a, b))
        {
            return true;
        }

        // If one is null, but not both, return false.
        if (((object)a == null) || ((object)b == null))
        {
            return false;
        }

        // Return true if the fields match:
        return a.Name == b.Name && a.CultureColor == b.CultureColor && a.StartPosition == b.StartPosition;
    }

    public static bool operator !=(Culture a, Culture b)
    {
        return !(a == b);
    }

    protected bool Equals(Culture other)
    {
        return string.Equals(Name, other.Name) && CultureColor.Equals(other.CultureColor) && StartPosition.Equals(other.StartPosition);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Culture)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = (Name != null ? Name.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ CultureColor.GetHashCode();
            hashCode = (hashCode * 397) ^ StartPosition.GetHashCode();
            return hashCode;
        }
    }

    public void AddLosses(float lossPercantage)
    {
        Variables.PopulationSize -= Mathf.RoundToInt((Variables.PopulationSize * lossPercantage) / Variables.TerritorySize);
        Variables.Reputation -= Mathf.RoundToInt(((Variables.Reputation * lossPercantage) * 0.1f) / Variables.TerritorySize);//Reputation Weight Loss
        Variables.Production -= Mathf.RoundToInt(((Variables.Production * lossPercantage) * 0.4f) / Variables.TerritorySize);//Prodcution Weight Loss
    }

    private void UpdateVariables()
    {
        TileResources resources = new TileResources();
        Tile[] territory = TerrainManager.Instance.GetTerritory(this);
        Variables.TerritorySize = territory.Length;
        foreach (Tile tile in territory)
        {
            resources.Water += tile.Resources.Water;
            resources.Food += tile.Resources.Food;
            resources.Production += tile.Resources.Production;
            resources.Goods += tile.Resources.Goods;
        }
        Variables.EscalationRate = GetParameterValue("Behaviour");
        Variables.ReproductionRate = (1 - ((Variables.PopulationSize / Variables.TerritorySize) / CultureManager.MaxPopulationPerTile)) * (GetParameterValue("Sociology") * 2);
        Variables.InteractionRate = GetParameterValue("Communication");

        //resources now has the complete resources of this culture
        if (resources.Water > Variables.PopulationSize / CultureManager.MaxPopulationPerWater &&
            resources.Food > Variables.PopulationSize / CultureManager.MaxPopulationPerFood &&
            resources.Production > Variables.PopulationSize / CultureManager.MaxPopulationPerProduction &&
            resources.Goods > Variables.PopulationSize / CultureManager.MaxPopulationPerGoods)
        {
            Variables.PopulationSize += Mathf.RoundToInt(Random.Range(0,15) * Variables.ReproductionRate);
        }

        Variables.Production += Mathf.RoundToInt(resources.Production / Variables.PopulationSize);

        if (Variables.PopulationSize <= 0)
        {
            if (OnPopulationEnd != null)
                OnPopulationEnd.Invoke(this);
        }

        if (Variables.TerritorySize <= 0)
        {
            if (OnTerritoryEnd != null)
                OnTerritoryEnd.Invoke(this);
        }

        if (Variables.ReproductionRate <= -10f)
        {
            Variables.PopulationSize -= Random.Range(60, 100);
        }

    }

    /// <summary>
    /// adds reputation to the culture
    /// </summary>
    /// <param name="value">negative value means decrease</param>
    public void AddReputation(float value)
    {
        Variables.Reputation += value;
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

[System.Serializable]
public struct CultureVariables
{
    public float PopulationSize;
    public float Reputation;
    public float Production;
    public float TerritorySize;

    public float ReproductionRate;
    public float EscalationRate;
    public float InteractionRate;

    public CultureVariables(float populationSize, float reputation, float production, float territorySize, float reproductionRate, float escalationRate, float interactionRate)
    {
        PopulationSize = populationSize;
        Reputation = reputation;
        Production = production;
        TerritorySize = territorySize;
        ReproductionRate = reproductionRate;
        EscalationRate = escalationRate;
        InteractionRate = interactionRate;
    }
}
