using UnityEngine;
using UnityEngine.UI;

public class InfoManager : Singleton<InfoManager>
{
    public Text MorningText, NoonText, EveText, NightText;

    void Start()
    {
        MorningText.text = "";
        NoonText.text = "";
        EveText.text = "";
        NightText.text = "";
    }

    public void RefreshPlayerInfo(Player player)
    {
        MorningText.text = Mathf.RoundToInt(player.Health).ToString();
        NoonText.text = Mathf.RoundToInt(player.Hunger).ToString();
        EveText.text = Mathf.RoundToInt(player.Thirst).ToString();
    }
}
