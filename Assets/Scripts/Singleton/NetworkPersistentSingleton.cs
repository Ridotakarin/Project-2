using Unity.Netcode;
using UnityEngine;

public class NetworkPersistentSingleton<T> : NetworkBehaviour where T : NetworkPersistentSingleton<T>
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<T>();
                if (instance == null)
                {
                    Debug.LogError(typeof(T).Name + " is not found in the scene.");
                }
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = (T)this;
        DontDestroyOnLoad(gameObject);
    }
}
