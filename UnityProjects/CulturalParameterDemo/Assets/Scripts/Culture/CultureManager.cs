using UnityEngine;

public class CultureManager : Singleton<CultureManager>
{
    public Culture[] Cultures;
    public CultureUI[] CultureUis;

    protected void Awake()
    {
        TimeManager.OnStartSimulationLate += GetWorldSetup;
    }

    protected void FixedUpdate()
    {
        if (!TimeManager.Started)
            return;

        //Debugging
        if (Input.GetKey(KeyCode.A))
        {
            ClaimTile(Random.Range(0, 50), Random.Range(0, 50), Cultures[0]);
        }

        foreach (Culture culture in Cultures)
        {
            culture.Update();
        }
    }

    public void ClaimTile(int xPos, int yPos, Culture occupyingCulture)
    {
        TerrainManager.Instance.World[xPos, yPos].Claim(occupyingCulture);
    }

    public void ClaimTile(Tile tile, Culture occupyingCulture)
    {
        ClaimTile(tile.XPosition, tile.YPosition, occupyingCulture);
    }

    public string GetCultureName(int index)
    {
        return Cultures[index].Name;
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

    private void ChangeCultureParameter(int cultureIndex, string parameterName, float parameterValue)
    {
        Cultures[cultureIndex].ChangeParameterValue(parameterName,parameterValue);
    }
}
