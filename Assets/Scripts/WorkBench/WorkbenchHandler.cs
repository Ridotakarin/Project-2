using UnityEngine;

public class WorkbenchHandler : MonoBehaviour
{
    public enum WorkbenchType
    {
        Crafting,
        Smelting,
        Enchanting,
        ArmorMaking,
    }

    [SerializeField] private WorkbenchType workbenchType;
    [SerializeField] private GameObject emoteGO;
    private bool playerInRange = false;

    private void Start()
    {
        if (emoteGO != null)
        {
            emoteGO.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Emote GameObject is not assigned.");
        }
    }

    public void OpenWorkbenchSystem()
    {
        if (!playerInRange) return;

        switch (workbenchType)
        {
            case WorkbenchType.Crafting:
                // Open the crafting panel
                UIWorkbenchPanel.Instance.OpenCraftingPanel();
                break;
            case WorkbenchType.Smelting:
                break;
            case WorkbenchType.Enchanting:
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            emoteGO.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            emoteGO.SetActive(false);
        }
    }
}
