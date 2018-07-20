using UnityEngine;

namespace StaticAlgorithms
{
    public static class LevelGenerator
    {

        public static int Length { get; private set; }
        public static int Width { get; private set; }

        private static Sprite[] _stoneSprites;

        public static Sprite[,] GenerateLevel(string stoneSpriteSheetName, int length, int width)
        {
            if (_stoneSprites == null)
                LoadGeneratingVariables(stoneSpriteSheetName, length, width);

            Sprite[,] levelField = new Sprite[Length, Width];

            for (int x = 0; x < Length; x++)
            {
                for (int y = 0; y < Width; y++)
                {
                    levelField[x, y] = _stoneSprites[Random.Range(0, _stoneSprites.Length)];
                }
            }

            return levelField;
        }

        private static void LoadGeneratingVariables(string stoneSpriteSheetName, int length, int width)
        {
            Length = length;
            Width = width;
            _stoneSprites = Resources.LoadAll<Sprite>(stoneSpriteSheetName);
            if (_stoneSprites == null)
                Debug.LogError("Stone Sprite Sheet was not found in the Resources Folder");
        }
    }
}
