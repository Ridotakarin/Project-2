using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryController : NetworkBehaviour, IDataPersistence
{
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private InventoryManagerSO _inventoryManagerSO;

    // GameEvent
    [SerializeField] private GameEvent onOpenInventory;
    [SerializeField] private GameEvent onCloseInventory;
    [SerializeField] private GameEvent onChangeSelectedSlot;
    [SerializeField] private GameEvent onInventoryLoad;

    private void Start()
    {
        if (!IsOwner) enabled = false;
    }

    private void OnEnable()
    {

        _inputReader.playerActions.changeInventorySlotEvent += GetInputValueToChangeSlot;
        _inputReader.playerActions.openInventoryEvent += OpenInventory;
        _inputReader.uiActions.closeInventoryEvent += CloseInventory;
        GameEventsManager.Instance.inventoryEvents.onItemAdded += AddItem;
        GameEventsManager.Instance.inventoryEvents.onRemoveItem += RemoveItem;
    }

    private void OnDisable()
    {
        _inputReader.playerActions.changeInventorySlotEvent -= GetInputValueToChangeSlot;
        _inputReader.playerActions.openInventoryEvent -= OpenInventory;
        _inputReader.uiActions.closeInventoryEvent -= CloseInventory;
        GameEventsManager.Instance.inventoryEvents.onItemAdded -= AddItem;
        GameEventsManager.Instance.inventoryEvents.onRemoveItem -= RemoveItem;

    }
    private void OpenInventory()
    {
        onOpenInventory.Raise(this, ActionMap.UI);
    }
    private void CloseInventory()
    {
        onCloseInventory.Raise(this, ActionMap.Player);
    }

    public void SwitchActionMap(Component sender, object data)
    {
        ActionMap map = (ActionMap)data;
        _inputReader.SwitchActionMap(map);
        if(map == ActionMap.UI)
        {
            _inventoryManagerSO.isOpeningInventory = true;
        }
        else
        {
            _inventoryManagerSO.isOpeningInventory = false;
        }
    }
    private void GetInputValueToChangeSlot(int value, bool isKeyboard)
    {

        if (isKeyboard)
        {

            if (value != _inventoryManagerSO.selectedSlot)
            {
                onChangeSelectedSlot.Raise(this, value); // gui duy nhat newValue thoi
                
            }
            
        }
        else
        {
            int newValue = _inventoryManagerSO.selectedSlot + value;
            if (newValue > 8) newValue = 0;
            else if (newValue < 0) newValue = 8;
            
            onChangeSelectedSlot.Raise(this, newValue);
        }
    }

    private void AddItem(string itemName)
    {
        Item item = ItemDatabase.Instance.GetItemByName(itemName);
        if (item == null)
        {
            Debug.LogWarning("Item not found in database: " + itemName);
            return;
        }
        int slotIndex = GetEmptySlot();
        if (slotIndex == -1)
        {
            Debug.LogWarning("No empty slot available in inventory.");
            return;
        }
        InventoryItem inventoryItem = new InventoryItem(null, item, slotIndex);
        _inventoryManagerSO.inventory.AddItemToInventory(inventoryItem, slotIndex);
        GameEventsManager.Instance.inventoryEvents.AddItemToUI(inventoryItem, slotIndex);
    }

    public void RemoveItem(string itemName, int amout)
    {
        InventoryItem inventoryItem = _inventoryManagerSO.inventory.GetInventoryItemFromName(itemName);
        if (inventoryItem == null)
        {
            Debug.Log("null");
            return;
        }
        
        if (inventoryItem.Quantity >=  amout) 
        {
            inventoryItem.DecreaseQuantity(amout);

            int slotIndex = _inventoryManagerSO.inventory.GetIndexOfInventory(inventoryItem);

            if (inventoryItem.Quantity < 0 || inventoryItem.Quantity == 0)
            {
                _inventoryManagerSO.inventory.RemoveInventoryItem(inventoryItem);
                GameEventsManager.Instance.inventoryEvents.OnItemRemoved(slotIndex);
                return;
            }

            GameEventsManager.Instance.inventoryEvents.UpDateAmount(slotIndex);
        }
    }

    private int GetEmptySlot()
    {
        for (int i = 0; i < _inventoryManagerSO.inventory.MaxSlotInventory; i++)
        {
            if (_inventoryManagerSO.inventory.GetInventoryItemOfIndex(i) == null)
            {
                return i;
            }
        }
        return -1;
    }

    public void UseItem(Component sender, object data)
    {
        Item item = _inventoryManagerSO.GetCurrentItem();

        if (item == null) return;

        if (item.type != ItemType.Usable) return;

        switch (item.itemName)
        {
            case "Letter":
                GameEventsManager.Instance.uiEvents.OpenLetter();
                GameEventsManager.Instance.inputReader.SwitchActionMap(ActionMap.UI);
                break;
            case "Key":
                GameEventsManager.Instance.playerHouseEvents.UnlockHouse();
                Debug.Log("Player house unlocked!");
                break;
            default :
                return;
        }
    }

    public void LoadData(GameData data)
    {
        if (SceneManager.GetActiveScene().name == Loader.Scene.WorldScene.ToString() ||
            SceneManager.GetActiveScene().name == Loader.Scene.CutScene.ToString())
        {
            if (_inventoryManagerSO.hasLoad) return;

            _inventoryManagerSO.ResetInventorySO();

            _inventoryManagerSO.inventory = data.InventoryData;
            Debug.Log("Load Inventory Data: " + _inventoryManagerSO.inventory.InventoryItemList.Count);
            ItemDatabase.Instance.SetItem(_inventoryManagerSO.inventory.InventoryItemList);
            //StartCoroutine(WaitForUIInventoryAwaked());

            if (!_inventoryManagerSO.hasLoad) _inventoryManagerSO.hasLoad = true;
        }
    }

    //private IEnumerator WaitForUIInventoryAwaked()
    //{
    //    yield return new WaitUntil(() => SceneManager.GetSceneByName("UIScene").isLoaded);
    //    onInventoryLoad.Raise(this, null);
    //    _inventoryManagerSO.RefreshCurrentHoldingItem();
    //    onChangeSelectedSlot.Raise(this, _inventoryManagerSO.selectedSlot);
    //    Debug.Log("did load ui inven");
    //}

    public void SaveData(ref GameData gameData)
    {
        gameData.SetInventoryData(_inventoryManagerSO.inventory);

        _inventoryManagerSO.ResetInventorySO();
    }
}
