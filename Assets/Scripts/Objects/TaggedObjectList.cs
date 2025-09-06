using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TaggedObjectList", menuName = "Tools/Tagged Object List")]
public class TaggedObjectList : ScriptableObject
{
    [System.Serializable]
    public class TaggedObjectData
    {
        public string tag;
        public Vector3 position;
        public GameObject prefab;
    }

    public List<TaggedObjectData> allObjects = new();
}
