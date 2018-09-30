using System;
using UnityEngine;

public class MovementController : Singleton<MovementController>
{
    private Vector2 _activePosition = new Vector2(0,0);

    public void SetPosition(int x, int y)
    {
        _activePosition = new Vector2(x, y);
    }

    public void HitActive()
    {
        FossilField.Instance.ClickOnTile(Mathf.RoundToInt(_activePosition.x), Mathf.RoundToInt(_activePosition.y), FossilePlayer.Instance.ActiveTool);
    }

    public void Move(Direction direction, int amount = 1)
    {
        switch (direction)
        {
            case Direction.Up:
                if (FossilField.Instance.MoveActiveTile(new Vector2(_activePosition.x, _activePosition.y + 1)))
                    _activePosition.y++;
                break;
            case Direction.Left:
                if (FossilField.Instance.MoveActiveTile(new Vector2(_activePosition.x - 1, _activePosition.y)))
                    _activePosition.x--;
                break;
            case Direction.Down:
                if (FossilField.Instance.MoveActiveTile(new Vector2(_activePosition.x, _activePosition.y - 1)))
                    _activePosition.y--;
                break;
            case Direction.Right:
                if (FossilField.Instance.MoveActiveTile(new Vector2(_activePosition.x + 1, _activePosition.y)))
                    _activePosition.x++;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    public enum Direction
    {
        Up,Left,Down,Right
    }
}
