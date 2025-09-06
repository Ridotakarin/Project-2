using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameEvent))]
public class GameEventEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameEvent gameEvent = (GameEvent)target;

        // Clean up null references
        gameEvent.editorListeners.RemoveAll(l => l == null);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Active Listeners (Edit + Play Mode)", EditorStyles.boldLabel);

        var listeners = gameEvent.editorListeners;

        if (listeners == null || listeners.Count == 0)
        {
            EditorGUILayout.LabelField("No listeners currently registered.");
        }
        else
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                var listener = listeners[i];
                if (listener == null)
                    continue;

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.ObjectField(listener.gameObject.name, listener.gameObject, typeof(GameObject), true);

                if (GUILayout.Button("Remove", GUILayout.Width(60)))
                {
                    // Confirm deletion
                    if (EditorUtility.DisplayDialog(
                        "Remove Listener",
                        $"Are you sure you want to delete the GameEventListener component from '{listener.gameObject.name}'?",
                        "Yes", "Cancel"))
                    {
                        gameEvent.UnRegisterListener(listener);
                        Undo.RecordObject(listener.gameObject, "Remove GameEventListener");
                        Undo.DestroyObjectImmediate(listener);
                        EditorUtility.SetDirty(gameEvent);
                        break;
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Remove All Listeners"))
            {
                if (EditorUtility.DisplayDialog(
                    "Remove ALL Listeners",
                    "Are you sure you want to delete ALL GameEventListener components linked to this GameEvent?",
                    "Yes", "Cancel"))
                {
                    for (int i = listeners.Count - 1; i >= 0; i--)
                    {
                        var listener = listeners[i];
                        if (listener == null) continue;

                        gameEvent.UnRegisterListener(listener);
                        Undo.RecordObject(listener.gameObject, "Remove GameEventListener");
                        Undo.DestroyObjectImmediate(listener);
                    }

                    listeners.Clear();
                    EditorUtility.SetDirty(gameEvent);
                }
            }
        }

        // Keep inspector updating in edit mode
        if (!Application.isPlaying)
        {
            EditorApplication.QueuePlayerLoopUpdate();
            SceneView.RepaintAll();
        }
    }
}
