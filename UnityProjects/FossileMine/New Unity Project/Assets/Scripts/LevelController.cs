using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using StaticAlgorithms;

public class LevelController : MonoBehaviour
{
    public Vector2Int LevelBoundary = new Vector2Int(10, 10);
    private Sprite[,] _levelSprites;
    private LevelItem[] _levelItems;

    void Awake()
    {
        _levelSprites = LevelGenerator.GenerateLevel("StoneSpriteSheet/Stones", LevelBoundary.x, LevelBoundary.y);
        _levelItems = new LevelItem[LevelBoundary.x * LevelBoundary.y];
    }

    void Start()
    {
        for (int x = 0; x < LevelBoundary.x; x++)
        {
            for (int y = 0; y < LevelBoundary.y; y++)
            {
                _levelItems[LevelBoundary.x * y + x] = GetLevelItem(_levelSprites[x, y], x, y);
            }
        }
    }

    private LevelItem GetLevelItem(Sprite sprite, int xPos, int yPos)
    {
        Vector2Int position = new Vector2Int(xPos, yPos);
        GameObject go = new GameObject("LevelItemSprite");
        go.transform.SetParent(transform);
        go.transform.position = new Vector3((LevelBoundary.x / 2f) - xPos, (LevelBoundary.y / 2f) - yPos);
        go.AddComponent<SpriteRenderer>().sprite = sprite;

        LevelItem item = new LevelItem(position, go);
        return item;
    }

    public LevelItem GetNeighbour(LevelItem rootItem, LevelDirection dir)
    {
        switch (dir)
        {
            case LevelDirection.Up:
                return _levelItems.FirstOrDefault(i => i.Position.y - 1 == rootItem.Position.y);
            case LevelDirection.Down:
                return _levelItems.FirstOrDefault(i => i.Position.y + 1 == rootItem.Position.y);
            case LevelDirection.Left:
                return _levelItems.FirstOrDefault(i => i.Position.x - 1 == rootItem.Position.x);
            case LevelDirection.Right:
                return _levelItems.FirstOrDefault(i => i.Position.x + 1 == rootItem.Position.x);
            default:
                throw new ArgumentOutOfRangeException("dir", dir, null);
        }
    }

    public LevelItem[] GetAllNeighbours(LevelItem rootItem)
    {
        List<LevelItem> items = new List<LevelItem>();

        //Direction as int
        for (int i = 0; i < 4; i++)
        {
            LevelItem item = GetNeighbour(rootItem, (LevelDirection) i);
            if(item.GameObjetItem != null)
                items.Add(item);
        }

        return items.ToArray();
    }

    [Serializable]
    public struct LevelItem
    {
        public Vector2Int Position;
        public GameObject GameObjetItem;

        public LevelItem(Vector2Int position, GameObject gameObjetItem)
        {
            Position = position;
            GameObjetItem = gameObjetItem;
        }
    }
}

public enum LevelDirection
{
    Up,
    Down,
    Left,
    Right
}