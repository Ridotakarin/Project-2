using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "ScriptableObject/Item")]
[System.Serializable]
public class Item : ScriptableObject
{
    [Header("Only gameplay")]
    public int itemPrice = 0;

    [Header("For Crops")]
    public Crop CropSetting;

    [Header("For Crops Products")]
    public Sprite[] cropLevelImage;

    [Header("For FarmAnimal")]
    public FarmAnimal.Food foodType = FarmAnimal.Food.None;

    [Header("For Tools")]
    public TileBase tile;
    public RuleTile ruleTile;
    public AnimatedTile animatedTile;
    public Tilemap tilemap;
    public ItemType type;
    public Vector3Int range = new Vector3Int(5, 4);

    [Header("Only UI")]
    public bool stackable = true;

    [Header("Both")]
    public Sprite image;
    public string itemName;

    

}
[System.Serializable]
public class Crop
{
    public string cropProductName;
    public TileBase[] growthStages;
    public int TimeToGrowth;
    public bool canReHarvest;
    public int[] numOfProductCouldDrop;
    public float[] ratioForEachNum;
    public ESeason season;
}
public enum ItemType
{
    Tile,
    Food,
    Tool,
    Crop,
    Object,
    Usable,
    Resouce,
}

