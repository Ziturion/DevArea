using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public Text PlayerName;
    public Slider HealthBar, HungerBar, ThirstBar;
    public PlayerInventory Inventory;
    public Image ActiveImage;

    public event Action OnDeath;

    public float MaxHealth { get; private set; }
    public float MaxHunger { get; private set; }
    public float MaxThirst { get; private set; }

    private float _health;
    private float _hunger;
    private float _thirst;

    private int _starvingPoints = 0;

    public float Health
    {
        get { return _health; }
        private set
        {
            if (value < 0)
            {
                _health = 0;
                if(OnDeath != null)
                    OnDeath.Invoke();
                OnDeath = null;
                return;
            }
            if (value > MaxHealth)
            {
                _health = MaxHealth;
                return;
            }
            _health = value;
        }
    }
    public float Hunger
    {
        get { return _hunger; }
        private set
        {
            if (value < 0)
            {
                _hunger = 0;
                _starvingPoints += Random.Range(1,6);
                return;
            }
            if (value > MaxHunger)
            {
                _hunger = MaxHunger;
                return;
            }
            _hunger = value;
        }
    }
    public float Thirst
    {
        get { return _thirst; }
        private set
        {
            if (value < 0)
            {
                _thirst = 0;
                _starvingPoints += Random.Range(1,4);
                return;
            }
            if (value > MaxThirst)
            {
                _thirst = MaxThirst;
                return;
            }
            _thirst = value;
        }
    }

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

    public bool PhaseStatDecrease(TimeController.TimeOfDay phase)
    {
        switch (phase)
        { 
            case TimeController.TimeOfDay.Morning:
                Hunger -= Strength > Intelligence ? 10 : 9; // -40 / -30
                Thirst -= Strength > Intelligence ? 7 : 6; // -30 / -25
                break;
            case TimeController.TimeOfDay.Noon:
                Hunger -= Strength > Intelligence ? 12 : 10;
                Thirst -= Strength > Intelligence ? 10 : 8;
                break;
            case TimeController.TimeOfDay.Evening:
                Hunger -= Strength > Intelligence ? 10 : 9;
                Thirst -= Strength > Intelligence ? 7 : 6;
                break;
            case TimeController.TimeOfDay.Night:
                Hunger -= Strength > Intelligence ? 8 : 7;
                Thirst -= Strength > Intelligence ? 6 : 5;
                break;
            default:
                throw new ArgumentOutOfRangeException("phase", phase, null);
        }

        if (Hunger > 30 || Thirst > 40)
            _starvingPoints = 0;

        Health -= _starvingPoints;

        return true;
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
        return new PlayerStats(primaryStatPoints, secondaryStatPoints, lastStatpoints);
    }

    private void SetStats()
    {
        PlayerStats stats = GenerateStats();
        //Debug.Log(stats);

        List<int> statList = new List<int> { stats.PrimaryStat, stats.SecondaryStat, stats.TertiarStat };

        int picked = Random.Range(0, 3);

        Charisma = statList[picked];

        statList.RemoveAt(picked);

        int picked2 = Random.Range(0, 2);

        Strength = statList[picked2];

        statList.RemoveAt(picked2);

        Intelligence = statList[0];
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
