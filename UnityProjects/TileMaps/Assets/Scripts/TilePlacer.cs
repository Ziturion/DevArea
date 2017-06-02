using UnityEngine;

public class TilePlacer : MonoBehaviour
{

    public Sprite PlaceSprite;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            MapCreator.Instance.CreateNewTile(PlaceSprite,
                Mathf.RoundToInt(pos.x / MapCreator.Instance.SpriteWidth), 
                -Mathf.RoundToInt(pos.y / MapCreator.Instance.SpriteWidth));
        }
	}
}
