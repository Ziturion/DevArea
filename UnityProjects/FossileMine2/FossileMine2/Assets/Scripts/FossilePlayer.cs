using System.Linq;
using UnityEngine;

public class FossilePlayer : Singleton<FossilePlayer>
{
    public ToolDatabase PlayerTools;

    [HideInInspector]
    public Tool[] MyTools;
    public Tool ActiveTool;

    public void InitPlayer(int level) //TODO savegame
    {
        MyTools = PlayerTools.Tools.Where(t => level >= t.Level).ToArray();
        ActiveTool = MyTools.FirstOrDefault();
    }

    public void ChangeTool(int index)
    {
        if (index < 0 || index >= MyTools.Length)
        {
            Debug.LogWarning("[FossilePlayer] Tool out of Range");
            return;
        }
        ActiveTool = MyTools[index];
    }
}
