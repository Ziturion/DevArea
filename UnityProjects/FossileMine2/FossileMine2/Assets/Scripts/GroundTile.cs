using System;
using UnityEngine;

[System.Serializable]
public class GroundTile
{
    public string TileName;
    public Sprite TileSprite;
    public int Hardness;

    public event Action OnBroken;

    public void UpdateTile()
    {
        if(Hardness <= 0)
            OnBroken?.Invoke();
    }

    public GroundTile(GroundTile tile)
    {
        TileName = tile.TileName;
        TileSprite = tile.TileSprite;
        Hardness = tile.Hardness;
    }
}
