using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemDetails : MonoBehaviour
{
    private Item item;
    [SerializeField] private Image itemImg;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI price;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Button buyItemButton;
    [SerializeField] private Button closeButton;

    private void Start()
    {
        buyItemButton.onClick.AddListener(BuyItem);
        closeButton.onClick.AddListener(() => gameObject.SetActive(false));
    }

    public void InitItem(Item item)
    {
        if (item != null)
        {
            this.item = item;

            if (item.image != null)
            {
                itemImg.sprite = item.image;
            }

            if (item.name != null)
            {
                itemName.text = item.name;
            }

            if (item.itemPrice != 0)
            {
                price.text = item.itemPrice.ToString();
            }
        }
    }

    private void BuyItem()
    {
        if (GoldManager.Instance.currentGold < item.itemPrice) return;

        GameEventsManager.Instance.inventoryEvents.AddItem(item.itemName);
        GameEventsManager.Instance.goldEvents.GoldLost(item.itemPrice);
    }
}
