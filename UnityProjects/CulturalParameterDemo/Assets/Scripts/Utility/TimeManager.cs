using System;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : Singleton<TimeManager>
{
    public static event Action OnStartSimulation;
    public static event Action OnStartSimulationLate;
    public static event Action OnPauseSimulation;
    public static event Action OnForwardTimeSimulation;
    public static event Action OnNormalTimeSimulation;

    public Text TimeDisplayText;

    static TimeManager()
    {
        Started = false;
    }

    public static bool Started { get; private set; }

    protected void OnDisable()
    {
        OnStartSimulation = null;
        OnStartSimulationLate = null;
        OnPauseSimulation = null;
        OnForwardTimeSimulation = null;
        OnNormalTimeSimulation = null;
    }

    //Button Pause
    public void OnPressedPause()
    {
        if (!Started)
            return;

        if (OnPauseSimulation != null)
            OnPauseSimulation.Invoke();
        Time.timeScale = 0f;

        UpdateTimeText();
    }

    //Button Play
    public void OnPressedPlay()
    {
        if (!Started)
        {
            if(OnStartSimulation != null)
                OnStartSimulation.Invoke();
            Started = true;
            Debug.Log("Starting Simulation...");

            if (OnStartSimulationLate != null)
                OnStartSimulationLate.Invoke();
        }
        else
        {
            if (OnNormalTimeSimulation != null)
                OnNormalTimeSimulation.Invoke();
            Time.timeScale = 1f;
        }

        UpdateTimeText();
    }

    //Button Forward
    public void OnPressedForward(float scale)
    {
        if (!Started)
            return;

        if (OnForwardTimeSimulation != null)
            OnForwardTimeSimulation.Invoke();
        Time.timeScale = Math.Min(Time.timeScale + scale, 100);

        UpdateTimeText();
    }

    public void UpdateTimeText()
    {
        TimeDisplayText.text = string.Format("{0}x",Time.timeScale);
    }
}
