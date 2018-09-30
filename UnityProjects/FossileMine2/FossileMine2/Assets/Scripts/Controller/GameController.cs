using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : Singleton<GameController>
{
    public string MainGameName = "MainGame";
    public string MiniFossilGameName = "MiniFossilGame";

    public GameObject PauseMenuPrefab;

    public static event Action OnGameStart;
    public static event Action OnGameStarted;

    public static event Action OnMiniFossilGameStart;
    public static event Action OnMiniFossilGameStarted;

    public bool IsInMiniGame => SceneManager.GetActiveScene().name == MiniFossilGameName;
    public bool IsPaused { get; private set; }

    private GameObject _pauseMenu;

    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);
        CustomInputController.Instance.IgnoreAllInput = false;
    }

    public void StartMainGame(bool newGame = false)
    {
        OnGameStart?.Invoke();
        AsyncOperation asop = SceneManager.LoadSceneAsync(MainGameName);
        asop.completed += _ => OnGameStarted?.Invoke();
    }

    public void StartMiniFossilGame(int level) //TODO Parameters
    {
        OnMiniFossilGameStart?.Invoke();
        AsyncOperation asop = SceneManager.LoadSceneAsync(MiniFossilGameName);

        OnMiniFossilGameStarted += () => FossilePlayer.Instance.InitPlayer(50);

        asop.completed += _ => OnMiniFossilGameStarted?.Invoke();


    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    public void PauseGame(bool toggle)
    {
        if(_pauseMenu == null)
            _pauseMenu = Instantiate(PauseMenuPrefab);

        if (toggle)
        {
            _pauseMenu.SetActive(!_pauseMenu.activeSelf);
            IsPaused = _pauseMenu.activeSelf;
        }
        else
        {
            _pauseMenu.SetActive(true);
            IsPaused = true;
        }
    }
}
