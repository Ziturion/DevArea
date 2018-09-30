using UnityEngine;

public class FossilField : Singleton<FossilField>
{
    public TileDatabase FossilTileDatabase;
    public GroundTile this[int x, int y] => _tileField[x, y];
    public GameObject ActiveIndicatorPrefab;
    public int Width = 15, Height = 15;
    public float Length = .5f;
    public Vector2 StartActiveTile;

    private GameObject[,] _tileFieldObject;
    private GroundTile[,] _tileField;
    private GameObject _indicator;
    private int _width, _height;
    private float _length;

    private void Start()
    {
        CreateField(Width, Height, Length);
        _indicator = Instantiate(ActiveIndicatorPrefab, transform);
        MoveActiveTile(StartActiveTile);

#if UNITY_EDITOR
        CustomInputController.Instance.IgnoreAllInput = false;
#endif
    }

    public bool ClickOnTile(int x, int y, Tool tool)
    {
        GroundTile tile = _tileField[x, y];
        HitTile(tile, tool.Strength);

        foreach (Vector2 otherTilePos in tool.OtherTiles) //Hits other Tiles
        {
            int xPos = Mathf.RoundToInt(otherTilePos.x + x);
            int yPos = Mathf.RoundToInt(otherTilePos.y + y);
            if (xPos < 0 || xPos >= _width || yPos < 0 || yPos >= _height)
                continue;
            GroundTile otherTile = _tileField[xPos, yPos];
            HitTile(otherTile, tool.Strength);
        }

        return tile == null;
    }

    private static void HitTile(GroundTile tile, int strength)
    {
        if (tile == null)
            return;
        tile.Hardness -= strength;

        tile.UpdateTile(); //Update Tile = Check for Destroy
    }

    public bool MoveActiveTile(Vector2 pos)
    {
        return MoveActiveTile(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
    }

    public bool MoveActiveTile(int x, int y)
    {
        if (x < 0 || x >= _width || y < 0 || y >= _height)
            return false;
        Vector2 position = ConvertPosition(x, y);
        _indicator.transform.position = new Vector3(position.x, position.y);
        return true;
    }

    public void DestroyTile(int x, int y)
    {
        Destroy(_tileFieldObject[x,y]);
        _tileField[x, y] = null;
    }

    private void CreateField(int width, int height, float length)
    {
        _tileField = new GroundTile[width, height];
        _tileFieldObject = new GameObject[width, height];

        _width = width;
        _height = height;
        _length = length;

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                CreateTile(x, y);
            }
        }
    }

    private void CreateTile(int x, int y)
    {
        _tileField[x, y] = new GroundTile(FossilTileDatabase.GroundTiles[0]); //TODO
        _tileField[x, y].OnBroken += () => DestroyTile(x, y);

        GameObject go = new GameObject($"GroundTile ({x}-{y})");
        go.transform.SetParent(transform);
        go.AddComponent<SpriteRenderer>().sprite = _tileField[x, y].TileSprite;
        MouseTileHover hover = go.AddComponent<MouseTileHover>();
        hover.X = x;
        hover.Y = y;
        go.AddComponent<BoxCollider2D>();

        Vector2 position = ConvertPosition(x, y);

        go.transform.position = new Vector3(position.x, position.y);

        _tileFieldObject[x, y] = go;
    }

    private Vector2 ConvertPosition(int x, int y)
    {
        return new Vector2((_height / 2f + x - _height) * _length, (_width / 2f + y - _width) * _length);
    }
}
