using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{

    public Player PlayerPrefab;
    public Transform PlayerAnchor;

    public const int InventorySize = 5;

    public List<Player> AllPlayers { get; private set; }

	void Awake ()
	{
        AllPlayers = new List<Player>();
	    GameController.Instance.Init += Initilize;
	}

    public void ClickedOnPlayer(Player player)
    {
        DeselectAllPlayers();
        player.ActiveImage.enabled = true;
        InfoManager.Instance.RefreshPlayerInfo(player);
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

    private void Initilize()
    {
        if (GameController.Instance.EnableDebug)
            Debug.Log("Initilizing Players");
        for (int i = 0; i < 16; i++)
        {
            AddPlayer();
        }
    }
    private void DeselectAllPlayers()
    {
        foreach (Player player in AllPlayers)
        {
            player.ActiveImage.enabled = false;
        }
    }

    private void AddPlayer()
    {
        Player player = Instantiate(PlayerPrefab, PlayerAnchor);
        player.Initilize("Dummy", 100, 100, 100);
        AllPlayers.Add(player);
    }

    public void RemovePlayer(int index)
    {
        if (index < 0 || index > AllPlayers.Count)
            return;

        RemovePlayer(AllPlayers[index]);
    }

    public void RemovePlayer(string playerName)
    {
        RemovePlayer(AllPlayers.Find(t => t.PlayerName.text == playerName));
    }

    public void RemovePlayer(Player player)
    {
        if (!AllPlayers.Contains(player))
            return;

        Destroy(player);
        AllPlayers.Remove(player);
    }

    public void RemoveAllPlayers()
    {
        foreach (Player player in AllPlayers)
        {
            Destroy(player);
        }
        AllPlayers.Clear();
    }
}
