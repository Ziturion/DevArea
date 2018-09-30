using UnityEngine;

public class CustomInputController : Singleton<CustomInputController>
{
    public bool IgnoreAllInput;
    public float InputCooldown = 0.135f;
    private float _cooldown;

    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);
    }

    protected virtual void Update ()
	{
	    if (IgnoreAllInput)
	        return;

	    if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && GameController.Instance.IsInMiniGame &&
	        !GameController.Instance.IsPaused)
	        MovementController.Instance.HitActive();

	    _cooldown -= Time.deltaTime;
	    if (_cooldown > 0)
	        return;

	    if (Input.anyKey)
	        _cooldown = InputCooldown;

	    if (Input.GetKey(KeyCode.Escape))
	    {
	        GameController.Instance.PauseGame(true);
	    }

	    if (!GameController.Instance.IsInMiniGame || GameController.Instance.IsPaused) //Disable Mini Game Movement
	        return;

	    if (Input.GetKey(KeyCode.W))
	    {
	        MovementController.Instance.Move(MovementController.Direction.Up);
	    }

	    if (Input.GetKey(KeyCode.A))
	    {
	        MovementController.Instance.Move(MovementController.Direction.Left);
        }

	    if (Input.GetKey(KeyCode.S))
	    {
	        MovementController.Instance.Move(MovementController.Direction.Down);
        }

	    if (Input.GetKey(KeyCode.D))
	    {
	        MovementController.Instance.Move(MovementController.Direction.Right);
	    }

	    if (Input.GetKey(KeyCode.Alpha1))
	    {
	        FossilePlayer.Instance.ChangeTool(0);
	    }

	    if (Input.GetKey(KeyCode.Alpha2))
	    {
	        FossilePlayer.Instance.ChangeTool(1);
        }

	    if (Input.GetKey(KeyCode.Alpha3))
	    {
	        FossilePlayer.Instance.ChangeTool(2);
        }
    }
}
