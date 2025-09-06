using System;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryManagerSO", menuName = "ScriptableObject/SOManager/InventoryManagerSO")]
public class InventoryManagerSO : ScriptableObject
{
    public Inventory inventory;
    public bool hasLoad = false;

    public event Func<Transform> onFindEmptySlot;
    public event Action onChangedSelectedSlot;
    public event Action<bool> onShowPlaceableObject;
    public event Action<int> onDecreaseItemQuantity;
    public event Action<InventoryItem, GameObject> onPutItemDownByRightClick;
    private int _selectedSlot = 0;
    public int selectedSlot
    {
        get => _selectedSlot;
        set
        {
            _selectedSlot = value;
            RefreshCurrentHoldingItem();
        }
    }

    public UI_InventoryItem currentDraggingItem = null;
    private bool _isOpeningInventory = false;
    public bool isOpeningInventory
    {
        get => _isOpeningInventory;
        set
        {
            _isOpeningInventory = value;
        }
    }

    private bool _isPointerOverUI;
    public bool IsPointerOverUI
    {
        get => _isPointerOverUI;
        set
        {
            _isPointerOverUI = value;
        }
    }

    public Item GetCurrentItem()
    {
        InventoryItem item = inventory.GetInventoryItemOfIndex(_selectedSlot);
        if (item != null)
        {
            Debug.Log($"GetCurrentItem: {item.Item.itemName}");
            return item.Item;
        }
        return null;
    }

    public InventoryItem GetItemInSlot(int index)
    {
        return inventory.FindItemInInventory(index);
    }

    public void RefreshCurrentHoldingItem()
    {
        onChangedSelectedSlot?.Invoke();
    }

    public void RemoveInventoryItem(InventoryItem item)
    {
        inventory.RemoveInventoryItem(item);
    }

    public Transform FindEmptySlot()
    {
        Transform emptySlot = onFindEmptySlot.Invoke();

        return emptySlot;
    }

    public void PutItemDownByRightClick(InventoryItem item, int slotIndex, GameObject slot) // for mouse interact on item
    {
        onPutItemDownByRightClick?.Invoke(item, slot);
        inventory.AddItemToInventory(item, slotIndex);
    }

    public void DecreaseItemQuantityOnUse()
    {
        onDecreaseItemQuantity?.Invoke(_selectedSlot);
    }

    public void ShowPlaceableObject(bool isActivate)
    {
        onShowPlaceableObject?.Invoke(isActivate);
    }
    public void ResetInventorySO()
    { 
        inventory = new Inventory();
        hasLoad = false;
        _selectedSlot = 0;
        currentDraggingItem = null;
        _isOpeningInventory = false;
        _isPointerOverUI = false;
    }
}
