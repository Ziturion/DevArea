using System.Collections.Generic;
using UnityEngine;

public class MapCreator : Singleton<MapCreator>
{
    public float SpriteWidth = 0.32f;
    public Vector2 Size = new Vector2(20,20);
    public bool Centered = false;

    public int Seed = 1337;
    public float Scale = 0.4f;
    public int Octaves = 4;
    public float Persistance = 0.5f;
    public float Lacunarity = 2f;

    public TerrainType[] Regions;

    public bool AutoUpdate;

    public Vector2 Offset
    {
        get
        {
            return Centered ? new Vector2(-(Size.x * SpriteWidth / 2), -(Size.y * SpriteWidth / 2)) : new Vector2(0,0);
        }
    }

    private readonly List<GameObject> _sprites = new List<GameObject>();
    private float[,] _noiseMap;

	void Start ()
    {
        //Generate();
    }

    void OnValidate()
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


    public void Generate()
    {
        ClearAllChildren();
        //DrawTexture(TextureGenerator.TextureFromHeightMap(_noiseMap));
        _noiseMap = Noise.GenerateNoiseMap((int)Size.x, (int)Size.y, Seed, Scale, Octaves, Persistance, Lacunarity, new Vector2(0, 0));

        Sprite[,] sprites = new Sprite[(int)Size.x, (int)Size.y];

        for (int y = 0; y < Size.y; y++)
        {
            for (int x = 0; x < Size.x; x++)
            {
                float currentHeight = _noiseMap[x, y];
                for (int i = 0; i < Regions.Length; i++)
                {
                    if (currentHeight <= Regions[i].Height)
                    {
                        sprites[x, y] = Regions[i].Sprites[Random.Range(0, Regions[i].Sprites.Length)];
                        //CreateNewTile(Regions[i].Sprites[Random.Range(0, Regions[i].Sprites.Length)], x, y, Offset);
                        break;
                    }
                }
            }
        }

        DrawTexture(TextureGenerator.TextureFromSprites(sprites, (int)Size.x, (int)Size.y));
    }

    public void CreateNewTile(Sprite sprite, int xCoord, int yCoord, float xOffset  = 0, float yOffset = 0)
    {
        GameObject newTile = new GameObject("Tile_" + xCoord + "_" + yCoord, typeof(SpriteRenderer));
        newTile.transform.SetParent(transform);
        newTile.transform.position = Vector3.right * xCoord * SpriteWidth + Vector3.down * yCoord * SpriteWidth;
        newTile.transform.position += Vector3.right * xOffset + Vector3.down * yOffset;
        _sprites.Add(newTile);

        SpriteRenderer sRenderer = newTile.GetComponent<SpriteRenderer>();
        sRenderer.sprite = sprite;
        sRenderer.flipX = System.Convert.ToBoolean(UnityEngine.Random.Range(0, 2));
        sRenderer.flipY = System.Convert.ToBoolean(UnityEngine.Random.Range(0, 2));
    }

    public void CreateNewTile(Sprite sprite, int xCoord, int yCoord, Vector2 offset)
    {
        CreateNewTile(sprite, xCoord, yCoord, offset.x, offset.y);
    }

    public void DrawTexture(Texture2D texture)
    {
        Renderer textureRender = GetComponent<Renderer>();
        textureRender.sharedMaterial.mainTexture = texture;
        //textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void ClearAllChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        _sprites.Clear();
    }
}

[System.Serializable]
public struct TerrainType
{
    public string Name;
    public float Height;
    public Sprite[] Sprites;
}
