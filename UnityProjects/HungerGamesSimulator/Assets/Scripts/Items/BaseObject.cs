using System;
using System.Linq;
using System.Text;
using EmptySkull.TypeDatabases;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class BaseObject : DatabaseAsset
{
    public Sprite Icon;
    public ItemRarity Rarity;

    private const float RareChance = 0.38f; //C: 0.50 R: 0.38 E: 0.10 L: 0.02
    private const float EpicChance = 0.10f;
    private const float LegendaryChance = 0.02f;

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

    public static BaseObject GetObjectByRarity(BaseObject[] objects)
    {
        float chance = Random.Range(0f, 1f);
        ItemRarity rarity;

        if (chance < LegendaryChance)
        {
            rarity = ItemRarity.Legendary;
        }
        else if (chance < EpicChance)
        {
            rarity = ItemRarity.Epic;
        }
        else if (chance < RareChance)
        {
            rarity = ItemRarity.Rare;
        }
        else
        {
            rarity = ItemRarity.Common;
        }

        BaseObject[] rarityObjs = objects.Where(t => t.Rarity == rarity).ToArray();
        return rarityObjs.Length <= 0 ? null : rarityObjs[Random.Range(0, rarityObjs.Length)]; //Legendary

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
