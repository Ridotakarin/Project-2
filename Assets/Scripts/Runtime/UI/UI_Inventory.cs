using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;

#if UNITY_EDITOR
using UnityEditorInternal.Profiling.Memory.Experimental;
#endif
using UnityEngine;
using UnityEngine.Rendering;

public class UI_Inventory : MonoBehaviour
{
    public Transform toolBarContainer;
    public Transform inventoryContainer;
    public GameObject slotPrefab;
    public GameObject UI_inventoryItemPrefab;
    public List<UI_InventorySlot> inventorySlotsUI;
    private int maxToolBarSlot = 9;
    [SerializeField] private InventoryManagerSO _inventoryManagerSO;


    private void Start()
    {
       UpdateSlotUI(this, null);
    }

    private void OnEnable()
    {
        _inventoryManagerSO.onFindEmptySlot += FindEmptySlot;
        _inventoryManagerSO.onPutItemDownByRightClick += SpawnItem;
        _inventoryManagerSO.onDecreaseItemQuantity += DecreaseItemQuantity;
        GameEventsManager.Instance.inventoryEvents.onAddItemToUI += AddItem;
        GameEventsManager.Instance.inventoryEvents.onItemRemoved += RemoveItem;
        GameEventsManager.Instance.inventoryEvents.upDateAmount += UpdateItemAmount;
    }

    private void OnDisable()
    {
        _inventoryManagerSO.onFindEmptySlot -= FindEmptySlot;
        _inventoryManagerSO.onPutItemDownByRightClick -= SpawnItem;
        _inventoryManagerSO.onDecreaseItemQuantity -= DecreaseItemQuantity;
        GameEventsManager.Instance.inventoryEvents.onAddItemToUI -= AddItem;
        GameEventsManager.Instance.inventoryEvents.onItemRemoved -= RemoveItem;
        GameEventsManager.Instance.inventoryEvents.upDateAmount -= UpdateItemAmount;
    }

    public void UpdateSlotUI(Component sender, object data)
    {
        Debug.Log("UpdateSlotUI called by: " + sender.ToString());
        var inventory = _inventoryManagerSO.inventory;
        ClearSlotUI();

        inventorySlotsUI.Clear();
        int totalSlots = inventory.MaxSlotInventory;

        for (int i = 0; i < totalSlots; i++)
        {
            Transform parent = (i < maxToolBarSlot) ? toolBarContainer : inventoryContainer;
            GameObject slotUIGO = Instantiate(slotPrefab, parent);
            UI_InventorySlot inventoryslotUI = slotUIGO.GetComponent<UI_InventorySlot>();
            inventoryslotUI.slotIndex = i;
            inventorySlotsUI.Add(inventoryslotUI);

            InventoryItem inventoryItem = inventory.GetInventoryItemOfIndex(i);
            if (inventoryItem != null)
            {
                Item item = ItemDatabase.Instance.GetItemByName(inventoryItem.ItemName);
                inventoryItem.SetItem(item);
                SpawnItem(inventoryItem, slotUIGO);
            }
        }
    }

    public void ClearSlotUI()
    {
        if (inventorySlotsUI.Count == 0) return;
        Debug.Log("count of ui inven slot: " +  inventorySlotsUI.Count);
        foreach (UI_InventorySlot slotUI in inventorySlotsUI) Destroy(slotUI.gameObject);
        inventorySlotsUI.Clear();
    }

    // AddItemUI
    public bool AddItemToInventoryUI(InventoryItem inventoryItem, int index) // for load game
    {
        UI_InventorySlot slotUI = inventorySlotsUI[index];
        SpawnItem(inventoryItem, slotUI.gameObject);

        return true;
    }

    public void AddItem(InventoryItem inventoryItem, int index)
    {
        UI_InventorySlot slotUI = inventorySlotsUI[index];
        SpawnItem(inventoryItem, slotUI.gameObject);
    }

    private void RemoveItem(int slotIndex)
    {
        UI_InventoryItem uI_InventoryItem = inventorySlotsUI[slotIndex].GetComponentInChildren<UI_InventoryItem>();
        Destroy(uI_InventoryItem.gameObject);
    }

    public void UpdateItemAmount(int slotIndex)
    {
        UI_InventoryItem uI_InventoryItem = inventorySlotsUI[slotIndex].GetComponentInChildren<UI_InventoryItem>();
        uI_InventoryItem.RefreshCount();
    }

    public void SpawnItem(InventoryItem item, GameObject slot)
    {
        GameObject newItemGO = Instantiate(UI_inventoryItemPrefab, slot.transform);
        UI_InventoryItem inventoryItem = newItemGO.GetComponent<UI_InventoryItem>();
        inventoryItem.InitialiseItem(item);
        if(slot.GetComponent<UI_CraftingSlot>() != null)
        {
            inventoryItem.CheckItemSlot();
        }
        
    }

    // event stuff =======================================================================
    public void ChangeSelectedSlot(Component sender, object data)
    {
        int newValue = (int)data;
        inventorySlotsUI[_inventoryManagerSO.selectedSlot].Deselect();
        inventorySlotsUI[newValue].Select();
        _inventoryManagerSO.selectedSlot = newValue;
    }


    // on onItemWorldTouchPlayer
    public void AddItemToInventorySlotFromItemWorld(Component sender, object data) // for pick itemworld
    {
        ItemWorldControl itemWorldControl = sender as ItemWorldControl;
        ItemWorld item = itemWorldControl.GetItemWorld();
        InventoryItem newItem = new(item.Id, item.Item, 0, item.Quantity, item.Level);
        var inventory = _inventoryManagerSO.inventory;
        for (int i = 0; i < inventorySlotsUI.Count; i++)
        {
            UI_InventorySlot slotUI = inventorySlotsUI[i];
            UI_InventoryItem itemUI = slotUI.GetComponentInChildren<UI_InventoryItem>();

            if (itemUI != null &&
                itemUI.InventoryItem.Item == newItem.Item &&
                itemUI.InventoryItem.Item.stackable &&
                itemUI.InventoryItem.Quantity < itemUI.InventoryItem.MaxStack &&
                itemUI.InventoryItem.Level == newItem.Level)
            {
                if (itemUI.InventoryItem.Quantity + newItem.Quantity <= itemUI.InventoryItem.MaxStack)
                {
                    itemUI.InventoryItem.IncreaseQuantity(newItem.Quantity);
                    itemUI.RefreshCount();
                    ItemWorldManager.Instance.RemoveItemWorld(item, itemWorldControl);
                    return;
                }
                else
                {
                    int quantityToAdd = itemUI.InventoryItem.MaxStack - itemUI.InventoryItem.Quantity;
                    itemUI.InventoryItem.IncreaseQuantity(quantityToAdd);
                    itemUI.RefreshCount();
                    newItem.DecreaseQuantity(quantityToAdd);
                }

                AudioManager.Instance.PlaySFX("pop");

            }
        }

        for (int i = 0; i < inventorySlotsUI.Count; i++)
        {
            UI_InventorySlot slotUI = inventorySlotsUI[i];
            if (slotUI.transform.childCount == 0)
            {
                inventory.AddItemToInventory(newItem, slotUI.slotIndex);
                AddItemToInventoryUI(newItem, slotUI.slotIndex);

                AudioManager.Instance.PlaySFX("pop");
                ItemWorldManager.Instance.RemoveItemWorld(item, itemWorldControl);
                if (_inventoryManagerSO.selectedSlot == i) _inventoryManagerSO.RefreshCurrentHoldingItem();
                break;
            }
        }
        //if cant add because full inventory
        itemWorldControl.StartWaitForPickup(.1f);
        return;
    }

    public void DecreaseItemQuantity(int slotToDecrease)
    {

        UI_InventoryItem ui_InventoryItem = inventorySlotsUI[slotToDecrease].GetComponentInChildren<UI_InventoryItem>();
        ui_InventoryItem.InventoryItem.DecreaseQuantity(1);
        
        if (ui_InventoryItem.InventoryItem.Quantity <= 0)
        {
            Destroy(ui_InventoryItem.gameObject);
            _inventoryManagerSO.RemoveInventoryItem(ui_InventoryItem.InventoryItem);
        }
        else
            ui_InventoryItem.RefreshCount();
    }

    // this is onCraftingWindowClosed
    public void FindSlotAndPutItemInInventory(Component sender, object data) // find and put item from crafting slot back into inventory
    {
        UI_InventoryItem itemToAdd = sender as UI_InventoryItem;
        Inventory inventory = _inventoryManagerSO.inventory;
        InventoryItem itemToAddInfo = itemToAdd.InventoryItem;
        for (int i = 0; i < inventorySlotsUI.Count; i++)
        {
            UI_InventorySlot slotUI = inventorySlotsUI[i];
            UI_InventoryItem itemUI = slotUI.GetComponentInChildren<UI_InventoryItem>();

            if (itemUI != null &&
                itemUI.InventoryItem.Item == itemToAddInfo.Item &&
                itemUI.InventoryItem.Item.stackable &&
                itemUI.InventoryItem.Quantity < itemUI.InventoryItem.MaxStack)
            {
                if (itemUI.InventoryItem.Quantity + itemToAddInfo.Quantity <= itemUI.InventoryItem.MaxStack)
                {
                    itemUI.InventoryItem.IncreaseQuantity(itemToAddInfo.Quantity);
                    itemUI.RefreshCount();
                    _inventoryManagerSO.RemoveInventoryItem(itemToAddInfo);
                    Destroy(itemToAdd.gameObject);
                    return;
                }
                else
                {
                    int quantityToAdd = itemUI.InventoryItem.MaxStack - itemUI.InventoryItem.Quantity;
                    itemUI.InventoryItem.IncreaseQuantity(quantityToAdd);
                    itemUI.RefreshCount();
                    itemToAddInfo.DecreaseQuantity(quantityToAdd);
                    itemToAdd.RefreshCount();
                    
                }


            }
        }

        for (int i = 0; i < inventorySlotsUI.Count; i++)
        {
            UI_InventorySlot slotUI = inventorySlotsUI[i];
            if (slotUI.transform.childCount == 0)
            {
                inventory.AddItemToInventory(itemToAddInfo, slotUI.slotIndex);
                AddItemToInventoryUI(itemToAddInfo, slotUI.slotIndex);
                _inventoryManagerSO.RemoveInventoryItem(itemToAddInfo);
                Destroy(itemToAdd.gameObject);
                if (_inventoryManagerSO.selectedSlot == i) _inventoryManagerSO.RefreshCurrentHoldingItem();

                return;
            }
        }
        // neu ko thay slot nao add dc thi quang ra world
        ItemWorld itemToDropIntoWorld = itemToAddInfo.GetItemWorld();
        ItemWorldManager.Instance.DropItemIntoWorld(itemToDropIntoWorld, true, true);
        _inventoryManagerSO.RemoveInventoryItem(itemToAddInfo);
        Destroy(itemToAdd.gameObject);
        return;
    }

    public Transform FindEmptySlot()
    {
        for (int i = 0; i < inventorySlotsUI.Count; i++)
        {
            UI_InventorySlot slotUI = inventorySlotsUI[i];
            UI_InventoryItem itemUI = slotUI.GetComponentInChildren<UI_InventoryItem>();
            if (itemUI == null)
            {
                return slotUI.transform;
            }
        }
            return null;
    }

    //public void AddItemToSlot(Component sender, object data)
    //{
    //    InventoryItem newItem = _inventoryManagerSO.middleInventoryItem;
    //    int emptySlotIndex = (int)data;
    //    _inventoryManagerSO.inventory.AddItemToInventory(newItem, emptySlotIndex);
    //    AddItemToInventoryUI(newItem, emptySlotIndex);
    //}

}
