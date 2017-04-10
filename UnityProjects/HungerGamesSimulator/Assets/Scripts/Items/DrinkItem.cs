using System;
using System.Text;

[Serializable]
public class DrinkItem : GenericItem
{
    public float DrinkValue;

    public override string ToString()
    {
        StringBuilder name = new StringBuilder(base.ToString());
        name.Append(" (D: " + DrinkValue + ")");
        return name.ToString();
    }
}
