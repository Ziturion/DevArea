using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapCreator))]
public class MapCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapCreator myTarget = (MapCreator)target;

        if (DrawDefaultInspector())
        {
            if (myTarget.AutoUpdate)
            {
                myTarget.Generate();
            }
        }
        if (GUILayout.Button("Generate"))
        {
            myTarget.Generate();
        }
    }
}
