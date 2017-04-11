using System;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    public bool EnableDebug = true;
    public bool DebugStats = true;
    public Sprite TestSprite;

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
        if(GUI.Button(new Rect(5,505,120,35), "Give Player0 Item"))
        {
            PlayerController.Instance.GiveItemToPlayer(0, new FoodItem
            {
                Name = "Banana",
                Icon = TestSprite,
                Buff = "",
                DrinkValue = 0,
                FoodValue = 45,
                Rarity = ItemRarity.Common
            });
        }

        if (GUI.Button(new Rect(5, 545, 120, 35), "Take Player0 Item"))
        {
            PlayerController.Instance.TakeItemToPlayer(0, new FoodItem
            {
                Name = "Banana",
                Icon = TestSprite,
                Buff = "",
                DrinkValue = 0,
                FoodValue = 45,
                Rarity = ItemRarity.Common
            });
        }

        if (GUI.Button(new Rect(5, 585, 120, 35), "Reset"))
        {
            PlayerController.Instance.RemoveAllPlayers();
            PlayerController.Instance.Initilize();
        }

    }

#endif
}
