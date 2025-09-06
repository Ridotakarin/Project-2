using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CraftingSystemManager : PersistentSingleton<CraftingSystemManager>
{
    public List<GameObject> listOutputSlot;
    public GameObject outputSlot;
    public List<Recipe> recipes;
    public GameObject ui_itemPrefab;

    private UI_InventoryItem[,] grid = new UI_InventoryItem[3, 3];

    [SerializeField] private InventoryManagerSO _inventoryManagerSO;
    private void OnEnable()
    {
        UI_InventoryItem.OnCraftingSlotAdded += AddItemToGrid;
        UI_InventoryItem.OnItemInCraftingSlotDrag += RemoveItemFromGrid;
        UI_InventoryItem.OnNewItemCreatedBeginDrag += TakeOffItem;
    }

    private void OnDisable()
    {
        UI_InventoryItem.OnCraftingSlotAdded -= AddItemToGrid;
        UI_InventoryItem.OnItemInCraftingSlotDrag -= RemoveItemFromGrid;
        UI_InventoryItem.OnNewItemCreatedBeginDrag -= TakeOffItem;
    }

    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(WaitForUISceneIsLoaded());
    }

    private IEnumerator WaitForUISceneIsLoaded()
    {
        yield return new WaitUntil(() => SceneManager.GetSceneByName("UIScene").isLoaded);

        listOutputSlot = SceneUtils.FindAllGameObjectsByNameInScene("OutputItemSlot", SceneManager.GetSceneByName("UIScene"));

    }
    public void AddItemToGrid(int i, int j, UI_InventoryItem item)
    {
        grid[i, j] = item;
        StartCoroutine(CheckRecipe());
    }

    public void RemoveItemFromGrid(int i, int j, Item item)
    {
        grid[i, j] = null;
        UI_InventoryItem uI_InventoryItem = outputSlot.GetComponentInChildren<UI_InventoryItem>();
        if (uI_InventoryItem != null) Destroy(uI_InventoryItem.gameObject);
        StartCoroutine(CheckRecipe());
    }

    public IEnumerator CheckRecipe()
    {

        yield return new WaitForEndOfFrame();
        if (grid == null) yield break;

        foreach (Recipe recipe in recipes)
        {
            bool completeRecipe = true;

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    var slotItem = grid[i, j];
                    var recipeItem = recipe.GetItem(i, j);

                    if ((slotItem == null && recipeItem != null) ||
                        (slotItem != null && recipeItem == null) ||
                        (slotItem != null && recipeItem != null &&
                        slotItem.InventoryItem.Item.itemName != recipeItem.itemName))
                    {
                        completeRecipe = false;
                        break;
                    }
                }

                if (!completeRecipe) break;
            }

            if (completeRecipe)
            {
                CreateItem(recipe.itemOutput, recipe.Quantity);
                yield break;
            }
        }
    }

    public void CreateItem(Item item, int quantity)
    {
        if (item == null) return;
       
        if (outputSlot.transform.childCount > 0) return;

        UI_InventorySlot slot = outputSlot.GetComponent<UI_InventorySlot>();
        InventoryItem inventoryItem = new InventoryItem(System.Guid.NewGuid().ToString(), item, slot.slotIndex, quantity, 1);
        GameObject newItem = Instantiate(ui_itemPrefab, outputSlot.transform);
        UI_InventoryItem inventoryItemUI = newItem.GetComponent<UI_InventoryItem>();
        inventoryItemUI.InitialiseItem(inventoryItem);
        inventoryItemUI.IsItemCreated(true);
    }

    public void TakeOffItem()
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i, j] != null)
                {
                    if (grid[i, j].InventoryItem.Quantity > 1)
                    {
                        grid[i, j].InventoryItem.DecreaseQuantity(1);
                        grid[i, j].RefreshCount();
                    }
                    else if (grid[i, j].InventoryItem.Quantity == 1)
                    {
                        _inventoryManagerSO.RemoveInventoryItem(grid[i, j].InventoryItem);
                        Destroy(grid[i, j].gameObject);
                    }
                }
                else continue;
            }
        }
        StartCoroutine(CheckRecipe());
    }

    public void SetNumCraftingTable(int num)
    {
        this.outputSlot = listOutputSlot[num];
    }

    // this is onInventoryClosed or onWorkbenchClosed
    public void OnCraftingWindowClosed(Component sender, object data)
    {
        foreach(var outputSlot in listOutputSlot)
        {
            var itemInOutputSlot = outputSlot.GetComponentInChildren<UI_InventoryItem>();
            if (itemInOutputSlot != null)
            {
                _inventoryManagerSO.RemoveInventoryItem(itemInOutputSlot.InventoryItem);
                Destroy(itemInOutputSlot.gameObject);
            }
        }
        
        
        for(int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i, j] != null)
                {
                    grid[i, j].OnCraftingWindowClosed();
                }
            }
        }
    }
}