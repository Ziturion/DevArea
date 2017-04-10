using System;

[Serializable]
public class SupplyDrop : BaseObject
{
    public int PackageNumber;
    public SupplyItem[] Items;
}

[Serializable]
public struct SupplyItem
{
    public SupplyType Type;
    public int Index;
}

public enum SupplyType
{
    Food,
    Drink,
    Weapon,
    Misc
}
