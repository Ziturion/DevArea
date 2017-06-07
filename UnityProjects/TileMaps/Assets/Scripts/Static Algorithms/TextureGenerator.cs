using UnityEngine;
using System.Linq;
using System.Linq.Expressions;

public static class TextureGenerator
{

    public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colourMap);
        texture.Apply();
        return texture;
    }


    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }

        return TextureFromColourMap(colourMap, width, height);
    }

    public static Texture2D TextureFromSprites(Sprite[,] sprites, int width, int height, int pMaxWidth = 32, int pMaxHeight = 32)
    {
        Color[] colourMap = new Color[width  * pMaxWidth * height * pMaxHeight];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int pixelHeight = 0; pixelHeight < pMaxHeight; pixelHeight++)
                {
                    for (int pixelWidth = 0; pixelWidth < pMaxWidth; pixelWidth++)
                    {
                        int xPos = (int)sprites[x, y].textureRect.position.x, yPos = (int)sprites[x, y].textureRect.position.y;

                        colourMap[((y  * pMaxHeight + pixelHeight) * width + x) * pMaxWidth + pixelWidth] = 
                        sprites[x, y].texture.GetPixel(pixelWidth + xPos, pixelHeight + yPos);
                    }

                }
            }
        }
        
        return TextureFromColourMap(colourMap, width * pMaxWidth, height * pMaxHeight);
    }

}