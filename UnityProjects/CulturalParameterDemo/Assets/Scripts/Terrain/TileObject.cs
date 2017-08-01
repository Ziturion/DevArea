using UnityEngine;

public class TileObject : MonoBehaviour
{
    public SpriteRenderer MainSprite;
    public SpriteRenderer HighlightSprite;

    public int XPos, YPos;

    public void Initialize(Sprite mainSprite, int xPosition, int yPosition, Color highlightColor, bool occupied = false)
    {
        MainSprite.sprite = mainSprite;
        HighlightSprite.enabled = occupied;
        HighlightSprite.color = highlightColor;
        XPos = xPosition;
        YPos = yPosition;
    }

    public void Initialize(Sprite mainSprite, int xPosition, int yPosition, bool occupied = false)
    {
        Initialize(mainSprite,xPosition,yPosition,new Color(0,0,0,0),occupied);
    }

    public void UpdateHighlight(bool occupied, Color color)
    {
        HighlightSprite.enabled = occupied;
        HighlightSprite.color = color;
    }
}
