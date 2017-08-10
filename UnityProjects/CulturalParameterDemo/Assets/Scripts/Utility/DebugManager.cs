using UnityEngine;

public class DebugManager : Singleton<DebugManager>
{
    public GameObject TileDebuggerObject, CultureDebuggerObject;

	protected void Update()
    {
        //Debugging
        if (Input.GetKeyDown(KeyCode.F4))
        {
            TileDebuggerObject.SetActive(!TileDebuggerObject.activeSelf);
            CultureDebuggerObject.SetActive(!CultureDebuggerObject.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            SetCultureDebug(0);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            SetCultureDebug(1);
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            SetCultureDebug(2);
        }

        if(CultureDebuggerObject.activeSelf)
            UpdateCultureDebug();
    }

    private void SetCultureDebug(int index)
    {
        if (!TimeManager.Started)
            return;
        if (CultureManager.Instance.Cultures.Length <= index)
            return;

        CultureDebuggerObject.GetComponent<CultureDebugger>().SetCulture(CultureManager.Instance.Cultures[index]);
    }

    private void UpdateCultureDebug()
    {
        if (!TimeManager.Started)
            return;
        CultureDebuggerObject.GetComponent<CultureDebugger>().UpdateCultureTexts();
    }
}
