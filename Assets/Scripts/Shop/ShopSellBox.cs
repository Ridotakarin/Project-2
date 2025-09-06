using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopSellBox : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private int slotIndex = -2;
    [SerializeField] private InventoryManagerSO _inventoryManagerSO;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button sellButton;
    [SerializeField] private GameObject sellBoxContainer;

    private InventoryItem _inventoryItem;
    private UI_InventoryItem _draggedItem;
    private int totalPrice = 0;

    private void Start()
    {
        sellButton.onClick.AddListener(SellItem);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_inventoryManagerSO.currentDraggingItem == null) return;
        _draggedItem = _inventoryManagerSO.currentDraggingItem;
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            DropItemOnInventorySlot(_draggedItem);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            DropOneOfDraggingItem(_draggedItem);
        }
    }

    public void DropItemOnInventorySlot(UI_InventoryItem draggedItem)
    {
        draggedItem.parentAfterDrag = sellBoxContainer.transform;
        draggedItem.OnItemFinishDrag();
        UpdatePrice(draggedItem);
    }

    public void DropOneOfDraggingItem(UI_InventoryItem draggedItem)
    {
        if (draggedItem.InventoryItem.Quantity == 1)
        {
            DropItemOnInventorySlot(draggedItem);
            return;
        }
        int ogQuantity = draggedItem.InventoryItem.Quantity;
        draggedItem.InventoryItem.SetQuantity(ogQuantity - 1);
        draggedItem.RefreshCount();
        InventoryItem newItem = new InventoryItem(System.Guid.NewGuid().ToString(), draggedItem.InventoryItem.Item, slotIndex, 1, draggedItem.InventoryItem.Level);
        _inventoryManagerSO.PutItemDownByRightClick(newItem, newItem.SlotIndex, sellBoxContainer.gameObject);
        UpdatePrice(draggedItem);
    }

    private void UpdatePrice(UI_InventoryItem dragItem)
    {
        totalPrice = 0;

        if (dragItem == null || dragItem.InventoryItem == null || dragItem.InventoryItem.Item == null ||
            dragItem.InventoryItem.Item.type != ItemType.Food || dragItem.InventoryItem.Item.itemPrice <= 0) return;
        
        totalPrice = dragItem.InventoryItem.Quantity * (dragItem.InventoryItem.Item.itemPrice + dragItem.InventoryItem.Item.itemPrice * 20/100);
        priceText.text = totalPrice.ToString();
        _inventoryItem = dragItem.InventoryItem;
    }

    private void SellItem()
    {
        if (_inventoryItem == null || _inventoryItem.Item == null || totalPrice <= 0) return;
        
        _inventoryManagerSO.RemoveInventoryItem(_inventoryItem);
        GameEventsManager.Instance.inventoryEvents.RemoveItem(_inventoryItem.Item.itemName, _inventoryItem.Quantity);
        GameEventsManager.Instance.goldEvents.GoldGained(totalPrice);
        Destroy(_draggedItem.gameObject);
        RefreshBox();
    }

    private void RefreshBox()
    {
        priceText.text = "0";
    }
}
