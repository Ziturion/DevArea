using System;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    public bool EnableDebug = true;
    public bool DebugStats = true;

    /// <summary>
    /// PreInit is invoked in Unitys Awake Method
    /// </summary>
    public event Action PreInit;
    /// <summary>
    /// Init is invoked in Unitys Start Method
    /// </summary>
    public event Action Init;
    /// <summary>
    /// PostInit is invoked in Unitys Start Method but after Init
    /// </summary>
    public event Action PostInit;

    void Awake ()
	{
	    if (EnableDebug)
	    {
	        PreInit += () => Debug.Log("Pre-Initliazing...");
	        Init += () => Debug.Log("Initliazing...");
	        PostInit += () => Debug.Log("Post-Initliazing...");
	    }

	    if(PreInit != null)
            PreInit.Invoke();
	}

    void Start()
    {
        if (Init != null)
            Init.Invoke();
        if (PostInit != null)
            PostInit.Invoke();
    }

	void Update ()
    {
		
	}

#if UNITY_EDITOR

    void OnGUI()
    {
        if(GUI.Button(new Rect(5,35,120,35), "Give Player0 Item"))
        {
            PlayerController.Instance.GiveItemToPlayer(0, new FoodItem
            {
                Name = "Fiery Greatsword",
                Buff = "",
                DrinkValue = 10,
                FoodValue = 100,
                Rarity = ItemRarity.Legendary
            });
        }

        if (GUI.Button(new Rect(5, 75, 120, 35), "Take Player0 Item"))
        {
            PlayerController.Instance.TakeItemToPlayer(0, new FoodItem
            {
                Name = "Fiery Greatsword",
                Buff = "",
                DrinkValue = 10,
                FoodValue = 100,
                Rarity = ItemRarity.Legendary
            });
        }

    }

#endif
}
