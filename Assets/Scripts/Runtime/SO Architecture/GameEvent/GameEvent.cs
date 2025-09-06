using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class GameEvent : ScriptableObject
{
#if UNITY_EDITOR
    [HideInInspector]
    public List<GameEventListener> editorListeners = new List<GameEventListener>();
#endif

    private readonly List<GameEventListener> listeners = new();

    public void RegisterListener(GameEventListener listener)
    {
        if (!listeners.Contains(listener))
            listeners.Add(listener);

#if UNITY_EDITOR
        if (!editorListeners.Contains(listener))
            editorListeners.Add(listener);
#endif
    }

    public void UnRegisterListener(GameEventListener listener)
    {
        listeners.Remove(listener);
#if UNITY_EDITOR
        editorListeners.Remove(listener);
#endif
    }

    public void Raise(Component sender, object data)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised(sender, data);
        }
    }

#if UNITY_EDITOR
    public List<GameObject> ListenerObjects =>
        editorListeners.Where(l => l != null).Select(l => l.gameObject).ToList();
#endif
}
