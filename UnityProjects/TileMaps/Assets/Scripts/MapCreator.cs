using System.Collections.Generic;
using UnityEngine;

public class MapCreator : Singleton<MapCreator>
{
    
    public Sprite[] DirtSprites;
    public float SpriteWidth = 0.32f;
    public Vector2 Size = new Vector2(20,20);
    public bool Centered = false;

    public Vector2 Offset
    {
        get
        {
            return Centered ? new Vector2(-(Size.x * SpriteWidth / 2), -(Size.y * SpriteWidth / 2)) : new Vector2(0,0);
        }
    }

    private readonly List<GameObject> _sprites = new List<GameObject>();

	void Start ()
    {
        for (int y = 0; y < Size.y; y++)
        {
            for (int x = 0; x < Size.x; x++)
            {
                CreateNewTile(DirtSprites[Random.Range(0, DirtSprites.Length)], x, y, Offset);
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
}
