using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    public static T Instance
    {
        get
        {
            if(_instance != null)
                return _instance;
            _instance = FindObjectOfType<T>();
            if (_instance != null)
                return _instance;
            GameObject go = new GameObject("[Singleton] " + typeof(T));
            _instance = go.AddComponent<T>();
            Debug.Log($"[Singleton] Did not find {typeof(T)} object. Creating new Singleton.");
            return _instance;
        }
    }

    private static T _instance;
}
