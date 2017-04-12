using System;
using System.Text;
using EmptySkull.TypeDatabases;
using UnityEngine;

[Serializable]
public class BaseObject : DatabaseAsset
{
    //public string Name;
    public Sprite Icon;
    public ItemRarity Rarity;

    public override string ToString()
    {
        StringBuilder name = new StringBuilder(Name);
        name.Append(" [" + Rarity + "]");
        return name.ToString();
    }
}

public enum ItemRarity
{
    UnObtainable,
    Common,
    Rare,
    Epic,
    Legendary
}
