using System.Collections.Generic;
using System.Linq;
using EmptySkull.TypeDatabases;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{

    public Player PlayerPrefab;
    public Transform PlayerAnchor;

    public const int InventorySize = 5;

    public float ChanceItemFind = 0.5f;
    public float ChanceTrapFind = 0.2f;
    public float ChanceSurvivorFind = 0.1f;

    public float ChanceFood = 0.5f;
    public float ChanceDrink = 0.4f;
    public float ChanceWeapon = 0.2f;

    public List<Player> AllPlayers { get; private set; }

	void Awake ()
	{
        AllPlayers = new List<Player>();
	    GameController.Instance.Init += Initilize;
	    TimeController.Instance.OnPostPhaseChanged += DeselectAllPlayers;
	    TimeController.Instance.OnPostPhaseChanged += GlobalPlayerStatDecrease;
	    TimeController.Instance.OnPostPhaseChanged += RefreshAllPlayers;
	}

    public void ClickedOnPlayer(Player player)
    {
        DeselectAllPlayers();
        player.ActiveImage.enabled = true;
        InfoManager.Instance.RefreshPlayerInfo(player);
    }

    public Player GetRandomPlayerEncounter()
    {
        return AllPlayers[Random.Range(0, AllPlayers.Count)];
    }

    public void GiveItemToPlayer(Player player, GenericItem item)
    {
        if (player == null || item == null)
            return;
        player.Inventory.AddItem(item);
    }
    public void GiveItemToPlayer(string playerName, GenericItem item)
    {
        GiveItemToPlayer(AllPlayers.Find(t => t.PlayerName.text == playerName), item);
    }
    public void GiveItemToPlayer(int playerIndex, GenericItem item)
    {
        if (playerIndex < 0 || playerIndex > AllPlayers.Count)
            return;
        GiveItemToPlayer(AllPlayers[playerIndex], item);
    }

    public void TakeItemToPlayer(Player player, GenericItem item)
    {
        if (player == null || item == null)
            return;
        player.Inventory.RemoveItem(item);
    }
    public void TakeItemToPlayer(string playerName, GenericItem item)
    {
        TakeItemToPlayer(AllPlayers.Find(t => t.PlayerName.text == playerName), item);
    }
    public void TakeItemToPlayer(int playerIndex, GenericItem item)
    {
        if (playerIndex < 0 || playerIndex > AllPlayers.Count)
            return;
        TakeItemToPlayer(AllPlayers[playerIndex], item);
    }

    public void RemovePlayer(int index, Cause cause = null)
    {
        if (index < 0 || index > AllPlayers.Count)
            return;

        RemovePlayer(AllPlayers[index], cause);
    }
    public void RemovePlayer(string playerName, Cause cause = null)
    {
        RemovePlayer(AllPlayers.Find(t => t.PlayerName.text == playerName), cause);
    }
    public void RemovePlayer(Player player, Cause cause = null)
    {
        if (!AllPlayers.Contains(player))
            return;

        player.SetInactive(cause);
        //AllPlayers.Remove(player);
    }

    public void RemoveAllPlayers()
    {
        foreach (Player player in AllPlayers)
        {
            Destroy(player.gameObject);
        }
        AllPlayers.Clear();
    }

    public void Initilize()
    {
        if (GameController.Instance.EnableDebug)
            Debug.Log("Initilizing Players");
        for (int i = 0; i < 16; i++)
        {
            AddPlayer("Dummy" + i);
        }
    }

    public void PlayerIsSearching(Player player)
    {
        if (ChanceItemFind > Random.Range(0f, 1f))
        {
            PlayerFindsItem(player);
            return;
        }

        if (ChanceTrapFind > Random.Range(0f, 1f))
        {
            PlayerFindsTrap(player);
            return;
        }

        if (ChanceSurvivorFind > Random.Range(0f, 1f))
        {
            PlayerFindsSurvivor(player);
            return;
        }

        PlayerFindsItem(player);
    }

    private void PlayerFindsItem(Player player)
    {
        //TODO RarityContext

        if (ChanceFood > Random.Range(0f, 1f))
        {
            GiveItemToPlayer(player, BaseObject.GetObjectByRarity(DatabaseReader.GetAllItems("FoodItems").OfType<BaseObject>().ToArray()) as FoodItem);
            return;
        }

        if (ChanceDrink > Random.Range(0f, 1f))
        {
            GiveItemToPlayer(player, BaseObject.GetObjectByRarity(DatabaseReader.GetAllItems("DrinkItems").OfType<BaseObject>().ToArray()) as DrinkItem);
            return;
        }

        if (ChanceWeapon > Random.Range(0f, 1f))
        {
            GiveItemToPlayer(player, BaseObject.GetObjectByRarity(DatabaseReader.GetAllItems("WeaponItems").OfType<BaseObject>().ToArray()) as WeaponItem);
            return;
        }
        GiveItemToPlayer(player, BaseObject.GetObjectByRarity(DatabaseReader.GetAllItems("FoodItems").OfType<BaseObject>().ToArray()) as FoodItem);
    }

    private void PlayerFindsTrap(Player player)
    {
        Trap foundTrap = BaseObject.GetObjectByRarity(DatabaseReader.GetAllItems("Traps").OfType<BaseObject>().ToArray()) as Trap;

        if (foundTrap.SurviveChance + (player.Intelligence * 0.04f) < Random.Range(0f, 1f))
        {
            RemovePlayer(player, new Cause(foundTrap)); //Kill Player
        }

        Debug.Log("Player Survived");
    }

    private void PlayerFindsSurvivor(Player player)
    {
        Debug.Log("Player Found...");
        if (0.5f > Random.Range(0f, 1f)) // 50/50
        {
            RemovePlayer(player); //Kill Player
        }
        else
        {
            RemovePlayer(GetRandomPlayerEncounter()); //Kill Defender
        }
    }

    private void DeselectAllPlayers()
    {
        foreach (Player player in AllPlayers)
        {
            player.ActiveImage.enabled = false;
        }
        InfoManager.Instance.ResetTexts();
    }

    private void RefreshAllPlayers()
    {
        AllPlayers = AllPlayers.OrderBy(t => !t.Active).ToList();
        foreach (Player player in AllPlayers)
        {
            player.Refresh();
            player.gameObject.transform.SetSiblingIndex(AllPlayers.IndexOf(player));
        }
    }

    private void GlobalPlayerStatDecrease()
    {
        for (int i = AllPlayers.Count - 1; i >= 0; i--)
        {
            AllPlayers[i].PlayerPhaseCompute(TimeController.Instance.CurrentTime);
        }
    }

    private void AddPlayer(string name = "Dummy", float health = 100, float hunger = 100, float thirst = 100)
    {
        Player player = Instantiate(PlayerPrefab, PlayerAnchor);
        player.Initilize(name, health, hunger, thirst);
        player.OnDeath += () => RemovePlayer(player);
        AllPlayers.Add(player);
    }

    public class Cause
    {
        public BaseObject Instigator;

        public Cause(BaseObject instigator)
        {
            Instigator = instigator;
        }
    }
}
