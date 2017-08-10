using System.Linq;
using UnityEngine;

namespace Ziturion.BehaviourTree
{
    public class BT_Kill : BT_Primitive
    {
        public BT_Kill(string name) : base(name) { }

        public override BT_Callback CallbackState(BT_CallbackInfo info)
        {
            Tile[] possibleEnemyTiles = TerrainManager.Instance.AdjacentCultureTiles(info.CultureInfo).Where(t => t.IsOccupied).ToArray();
            if (info.CultureInfo.Name == "Red Culture")
                Debug.Log(possibleEnemyTiles.Length);

            if(possibleEnemyTiles.Length <= 0)
                return BT_Callback.False;

            Tile selectedTile = null;
            foreach (Tile enemyTile in possibleEnemyTiles)
            {
                if(enemyTile.OccupyingCulture.Variables.Reputation > info.CultureInfo.Variables.Reputation)
                    continue;
                selectedTile = enemyTile;
            }

            if(selectedTile == null)
                return BT_Callback.False;

            if (CultureManager.Instance.CultureAttacks(info.CultureInfo,selectedTile))
            {
                return BT_Callback.True;
            }

            return BT_Callback.False;
        }
    }
}
