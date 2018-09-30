using UnityEngine;

[CreateAssetMenu(fileName = "tileDatabase", menuName = "Fossil/tileDatabase", order = 1)]
public class TileDatabase : ScriptableObject
{
    public GroundTile[] GroundTiles;
}
