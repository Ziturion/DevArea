using System;
using System.Text;
using EmptySkull.TypeDatabases;
using UnityEngine;

[Serializable]
public class BaseObject : DatabaseAsset
{
    public Sprite Icon;
    public ItemRarity Rarity;

    public override string ToString()
    {
        StringBuilder name = new StringBuilder(Name);
        name.Append(" [" + Rarity + "]");
        return name.ToString();
    }


    public Sprite GetIcon()
    {
        Sprite sp = Icon;
        if (sp == null)
            sp = GameController.Instance.MissingSprite;
        return sp;
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
