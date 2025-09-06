using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    private Item item;
    [SerializeField] private Image itemImg;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI price;
    [SerializeField] private Button itemShopButton;

    private void Start()
    {
        itemShopButton.onClick.AddListener(SelectedItem);
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

    private void SelectedItem()
    {
        GameEventsManager.Instance.shopEvents.SelectedShopItem(item);
    }
}
