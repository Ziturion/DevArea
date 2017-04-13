using System;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public Image[] ItemImages;
    public GenericItem[] Items;

    private int _currentInventoryCount = 0;

    void Awake()
    {
        Items = new GenericItem[PlayerController.InventorySize];
    }

    public void AddItem(GenericItem item)
    {
        if (_currentInventoryCount >= PlayerController.InventorySize)
        {
            Debug.LogWarning("Players can only have " + PlayerController.InventorySize + " Items.");
            return;
        }
        Items[_currentInventoryCount] = item;
        _currentInventoryCount++;

        RefreshIcons();
    }

    public void RemoveItem(int index)
    {
        if (index < 0 || index > Items.Length || Items[index] == null)
            return;

        Items[index] = null;
        _currentInventoryCount--;

        for (int i = Items.Length - 1; i > 0; i--)
        {
            if (Items[i] == null)
                continue;

            // ReSharper disable once InvertIf
            if (Items[i - 1] == null)
            {
                Items[i - 1] = Items[i];
                Items[i] = null;
                //reset
                i = Items.Length;
            }
        }

        RefreshIcons();
    }

    public void RemoveItem(GenericItem item)
    {
        //player inventory is empty
        if (Items.All(t => t == null))
            return;

        //if no item found 
        if (Items.Where(t => t != null).All(t => t.Name != item.Name))
            return;

        RemoveItem(Array.FindIndex(Items, t => t.Name == item.Name)); //TODO Change to Index
    }

    public void RefreshIcons()
    {
        for (int i = 0; i < ItemImages.Length; i++)
        {
            ItemImages[i].enabled = Items[i] != null;

            if (Items[i] == null)
                continue;

            ItemImages[i].sprite = Items[i].GetIcon();
        }
    }

    public GenericItem[] GetAllItems()
    {
        return Items.Where(t => t != null).ToArray();
    }

    public override string ToString()
    {
        StringBuilder invName = new StringBuilder("Inv: ");
        foreach (GenericItem item in GetAllItems())
        {
            invName.Append(item + " , ");
        }
        return invName.ToString();
    }
}
