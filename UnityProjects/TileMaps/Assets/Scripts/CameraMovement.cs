using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Range(0f,1f)]
    public float ZoomMod = 0.4f;

    private Camera _camera;
    private Vector3 _origin;
    private Vector3 _difference;
    private bool _isDragging;

    void Awake()
    {
        _camera = Camera.main;
    }

    void LateUpdate()
    {
        _camera.orthographicSize =Mathf.Clamp(_camera.orthographicSize - Input.mouseScrollDelta.y * ZoomMod, 0.2f, 10f);

        if (Input.GetMouseButton(2))
        {
            _difference = (_camera.ScreenToWorldPoint(Input.mousePosition)) - _camera.transform.position;
            if (_isDragging == false)
            {
                _isDragging = true;
                _origin = _camera.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else
        {
            _isDragging = false;
        }

        if (_isDragging)
        {
            _camera.transform.position = (_origin - _difference);
        }

        //camera.transform.position += Vector3.forward * Input.mouseScrollDelta.y;
    }
}
