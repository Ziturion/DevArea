using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PauseMenu : MonoBehaviour
{
    public Button ResumeGameBtn, OptionsBtn, QuitBtn;

    protected virtual void OnEnable()
    {
        ResumeGameBtn.onClick.AddListener(ResumeGame);
        OptionsBtn.onClick.AddListener(Options);
        QuitBtn.onClick.AddListener(QuitGame);
    }

    protected virtual void OnDisable()
    {
        ResumeGameBtn.onClick.RemoveListener(ResumeGame);
        OptionsBtn.onClick.RemoveListener(Options);
        QuitBtn.onClick.RemoveListener(QuitGame);
    }

    private void ResumeGame()
    {
        GameController.Instance.PauseGame(true);
    }
    
    private void Options()
    {
        throw new System.NotImplementedException();
    }

    private void QuitGame()
    {
        GameController.Instance.QuitGame();
    }
}
