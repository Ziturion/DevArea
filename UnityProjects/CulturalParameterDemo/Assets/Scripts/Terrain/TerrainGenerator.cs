using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TerrainGenerator : Singleton<TerrainGenerator>
{
    public TileObject TilePrefab;
    public float SpriteWidth = 0.32f;
    public bool Centered = false;

    public Vector2 Size = new Vector2(20, 20);
    public int Seed = 17358254;
    public float Scale = 0.4f;
    public int Octaves = 4;
    public float Persistance = 0.5f;
    public float Lacunarity = 2f;

    public TerrainType[] Regions;

    public World TerrainWorld;

    public Vector2 Offset
    {
        get
        {
            return Centered ? new Vector2(-(Size.x * SpriteWidth / 2), -(Size.y * SpriteWidth / 2)) : new Vector2(0, 0);
        }
    }

    public List<TileObject> TileObjects = new List<TileObject>();
    private float[,] _noiseMap;

    protected void Awake()
	{
	    TimeManager.OnStartSimulation += GenerateTerrain;
	}

    //Error Management
    protected void OnValidate()
    {
        if (Size.x < 1)
        {
            Size.x = 1;
        }
        if (Size.y < 1)
        {
            Size.y = 1;
        }
        if (Lacunarity < 1)
        {
            Lacunarity = 1;
        }
        if (Octaves < 0)
        {
            Octaves = 0;
        }
    }

    private void GenerateTerrain()
    {
        Debug.Log("Generating Terrain...");

        _noiseMap = Noise.GenerateNoiseMap((int)Size.x, (int)Size.y, Seed, Scale, Octaves, Persistance, Lacunarity, new Vector2(0, 0));
        TerrainWorld = new World(_noiseMap);
        CreateGameObjectMap(_noiseMap);

        Debug.Log("Terrain Generated.");
    }

    private void CreateGameObjectMap( float[,] noiseMap)
    {
        for (int x = 0; x < Size.x; x++)
        {
            for (int y = 0; y < Size.y; y++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < Regions.Length; i++)
                {
                    if (currentHeight <= Regions[i].Height)
                    {
                        CreateNewGameobject(i, x, y, Offset);
                        break;
                    }
                }
            }
        }
    }

    private void CreateNewGameobject(int regionindex, int xCoord, int yCoord, float xOffset = 0, float yOffset = 0)
    {
        TileObject tileObj = Instantiate(TilePrefab);
        tileObj.transform.SetParent(transform);
        tileObj.transform.position = Vector3.right * xCoord * SpriteWidth + Vector3.down * yCoord * SpriteWidth;
        tileObj.transform.position += Vector3.right * xOffset + Vector3.down * yOffset;
        tileObj.name = string.Format("{0}/{1} (Region: {2})",xCoord,yCoord,regionindex);

        TileObjects.Add(tileObj);

        tileObj.Initialize(GetSprite(regionindex),xCoord,yCoord);
    }

    private void CreateNewGameobject(int regionindex, int xCoord, int yCoord, Vector2 offset)
    {
        CreateNewGameobject(regionindex, xCoord, yCoord, offset.x, offset.y);
    }

    /// <summary>
    /// Gets the Sprite from the Region Index Number
    /// </summary>
    /// <param name="regionIndex"></param>
    /// <returns></returns>
    private Sprite GetSprite(int regionIndex)
    {
        return Regions[regionIndex].Sprites[Random.Range(0, Regions[regionIndex].Sprites.Length)];
    }
}

[System.Serializable]
public struct TerrainType
{
    public string Name;
    public float Height;
    public Sprite[] Sprites;
    public TileResources StandardResources;
}

[System.Serializable]
public struct TileResources
{
    public float Water;
    public float Food;
    public float Production;
    public float Goods;
}
