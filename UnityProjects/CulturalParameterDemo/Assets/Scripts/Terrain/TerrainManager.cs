using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainManager : Singleton<TerrainManager>
{
    public World World { get; private set; }
    public TileObject[] WorldObjects { get; private set; }

    protected void Awake()
    {
        TimeManager.OnStartSimulationInit += GetWorldSetup;
    }

    public void UpdateVisuals(Tile tile)
    {
        TileObject obj = GetTileObject(tile);
        if(tile.OccupyingCulture != null)
            obj.UpdateHighlight(true,tile.OccupyingCulture.CultureColor);

    }

    /// <summary>
    /// Throws an Exception if there is no Object with this position
    /// </summary>
    /// <param name="xPos"></param>
    /// <param name="yPos"></param>
    /// <returns>returns the object on the related Position.</returns>
    public TileObject GetTileObject(int xPos, int yPos)
    {
        return WorldObjects.Single(t => t.XPos == xPos && t.YPos == yPos);
    }

    public TileObject GetTileObject(Vector2 position)
    {
        return GetTileObject((int)position.x, (int)position.y);
    }

    public TileObject GetTileObject(Tile tile)
    {
        return GetTileObject(tile.XPosition, tile.YPosition);
    }

    public Tile TileByObject(int xPos, int yPos)
    {
        return World[xPos, yPos];
    }

    public Tile TileByObject(Vector2 position)
    {
        return TileByObject((int)position.x, (int)position.y);
    }

    public Tile TileByObject(TileObject objectTile)
    {
        return TileByObject(objectTile.XPos, objectTile.YPos);
    }

    /// <summary>
    /// searches for all tiles that are occupied by given culture
    /// </summary>
    /// <param name="culture"></param>
    /// <returns>return tile Array with all tiles from this culture</returns>
    public Tile[] GetTerritory(Culture culture)
    {
        List<Tile> tiles = new List<Tile>();

        for (int x = 0; x < World.Map.GetLength(0); x++)
        {
            for (int y = 0; y < World.Map.GetLength(1); y++)
            {
                if (World[x, y].IsOccupied && World[x,y].OccupyingCulture == culture)
                {
                    tiles.Add(World[x,y]);
                }
            }
        }
        return tiles.ToArray();
    }

    public Tile[] AdjacentTiles(Tile tile)
    {
        List<Tile> tiles = new List<Tile>();

        if (tile.XPosition > 0)
        {
            tiles.Add(TileByObject(tile.XPosition - 1,tile.YPosition));
        }
        if (tile.XPosition < World.Size.x - 1)
        {
            tiles.Add(TileByObject(tile.XPosition + 1, tile.YPosition));
        }
        if (tile.YPosition > 0)
        {
            tiles.Add(TileByObject(tile.XPosition, tile.YPosition - 1));
        }
        if (tile.YPosition < World.Size.y - 1)
        {
            tiles.Add(TileByObject(tile.XPosition, tile.YPosition + 1));
        }

        return tiles.ToArray();
    }

    public Tile[] AdjacentCultureTiles(Culture culture)
    {
        Tile[] territory = GetTerritory(culture);
        List<Tile> adjacentTiles = new List<Tile>();

        //adding all neighbours for all
        foreach (Tile tile in territory)
        {
            adjacentTiles.AddRange(AdjacentTiles(tile).Where(t => t.OccupyingCulture != culture));
        }

        return adjacentTiles.ToArray();
    }

    /// <summary>
    /// gets the world data and prefab reference from the generator
    /// </summary>
    private void GetWorldSetup()
    {
        World = TerrainGenerator.Instance.TerrainWorld;
        WorldObjects = TerrainGenerator.Instance.TileObjects.ToArray();

        foreach (Tile tile in World.Map)
        {
            tile.OnTileClaimed += () => UpdateVisuals(tile);
        }
    }

}
