using UnityEngine;
using UnityEngine.UI;

public class InfoManager : Singleton<InfoManager>
{
    public Text MorningText, NoonText, EveText, NightText;

    void Start()
    {
        ResetTexts();
    }

    public void RefreshPlayerInfo(Player player)
    {
        MorningText.text = "Charisma of this Character is: " + Mathf.RoundToInt(player.Charisma);
        NoonText.text = "Strength of this Character is: " + Mathf.RoundToInt(player.Strength);
        EveText.text = "Intelligence of this Character is: " + Mathf.RoundToInt(player.Intelligence);
        NightText.text = player.NeedQueue.ToString();
    }

    public void ResetTexts()
    {
        MorningText.text = "";
        NoonText.text = "";
        EveText.text = "";
        NightText.text = "";
    }
}
