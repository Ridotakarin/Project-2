using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

public class ExportTaggedObjectsEditor : EditorWindow
{
    private TaggedObjectList targetSO;
    private string savePath = "Assets/Prefabs";
    private string assetPath = "Assets/TaggedObjectList.asset";
    private static readonly List<string> tagFilters = new List<string> { "Tree", "House" };

    [MenuItem("Tools/Export Tagged Objects")]
    public static void ShowWindow()
    {
        GetWindow<ExportTaggedObjectsEditor>("Export Tagged Objects");
    }

    void OnGUI()
    {
        GUILayout.Label("Export Objects With Tags: Tree, house", EditorStyles.boldLabel);

        targetSO = (TaggedObjectList)EditorGUILayout.ObjectField("Target SO", targetSO, typeof(TaggedObjectList), false);
        assetPath = EditorGUILayout.TextField("SO Save Path", assetPath);
        savePath = EditorGUILayout.TextField("Prefab Save Path", savePath);

        if (GUILayout.Button("Export"))
        {
            ExportObjects();
        }
    }

    void ExportObjects()
    {
        if (targetSO == null)
        {
            targetSO = AssetDatabase.LoadAssetAtPath<TaggedObjectList>(assetPath);
            if (targetSO == null)
            {
                targetSO = CreateInstance<TaggedObjectList>();
                AssetDatabase.CreateAsset(targetSO, assetPath);
                AssetDatabase.SaveAssets();
                Debug.Log("Created new TaggedObjectList asset at " + assetPath);
            }
        }

        targetSO.allObjects.Clear();

        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (!obj.activeInHierarchy || obj.CompareTag("Untagged")) continue;
            if (!tagFilters.Contains(obj.tag)) continue;

            GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(obj);

            if (prefab == null)
            {
                string localPath = $"{savePath}/{obj.name}.prefab";
                localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
                prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(obj, localPath, InteractionMode.UserAction);
                Debug.Log($"Created prefab: {localPath}");
            }

            targetSO.allObjects.Add(new TaggedObjectList.TaggedObjectData
            {
                tag = obj.tag,
                position = obj.transform.position,
                prefab = prefab
            });
        }

        EditorUtility.SetDirty(targetSO);
        AssetDatabase.SaveAssets();
        Debug.Log($"Exported {targetSO.allObjects.Count} objects to ScriptableObject.");
    }
}
