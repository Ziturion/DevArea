using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_MainMenu : MonoBehaviour
{
    public Button ResumeGameBtn, StartNewGameBtn, OptionsBtn, QuitBtn;
    public GameObject LoadingScreen;

    private Action _loadingAction;

    protected virtual void OnEnable()
    {
        ResumeGameBtn.onClick.AddListener(ResumeGame);
        StartNewGameBtn.onClick.AddListener(StartNewGame);
        OptionsBtn.onClick.AddListener(Options);
        QuitBtn.onClick.AddListener(QuitGame);

        LoadingScreen.SetActive(false);
        _loadingAction = () => LoadingScreen.SetActive(true);
        GameController.OnGameStart += _loadingAction;
    }

    protected virtual void OnDisable()
    {
        GameController.OnGameStart -= _loadingAction;

        ResumeGameBtn.onClick.RemoveListener(ResumeGame);
        StartNewGameBtn.onClick.RemoveListener(StartNewGame);
        OptionsBtn.onClick.RemoveListener(Options);
        QuitBtn.onClick.RemoveListener(QuitGame);
    }

    private static void ResumeGame()
    {
        GameController.Instance.StartMainGame();
    }

    private static void StartNewGame()
    {
        GameController.Instance.StartMainGame(true);
    }

    private static void Options()
    {

    }

    private static void QuitGame()
    {
        GameController.Instance.QuitGame();
    }
}
