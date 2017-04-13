﻿using System.Collections.Generic;
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

        Destroy(player.gameObject);
        AllPlayers.Remove(player);
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
        foreach (Player player in AllPlayers)
        {
            player.Refresh();
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


}
