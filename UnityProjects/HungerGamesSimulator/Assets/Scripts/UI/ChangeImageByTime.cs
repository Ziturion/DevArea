using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ChangeImageByTime : MonoBehaviour
{
    public Sprite MorningSprite, NoonSprite, EveSprite, NightSprite;
    private Image _changingImage;

    void OnEnable()
    {
        _changingImage = GetComponent<Image>();
        Refresh();
        TimeController.Instance.OnPostPhaseChanged += Refresh;
    }

    void OnDisable()
    {
        if (TimeController.Instance != null)
            TimeController.Instance.OnPostPhaseChanged -= Refresh;
    }

    private void Refresh()
    {
        switch (TimeController.Instance.CurrentTime)
        {
            case TimeController.TimeOfDay.Morning:
                _changingImage.sprite = MorningSprite;
                break;
            case TimeController.TimeOfDay.Noon:
                _changingImage.sprite = NoonSprite;
                break;
            case TimeController.TimeOfDay.Evening:
                _changingImage.sprite = EveSprite;
                break;
            case TimeController.TimeOfDay.Night:
                _changingImage.sprite = NightSprite;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
