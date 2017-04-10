using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Text PlayerName;
    public Slider HealthBar, HungerBar, ThirstBar;
    public PlayerInventory Inventory;
    public Image ActiveImage;

    public float MaxHealth { get; private set; }
    public float MaxHunger { get; private set; }
    public float MaxThirst { get; private set; }
    public float Health { get; private set; }
    public float Hunger { get; private set; }
    public float Thirst { get; private set; }

    public int Charisma { get; private set; }
    public int Strength { get; private set; }
    public int Intelligence { get; private set; }

    public void Initilize(string playerName, float health, float hunger, float thirst)
    {
        PlayerName.text = playerName;
        name = "Button_" + playerName;

        MaxHealth = health;
        MaxHunger = hunger;
        MaxThirst = thirst;

        Health = health;
        Hunger = Random.Range(75f, 96f);
        Thirst = Random.Range(65f, 100f);

        SetStats();

        Refresh();
        Inventory.RefreshIcons();
    }

    private static PlayerStats GenerateStats()
    {
        int statPoints = Random.Range(16, 24);
        int primaryStatPoints = Random.Range(8, 10);
        statPoints -= primaryStatPoints;
        int secondaryStatPoints = Random.Range(6, Mathf.Min(10, statPoints));
        statPoints -= secondaryStatPoints;
        int lastStatpoints = Mathf.Min(10, statPoints);
        statPoints -= lastStatpoints;

        if (statPoints <= 0)
            return new PlayerStats(primaryStatPoints, secondaryStatPoints, lastStatpoints);

        while (primaryStatPoints < 10 && statPoints > 0)
        {
            primaryStatPoints++;
            statPoints--;
        }
        return new PlayerStats(primaryStatPoints,secondaryStatPoints,lastStatpoints);
    }

    private void SetStats()
    {
        PlayerStats stats = GenerateStats();
        //Debug.Log(stats);

        Charisma = stats.PrimaryStat;
        Strength = stats.SecondaryStat;
        Intelligence = stats.TertiarStat;
    }

    public void Refresh()
    {
        HealthBar.value = Health / MaxHealth;
        HungerBar.value = Hunger / MaxHunger;
        ThirstBar.value = Thirst / MaxThirst;

        if (GameController.Instance.DebugStats)
        {
            HealthBar.value = Charisma / 10f;
            HungerBar.value = Strength / 10f;
            ThirstBar.value = Intelligence / 10f;
        }
    }

    public void OnClick()
    {
        PlayerController.Instance.ClickedOnPlayer(this);
    }

    private struct PlayerStats
    {
        public readonly int PrimaryStat, SecondaryStat, TertiarStat;

        public PlayerStats(int primaryStat, int secondaryStat, int tertiarStat)
        {
            PrimaryStat = primaryStat;
            SecondaryStat = secondaryStat;
            TertiarStat = tertiarStat;
        }

        public override string ToString()
        {
            return string.Format("First: {0}, Second: {1}, Third: {2}", PrimaryStat,
                SecondaryStat, TertiarStat);
        }
    }
}
