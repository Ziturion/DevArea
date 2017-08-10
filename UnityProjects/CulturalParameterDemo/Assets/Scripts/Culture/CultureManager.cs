using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CultureManager : Singleton<CultureManager>
{
    public Culture[] Cultures;
    public CultureUI[] CultureUis;
    public static float MaxPopulationPerTile = 15;
    public static float MaxPopulationPerWater = 100;
    public static float MaxPopulationPerFood = 125;
    public static float MaxPopulationPerProduction = 200;
    public static float MaxPopulationPerGoods = 250;

    protected void Awake()
    {
        TimeManager.OnStartSimulationInit += GetWorldSetup;
        TimeManager.OnStartSimulationPostInit += ClaimStartLocations;
    }

    protected void Start()
    {
        InvokeRepeating("CultureUpdate", 0, 0.25f);
    }

    protected void FixedUpdate()
    {
        if (!TimeManager.Started)
            return;

        //Debugging
        if (Input.GetKey(KeyCode.Alpha1))
        {
            ClaimTile(Random.Range(0, 50), Random.Range(0, 50), Cultures[0]);
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            ClaimTile(Random.Range(0, 50), Random.Range(0, 50), Cultures[1]);
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            ClaimTile(Random.Range(0, 50), Random.Range(0, 50), Cultures[2]);
        }
    }

    private void CultureUpdate() //is used in Start (coroutine
    {
        foreach (Culture culture in Cultures)
        {
            if(Random.Range(0f,1f) < 0.25f)
                culture.Update();
        }
    }

    public bool ClaimTile(int xPos, int yPos, Culture claimingCulture)
    {
        return TerrainManager.Instance.World[xPos, yPos].Claim(claimingCulture);
    }

    public bool ClaimTile(Tile tile, Culture claimingCulture)
    {
        return ClaimTile(tile.XPosition, tile.YPosition, claimingCulture);
    }

    public bool ClaimTile(Vector2 position, Culture claimingCulture)
    {
        return ClaimTile((int)position.x, (int)position.y, claimingCulture);
    }

    public string GetCultureName(int index)
    {
        return Cultures[index].Name;
    }

    public bool CultureAttacks(Culture attackingCulture, Tile defendingTile)
    {

        Culture defender = defendingTile.OccupyingCulture;
        float attackingBehaviourValue = attackingCulture.GetParameterValue("Behaviour");

        //if the reputation of defender is too high (but the attacker has a small chance to attack nevertheless
        if (defender.Variables.Reputation > attackingCulture.Variables.Reputation && (Random.Range(0f, 1f) < 1.8f - attackingBehaviourValue))
            return false;

        float attackArmy = attackingCulture.Variables.PopulationSize * attackingBehaviourValue;
        float defendArmy = defender.Variables.PopulationSize * defender.GetParameterValue("Behaviour");


        int result = CalculateWin(attackArmy,defendArmy, attackingCulture.Variables.EscalationRate,Mathf.RoundToInt(attackingBehaviourValue*  8)/*TODO Luck value*/);
        float attackingPercentageLoss;
        float defendingPercantageLoss;
        switch (result)
        {
            case 1:
                //CritWin
                ForcedClaimTile(defendingTile, attackingCulture);
                attackingPercentageLoss = Random.Range(0.02f, 0.05f);
                defendingPercantageLoss = Random.Range(0.1f, 0.14f);
                break;
            case 2:
                //Win
                ForcedClaimTile(defendingTile, attackingCulture);
                attackingPercentageLoss = Random.Range(0.04f, 0.1f);
                defendingPercantageLoss = Random.Range(0.08f, 0.12f);
                break;
            case 3:
                //Lose
                attackingPercentageLoss = Random.Range(0.06f, 0.15f);
                defendingPercantageLoss = Random.Range(0.06f, 0.1f);
                break;
            case 4:
                //CritLose
                attackingPercentageLoss = Random.Range(0.2f, 0.25f);
                defendingPercantageLoss = Random.Range(0.04f, 0.08f);
                break;
            default:
                throw new ArgumentOutOfRangeException("the calculated WinValue is out of bounds: " + result);
        }
        attackingCulture.AddLosses(attackingPercentageLoss);
        defender.AddLosses(defendingPercantageLoss);

        return true;
    }

    private static void ForcedClaimTile(Tile tile, Culture claimingCulture)
    {
        TerrainManager.Instance.World[tile.XPosition, tile.YPosition].ForceClaim(claimingCulture);
    }

    private static int CalculateWin(float attackArmy, float defendArmy, float escalationRate, int luck = 4)
    {
        int result = 4;

        if (attackArmy > defendArmy)
            result--;

        if (escalationRate > 0.5f)
            result--;

        for (int i = 0; i < luck; i++)
        {
            if (Random.Range(0f, 1f) > 0.4f)
                result--;
        }
        result = Mathf.Clamp(result, 1, 4);
        return result;
    }

    private void GetWorldSetup()
    {
        Cultures = CultureGenerator.Instance.AllCultures.ToArray();
        CultureUis = CultureGenerator.Instance.AllCultureUIs.ToArray();

        for (int i = 0; i < CultureUis.Length; i++)
        {
            int index = i;
            CultureUis[i].OnParameterValueChanged += (t, u) => ChangeCultureParameter(index, t, u);
            Cultures[i].OnPopulationEnd += KillCulture;
            Cultures[i].OnTerritoryEnd += KillCulture;
        }

    }

    private void KillCulture(Culture culture)
    {
        Tile[] territory = TerrainManager.Instance.GetTerritory(culture).ToArray();
        foreach (Tile tile in territory)
        {
            ForcedClaimTile(tile,null);
        }

        Debug.Log("Killed: " + culture);

        List<Culture> newCultures = new List<Culture>();
        newCultures.AddRange(Cultures);

        CultureUis[newCultures.IndexOf(culture)].SetInactive();

        newCultures.Remove(culture);
        Cultures = newCultures.ToArray();
    }

    private void ClaimStartLocations()
    {
        //claim startposition
        foreach (Culture culture in Cultures)
        {
            ClaimTile(culture.StartPosition, culture);
        }
    }

    private void ChangeCultureParameter(int cultureIndex, string parameterName, float parameterValue)
    {
        Cultures[cultureIndex].ChangeParameterValue(parameterName,parameterValue);
    }
}
