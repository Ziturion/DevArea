using System;
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

            return CultureManager.Instance.ClaimTile(selectedTile, info.CultureInfo) ? BT_Callback.True : BT_Callback.False;
        }

        private Tile SelectTile(Culture culture)
        {
            Tile[] possibleTiles = TerrainManager.Instance.AdjacentCultureTiles(culture);
            Tile selectedTile;

            if (culture.Variables.PopulationSize < culture.Variables.TerritorySize)
                return null;

            switch (_cultureType)
            {
                case CultureType.Behaviour:
                    //Debug.Log("CL:Beh"); //Logic for Behavioural Claiming TODO
                    selectedTile = possibleTiles[Random.Range(0, possibleTiles.Length)];
                    break;
                case CultureType.Communication:
                    //Debug.Log("CL:Com"); //Logic for Communicative Claiming TODO
                    selectedTile = possibleTiles[Random.Range(0, possibleTiles.Length)];
                    break;
                case CultureType.Economics:
                    //Debug.Log("CL:Eco"); //Logic for Economic Claiming TODO
                    selectedTile = possibleTiles[Random.Range(0, possibleTiles.Length)];
                    break;
                case CultureType.Sociology:
                    throw new NotSupportedException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (culture.Name == "Red Culture")
                Debug.Log(Name);
            return selectedTile;
        }
    }
}
