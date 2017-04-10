using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ChangeTextByTime : MonoBehaviour
{
    private Text _changingImage;

    void OnEnable()
    {
        _changingImage = GetComponent<Text>();
        Refresh();
        TimeController.Instance.OnPostPhaseChanged += Refresh;
    }

    void OnDisable()
    {
        if(TimeController.Instance != null)
            TimeController.Instance.OnPostPhaseChanged -= Refresh;
    }

    private void Refresh()
    {
        _changingImage.text = "Day " + (TimeController.Instance.CurrentDay + 1) + " - " + TimeController.Instance.CurrentTime;
    }
}
