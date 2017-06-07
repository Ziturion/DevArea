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

    public Vector2 Offset
    {
        get
        {
            return Centered ? new Vector2(-(Size.x * SpriteWidth / 2), -(Size.y * SpriteWidth / 2)) : new Vector2(0,0);
        }
    }

    private readonly List<GameObject> _sprites = new List<GameObject>();
    private float[,] _noiseMap;


    void Awake()
    {
        _noiseMap = Noise.GenerateNoiseMap((int) Size.x, (int) Size.y, Seed, Scale, Octaves, Persistance, Lacunarity, new Vector2(0, 0));
    }

	void Start ()
    {

        DrawTexture(TextureGenerator.TextureFromHeightMap(_noiseMap));

        //Sprite[,] sprites = new Sprite[Size.x, Size.x];
        for (int y = 0; y < Size.y; y++)
        {
            for (int x = 0; x < Size.x; x++)
            {
                float currentHeight = _noiseMap[x, y];
                for (int i = 0; i < Regions.Length; i++)
                {
                    if (currentHeight <= Regions[i].Height)
                    {
                        CreateNewTile(Regions[i].Sprites[Random.Range(0, Regions[i].Sprites.Length)], x, y, Offset);
                        break;
                    }
                }
            }
        }
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
}

[System.Serializable]
public struct TerrainType
{
    public string Name;
    public float Height;
    public Sprite[] Sprites;
}
