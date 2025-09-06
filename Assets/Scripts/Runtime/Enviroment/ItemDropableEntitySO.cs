using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/ItemDropableEntitySO")]
[System.Serializable]
public class ItemDropableEntitySO : ScriptableObject
{
    public string id; // Unique string or GUID
    [Header("For enity doesn't have Animation")]
    public Sprite mineBlockIdleSprite;
    public Sprite mineBlockHitSprite;

    [Header("For Both")]
    public Item ItemToDrop;
    public int[] numOfItemCouldDrop;
    public float[] ratioForEachNum;

    [ContextMenu("Generate GUID")]
    public void GenerateGUID()
    {
        id = System.Guid.NewGuid().ToString();
    }
}
