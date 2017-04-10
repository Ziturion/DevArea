using System;
using UnityEngine;

public class TimeController : Singleton<TimeController>
{

    public event Action OnPrePhaseChanged;
    public event Action OnPostPhaseChanged;
    public event Action OnPreDayChanged;
    public event Action OnPostDayChanged;

    public TimeOfDay CurrentTime { get; private set; }
    public int CurrentDay { get; private set; }

    public void NextPhase()
    {
        if(OnPrePhaseChanged != null)
            OnPrePhaseChanged.Invoke();


        switch (CurrentTime)
        {
            case TimeOfDay.Morning:
                CurrentTime = TimeOfDay.Noon;
                if (GameController.Instance.EnableDebug)
                    Debug.Log("Refreshed Time to Noon");
                break;
            case TimeOfDay.Noon:
                CurrentTime = TimeOfDay.Evening;
                if (GameController.Instance.EnableDebug)
                    Debug.Log("Refreshed Time to Evening");
                break;
            case TimeOfDay.Evening:
                CurrentTime = TimeOfDay.Night;
                if (GameController.Instance.EnableDebug)
                    Debug.Log("Refreshed Time to Night");
                break;
            case TimeOfDay.Night:
                CurrentTime = TimeOfDay.Morning;
                ChangeDay();
                if (GameController.Instance.EnableDebug)
                    Debug.Log("Refreshed Time to Morning");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (OnPostPhaseChanged != null)
            OnPostPhaseChanged.Invoke();
    }

    public void NextDay()
    {
        while (CurrentTime != TimeOfDay.Night)
        {
            NextPhase();
        }
        NextPhase();

        //ChangeDay();
    }

    private void ChangeDay()
    {
        if (OnPreDayChanged != null)
            OnPreDayChanged.Invoke();

        CurrentDay++;

        if (OnPostDayChanged != null)
            OnPostDayChanged.Invoke();
    }

    public enum TimeOfDay
    {
        Morning,
        Noon,
        Evening,
        Night
    }
}
