using System;
using UnityEngine;

public class ShopEvents
{
    public event Action<Item> onSelectedShopItem;
    public void SelectedShopItem(Item item)
    {
        onSelectedShopItem?.Invoke(item);
    }

    public event Action<Item> onBuyItem;
    public void BuyItem(Item item)
    {
        onBuyItem?.Invoke(item);
    }

    public event Action<Item> onSellItem;
    public void SellItem(Item item)
    {
        onSellItem?.Invoke(item);
    }

    public event Action onOpenShop;
    public void OpenShop()
    {
        onOpenShop?.Invoke();
    }
}
