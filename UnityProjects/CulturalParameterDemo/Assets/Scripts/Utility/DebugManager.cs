using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : Singleton<DebugManager>
{
    public GameObject TileDebuggerObject;

	protected void Update()
    {

#if UNITY_EDITOR
        //Debugging
        if (Input.GetKeyDown(KeyCode.F1))
        {
            TileDebuggerObject.SetActive(!TileDebuggerObject.activeSelf);
        }
#endif

    }
}
