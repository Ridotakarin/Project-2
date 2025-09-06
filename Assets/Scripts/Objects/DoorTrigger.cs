using UnityEngine;

public enum ELayerSorting
{
    Player,
    Leaf,
    Rimlit,
}

[RequireComponent(typeof(SpriteRenderer),typeof(BoxCollider2D))]
public class DoorTrigger : MonoBehaviour
{
    public enum ETagTrigger
    {
        Player,
        Villager,
    }

    public ETagTrigger tagTrigger;
    public SpriteRenderer doorSprite;
    public Sprite openDoorSprite;
    public Sprite closedDoorSprite;

    public ELayerSorting upperLayer;
    public ELayerSorting lowerLayer;

    private BoxCollider2D boxCollider;

    public BoxCollider2D areaDestroy;

    private bool isOpen = false;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(tagTrigger.ToString()) && !isOpen)
        {
            OpenDoor();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(tagTrigger.ToString()) && isOpen)
        {
            CloseDoor();
            if (areaDestroy != null) areaDestroy.enabled = true;
        }
    }

    private void OpenDoor()
    {
        doorSprite.sprite = openDoorSprite;
        doorSprite.sortingLayerName = lowerLayer.ToString();
        isOpen = true;
    }

    private void CloseDoor()
    {
        doorSprite.sprite = closedDoorSprite;
        doorSprite.sortingLayerName = upperLayer.ToString();
        isOpen = false;
    }
}
