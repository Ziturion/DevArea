using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Ziturion.BehaviourTree
{
    public class BT_Claim : BT_Primitive
    {
        private readonly CultureType _cultureType;
        public BT_Claim(string name, CultureType cultureType) : base(name)
        {
            _cultureType = cultureType;
        }

        public override BT_Callback CallbackState(BT_CallbackInfo info)
        {
            Tile selectedTile = SelectTile(info.CultureInfo);
            if(selectedTile == null)
                return BT_Callback.False;

            if (Random.Range(0f, 1f) < 0.5f)
                return BT_Callback.False; //TODO maybe Redo cause unnecessary Randomness

            return CultureManager.Instance.ClaimTile(selectedTile, info.CultureInfo) ? BT_Callback.True : BT_Callback.False;
        }

        private Tile SelectTile(Culture culture)
        {
            Tile[] possibleTiles = TerrainManager.Instance.AdjacentCultureTiles(culture);
            Tile selectedTile;

            if (culture.Variables.PopulationSize < culture.Variables.TerritorySize)
                return null;

            if (possibleTiles.Length <= 0)
                return null;

            switch (_cultureType)
            {
                case CultureType.Behaviour:
                    if (Random.Range(0f, 1f) > 0.5f)
                    {
                        selectedTile = possibleTiles.OrderByDescending(t => t.Resources.Water + t.Resources.Food).First();
                    }
                    else
                    {
                        //TODO Claim the Tile to nearest Enemy
                        selectedTile = possibleTiles[Random.Range(0, possibleTiles.Length)];
                    }

                    break;
                case CultureType.Communication:
                    //Debug.Log("CL:Com"); //Logic for Communicative Claiming TODO Claim the Tile to nearest with highest Reputation
                    selectedTile = possibleTiles[Random.Range(0, possibleTiles.Length)];
                    break;
                case CultureType.Economics:
                    selectedTile = possibleTiles.OrderByDescending(t => t.Resources.Production + t.Resources.Goods).First();
                    break;
                case CultureType.Sociology:
                    throw new NotSupportedException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
            //if (culture.Name == "Red Culture")
            //    Debug.Log(Name);
            return selectedTile;
        }
    }
}
