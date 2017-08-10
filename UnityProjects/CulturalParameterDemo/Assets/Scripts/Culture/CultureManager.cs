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

    private void CultureUpdate()
    {
        //TODO asynchronous Update
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
        //TODO Attacking Logic
        return true;
    }

    private void GetWorldSetup()
    {
        Cultures = CultureGenerator.Instance.AllCultures.ToArray();
        CultureUis = CultureGenerator.Instance.AllCultureUIs.ToArray();

        for (int i = 0; i < CultureUis.Length; i++)
        {
            int index = i;
            CultureUis[i].OnParameterValueChanged += (t, u) => ChangeCultureParameter(index, t, u);
        }

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
