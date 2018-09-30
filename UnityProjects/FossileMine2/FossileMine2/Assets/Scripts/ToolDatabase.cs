using UnityEngine;

[CreateAssetMenu(fileName = "toolDatabase", menuName = "Fossil/toolDatabase", order = 1)]
public class ToolDatabase : ScriptableObject
{
    public Tool[] Tools;
}
