using UnityEngine;

[CreateAssetMenu(menuName = "Tilemap/MapGroup")]
public class MapGroupData : ScriptableObject
{
    public MapLayerData[] layers;
}
