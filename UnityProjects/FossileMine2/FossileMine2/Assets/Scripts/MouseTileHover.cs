using UnityEngine;

public class MouseTileHover : MonoBehaviour
{
    public int X, Y;

    private void OnMouseEnter()
    {
        if (GameController.Instance.IsPaused)
            return;
        FossilField.Instance.MoveActiveTile(X, Y);
        MovementController.Instance.SetPosition(X, Y);
    }
}
