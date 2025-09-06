using Ink.Runtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShop : Singleton<UIShop>
{
    [SerializeField] private List<ShopData> shopData;
    [SerializeField] private GameObject shopContainer;
    [SerializeField] private GameObject shopBoxContainer;
    [SerializeField] private GameObject shopItemPreb;
    [SerializeField] private GameObject itemDetailsContainer;
    [SerializeField] private GameObject itemSellBoxContainer;
    [SerializeField] private Button closeButton;

    public bool IsShopOpen => shopContainer.activeSelf;

    private void OnEnable()
    {
        GameEventsManager.Instance.enviromentStatusEvents.onChangeSeason += UpdateShop;
        GameEventsManager.Instance.shopEvents.onSelectedShopItem += OnSelectedItem;
        GameEventsManager.Instance.shopEvents.onOpenShop += OpenShop;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.enviromentStatusEvents.onChangeSeason -= UpdateShop;
        GameEventsManager.Instance.shopEvents.onSelectedShopItem -= OnSelectedItem;
        GameEventsManager.Instance.shopEvents.onOpenShop -= OpenShop;
    }

    private void Start()
    {
        UpdateShop(EnviromentalStatusManager.Instance.eStatus.SeasonStatus);
        itemSellBoxContainer.gameObject.SetActive(true);
        itemDetailsContainer.gameObject.SetActive(false);
        closeButton.onClick.AddListener(CloseShop);
    }

    private void UpdateShop(ESeason season)
    {
        ClearShop();
        foreach (var item in shopData[((int)season)].items)
        {
            if (item != null)
            {
                ShopItem shopItem = Instantiate(shopItemPreb, shopBoxContainer.transform).GetComponent<ShopItem>();
                shopItem.InitItem(item);
            }
        }
    }

    private void ClearShop()
    {
        ShopItem[] children = shopBoxContainer.GetComponentsInChildren<ShopItem>();
        if (children == null || children.Length == 0) return;
        foreach (var child in children)
        {
            Destroy(child.gameObject);
        }
    }

    private void OnSelectedItem(Item item)
    {
        itemDetailsContainer.SetActive(true);
        itemSellBoxContainer.SetActive(false);

        ShopItemDetails shopItemDetails = itemDetailsContainer.GetComponent<ShopItemDetails>();
        shopItemDetails.InitItem(item);
    }

    private void OpenShop()
    {
        shopContainer.SetActive(true);
    }

    private void CloseShop()
    {
        shopContainer.SetActive(false);
        GameEventsManager.Instance.inputReader.SwitchActionMap(ActionMap.Player);
    }
}
