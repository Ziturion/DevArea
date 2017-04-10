using System;
using System.Text;

[Serializable]
public class FoodItem : GenericItem
{
    public float FoodValue;
    public float DrinkValue;

    public override string ToString()
    {
        StringBuilder name = new StringBuilder(base.ToString());
        name.Append(" (F: " + FoodValue + " D: " + DrinkValue + ")");
        return name.ToString();
    }
}
