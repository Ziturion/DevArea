[System.Serializable]
public class World
{
    public Tile[,] Map;

    public World(float[,] noiseMap)
    {
        int xLength = noiseMap.GetLength(0), yLength = noiseMap.GetLength(1);

        Map = new Tile[xLength, yLength];

        for (int x = 0; x < xLength; x++)
        {
            for (int y = 0; y < yLength; y++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < TerrainGenerator.Instance.Regions.Length; i++)
                {
                    if (currentHeight <= TerrainGenerator.Instance.Regions[i].Height)
                    {
                        Map[x, y] = new Tile();
                        Map[x, y].Initialize((TileType)i,x,y,TerrainGenerator.Instance.Regions[i].StandardResources);
                        break;
                    }
                }


            }
        }
    }

    public Tile this[int x,int y]
    {
        get { return Map[x, y]; }
        //maybe setter should be private
        set { Map[x, y] = value; }
    }
}
