using System;
using System.Text;

[Serializable]
public class WeaponItem : GenericItem
{
    public WeaponDistance Distance;
    public float Damage;

    public override string ToString()
    {
        StringBuilder name = new StringBuilder(base.ToString());
        name.Append(" (Distance: " + Distance + " ATK: " + Damage + ")");
        return name.ToString();
    }
}

public enum WeaponDistance
{
    Near,
    Normal,
    Far
}
