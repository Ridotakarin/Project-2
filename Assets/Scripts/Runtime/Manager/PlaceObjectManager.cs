using UnityEngine;
using UnityEngine.Tilemaps;

public class PlaceObjectManager : PersistentSingleton<PlaceObjectManager>, IDataPersistence
{
    private SerializableDictionary<Vector3Int, string> _placedTile = new SerializableDictionary<Vector3Int, string>();
    public SerializableDictionary<Vector3Int, string> PlacedTile => _placedTile;

    [SerializeField]
    private Tilemap _groundTilemap;
    [SerializeField]
    private Tilemap _placeObjectTilemap;
    [SerializeField]
    private Tilemap _hoedTilemap;
    [SerializeField]
    private Tilemap _wateredTilemap;
    [SerializeField]
    private Tilemap _cropTilemap;
    [SerializeField]
    private Tilemap _waterTilemap;
    [SerializeField] private Transform _previewObject;
    [SerializeField] private SpriteRenderer _previewObjectSpriteRenderer;
    [SerializeField] private float tileSize = 1f;

    private Vector3Int _lastCellPosition;

    [SerializeField]
    private bool _isActivated;

    [SerializeField]
    private bool _canPlaceObject = true;
    public bool CanPlaceObject
    {
        get => _canPlaceObject;
        private set
        {
            _canPlaceObject = value;

            Color color;
            if (value)
            {
                color = Color.green;
            }else
                color = Color.red;

            color.a = 0.5f; // Set 50% opacity

            _previewObjectSpriteRenderer.color = color;
        }
    }

    [SerializeField] private InventoryManagerSO _inventoryManagerSO;
    private void OnEnable()
    {
        _inventoryManagerSO.onShowPlaceableObject += ActivatePlaceableObjectUI;
    }   

    private void OnDisable()
    {
        _inventoryManagerSO.onShowPlaceableObject -= ActivatePlaceableObjectUI;
    }
    protected override void Awake()
    {
        base.Awake();
        _previewObject = GameObject.Find("PlaceableObjectPreview").GetComponent<Transform>();
        _previewObjectSpriteRenderer = _previewObject.GetComponent<SpriteRenderer>();
        _previewObject.gameObject.SetActive(false);
    }
    public void SetTilemapForPlaceObject()
    {
        _groundTilemap = TileDatabase.Instance.GetTilemapByName("Ground");
        _hoedTilemap = TileDatabase.Instance.GetTilemapByName("FarmGround");
        _wateredTilemap = TileDatabase.Instance.GetTilemapByName("WateredGround");
        _cropTilemap = TileDatabase.Instance.GetTilemapByName("CropGround");
        _waterTilemap = TileDatabase.Instance.GetTilemapByName("Water");

        _placeObjectTilemap = TileDatabase.Instance.GetTilemapByName("PlaceableTile");

    }
    private void Update()
    {
        if (!_isActivated) return;

        // 1. Get current mouse world position
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldMousePos.z = 0f;

        // 2. Convert to tilemap cell position
        Vector3Int currentCell = _placeObjectTilemap.WorldToCell(worldMousePos);

        // 3. Only continue if cell changed
        if (currentCell == _lastCellPosition) return;
        _lastCellPosition = currentCell;

        // 4. Get the center of that tile cell
        Vector3 cellCenterWorldPos = _placeObjectTilemap.GetCellCenterWorld(currentCell);

        // 5. Move the preview object to this position
        _previewObject.position = cellCenterWorldPos;

        // 6. Optionally: Validate placement
        CheckIsValidToPlace();
    }



    private void CheckIsValidToPlace()
    {
        if(_placeObjectTilemap.HasTile(_lastCellPosition) ||
            _hoedTilemap.HasTile(_lastCellPosition) ||
            _wateredTilemap.HasTile(_lastCellPosition) ||
            _cropTilemap.HasTile(_lastCellPosition) ||
            _waterTilemap.HasTile(_lastCellPosition))
        {
            CanPlaceObject = false;
        }
        else
        {
            CanPlaceObject = true;
        }

    }

    private void ActivatePlaceableObjectUI(bool isActivate)
    {
        _isActivated = isActivate;

        var spriteRenderer = _previewObject.GetComponent<SpriteRenderer>();

        if (_isActivated)
        {
            _previewObject.gameObject.SetActive(true);
            Sprite itemSprite = _inventoryManagerSO.GetCurrentItem().image;

            spriteRenderer.sprite = itemSprite;
            float unitsPerPixel = 1f / itemSprite.pixelsPerUnit;
            Vector2 spriteSize = itemSprite.rect.size * unitsPerPixel;
            _previewObject.localScale = new Vector3(1f / spriteSize.x, 1f / spriteSize.y, 1f); // Scale it to fit 1x1 tile
        }
        else
        {
            _previewObject.gameObject.SetActive(false);
            spriteRenderer.sprite = null;
        }
    }

    public string GetTileBaseNameByPosition(Vector3Int position)
    {
        if (_placedTile.TryGetValue(position, out string tileName))
        {
            return tileName;
        }
        return null;
    }

    public void PlaceTile(TileBase tileToSet)
    {
        AudioManager.Instance.PlaySFX("Pickaxe3");
        _placeObjectTilemap.SetTile(_lastCellPosition, tileToSet);
        _placedTile[_lastCellPosition] = tileToSet.name;
        CheckIsValidToPlace();
    }

    public void BreakPlacedTile(Vector3Int pos)
    {
        string tileName = GetTileBaseNameByPosition(pos);
        tileName = tileName.Replace(" ", "_");
        Item item = ItemDatabase.Instance.GetItemByName(tileName);

        ItemWorld itemWorld = new ItemWorld
        (
            System.Guid.NewGuid().ToString(),
            item,
            1,
            _placeObjectTilemap.GetCellCenterWorld(pos),
            0
        );
        ItemWorldManager.Instance.DropItemIntoWorld(itemWorld,true,false);

        AudioManager.Instance.PlaySFX("Pickaxe3");
        _placeObjectTilemap.SetTile(pos, null);
        _placedTile.Remove(pos);
        CheckIsValidToPlace();
    }

    public void LoadData(GameData data)
    {
        _placedTile = data.PlacedTileData._placedTile;
        if (_placedTile == null || _placedTile.Count == 0) return;
        SetTileOnLoadData();
    }

    private void SetTileOnLoadData()
    {
        foreach (var placedTile in _placedTile)
        {
            Vector3Int position = placedTile.Key;
            string tileName = placedTile.Value;

            TileBase tileToSet = TileDatabase.Instance.GetTileByName(tileName);
            if (tileToSet != null)
            {
                _placeObjectTilemap.SetTile(position, tileToSet);
            }
        }
    }

    public void SaveData(ref GameData data)
    {
        PlacedTileData placedTileData = new PlacedTileData(_placedTile);
        data.SetPlacedTileData(placedTileData);
    }
}
