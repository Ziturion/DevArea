using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Tile
{
    public Action OnTileClaimed;

    public TileType TileType;
    public TileResources Resources { get; private set; }
    public Culture OccupyingCulture { get; private set; }

    public int XPosition { get; private set; }
    public int YPosition { get; private set; }

    public bool IsOccupied { get { return OccupyingCulture != null; } }

    //called after creation
    public void Initialize(TileType type, int xPos, int yPos, TileResources resources)
    {
        TileType = type;
        XPosition = xPos;
        YPosition = yPos;
        SetResources(resources);
    }

    public void Initialize( TileType type, Vector2 position, TileResources resources)
    {
        Initialize(type,(int)position.x,(int)position.y,resources);
    }

    public bool Claim(Culture culture)
    {
        if (OccupyingCulture != null)
            return false;

        OccupyingCulture = culture;

        if (OnTileClaimed != null)
            OnTileClaimed.Invoke();
        return true;
    }

    public void ForceClaim(Culture culture)
    {
        OccupyingCulture = culture;

        if (OnTileClaimed != null)
            OnTileClaimed.Invoke();
    }

    /// <summary>
    /// Sets the resources to a value in a small random range so that not every tile is the same
    /// </summary>
    /// <param name="resources"></param>
    public void SetResources(TileResources resources)
    {
        resources.Water = Mathf.Clamp01(resources.Water + Random.Range(-0.1f, 0.1f));
        resources.Food = Mathf.Clamp01(resources.Food + Random.Range(-0.05f, 0.05f));
        resources.Production = Mathf.Clamp01(resources.Production + Random.Range(-0.2f, 0.2f));
        resources.Goods = Mathf.Clamp01(resources.Goods + Random.Range(-0.05f, 0.05f));

        Resources = resources;
    }

    ~Tile()
    {
        OnTileClaimed = null;
    }

    public override string ToString()
    {
        return string.Format("{0} ({1},{2})",TileType,XPosition,YPosition);
    }
}

public enum TileType
{
    Water,
    Plain,
    Forest,
    Mountain
}
