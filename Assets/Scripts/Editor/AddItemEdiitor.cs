#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AddItemTest))]
public class AddItemTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        AddItemTest script = (AddItemTest)target;

        script.itemName = EditorGUILayout.TextField("Item Name", script.itemName);

        if (GUILayout.Button("Add Item"))
        {
            script.AddItem();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(script);
        }
    }
}
#endif