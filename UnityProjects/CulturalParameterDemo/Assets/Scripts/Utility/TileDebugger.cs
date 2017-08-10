using UnityEngine;
using UnityEngine.UI;

public class TileDebugger : MonoBehaviour
{
    public Text Water, Food, Production, Goods;

	protected void FixedUpdate()
    {
        Ray cRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        //if the mouse hit something
        if (Physics.Raycast(cRay, out hit))
        {
            TileObject tileObject = hit.transform.gameObject.GetComponent<TileObject>();
            //if the object is a tile
            if (tileObject != null)
            {
                Tile tile = TerrainManager.Instance.TileByObject(tileObject);
                SetTexts(tile.Resources);
            }
        }
    }

    private void SetTexts(TileResources resources)
    {
        Water.text = "Water: " + string.Format("{0:00%}",resources.Water);
        Food.text = "Food: " + string.Format("{0:00%}", resources.Food);
        Production.text = "Production: " + string.Format("{0:00%}", resources.Production);
        Goods.text = "Goods: " + string.Format("{0:00%}", resources.Goods);
    }
}
