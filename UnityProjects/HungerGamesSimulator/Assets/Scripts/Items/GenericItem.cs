using System;
using System.Text;

[Serializable]
public class GenericItem : BaseObject
{
    public string Buff;

    public override string ToString()
    {
        StringBuilder name = new StringBuilder(base.ToString());
        if (!string.IsNullOrEmpty(Buff))
        {
            name.Append(" - " + Buff);
        }
        return name.ToString();
    }
}
