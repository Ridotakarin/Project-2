using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class TileTargeter : NetworkBehaviour
{
    #region Setup variables
    [SerializeField] PlayerController playerController;
    [SerializeField]
    private List<Tilemap> _tilemaps;
    
    [SerializeField]
    private Tilemap _targetTilemap;

    [SerializeField]
    private Tilemap _grassTilemap;
    [SerializeField]
    private Tilemap _groundTilemap;


    [Header("TARGET TILE SETTINGS")]
    [SerializeField]
    private AnimatedTile _targetTile;

    public int TargetRange = 1;

    private Vector3 _mouseWorldPosition;
    public Vector3 MouseWorldPosition
    {
        get { return _mouseWorldPosition; }
        set { _mouseWorldPosition = value; }
    }
    private Vector3Int _previousTilePos;
    private Vector3Int _mouseTilePosition;
    private Vector3Int _playerTilePosition;
    private Vector3Int _clampedTilePosition;
    private Vector3Int _lockedTilePosition;

    //[SerializeField] private List<Tilemap> tilemapCheck = new();
    [Header("HOE ON TILES SETTINGS")]
    [SerializeField] private bool _canHoe = false;
    public bool CanHoe
    {
        get { return _canHoe; }
        set { _canHoe = value; }
    }

    [SerializeField] private bool _lockedCanHoe = false;
    public bool LockedCanHoe
    {
        get { return _lockedCanHoe; }
        set { _lockedCanHoe = value; }
    }


    [Header("WATER ON TILES SETTINGS")]
    [SerializeField] private bool _canWater = false;
    public bool CanWater
    {
        get { return _canWater; }
        set { _canWater = value; }
    }

    [SerializeField] private bool _lockedCanWater = false;
    public bool LockedCanWater
    {
        get { return _lockedCanWater; }
        set { _lockedCanWater = value; }
    }

    [Header("PLANT ON TILES SETTINGS")]
    [SerializeField] private bool _canPlantGround = false;
    public bool CanPlantGround
    {
        get { return _canPlantGround; }
        set { _canPlantGround = value; }
    }


    [SerializeField]
    private float stepCooldown = 0.3f;
    private float stepTimer = 0f;
    #endregion

    #region Dependencies
    [SerializeField] private InventoryManagerSO _inventoryManagerSO;
    #endregion
    #region Before Gameloop
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }
        //if (SceneUtils.ThisSceneIsGameplayScene(SceneManager.GetActiveScene().ToString()))
        //    onExitToWorldScene(SceneManager.GetActiveScene().ToString());
    }


    private void OnEnable()
    {
        //SceneManager.sceneLoaded += OnSceneLoaded;
        GameEventsManager.Instance.dataEvents.onExitToWorldScene += onExitToWorldScene;
    }

    private void OnDisable()
    {
        //SceneManager.sceneLoaded -= OnSceneLoaded;
        GameEventsManager.Instance.dataEvents.onExitToWorldScene -= onExitToWorldScene;
    }
   

    private void Update()
    {
        CheckSoundWhenMoving();

        if (SceneUtils.ThisSceneIsNotGameplayScene(SceneManager.GetActiveScene().name)) return;

        SetTargetTile();
    }
    private void CheckSoundWhenMoving()
    {

        if (playerController.Movement != Vector2.zero && playerController.CanMove)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer > 0f) return;

            Vector3 playerPos = playerController.transform.position;

            Vector3Int cellPosOfGrass = _grassTilemap.WorldToCell(playerPos);
            Vector3Int cellPosOfGround = _groundTilemap.WorldToCell(playerPos);

            if (_grassTilemap.HasTile(cellPosOfGrass))
            {
                AudioManager.Instance.PlaySFX("walk_grass");
            }
            else if (_groundTilemap.HasTile(cellPosOfGround))
            {
                AudioManager.Instance.PlaySFX("walk_Dirt");
            }
            stepTimer = stepCooldown;
        }
        else
        {
            
            stepTimer = 0f;
        }
    }
    #endregion
     
    private void onExitToWorldScene(string sceneName)
    {
        StartCoroutine(WaitForSceneLoaded(sceneName));
    }

    IEnumerator WaitForSceneLoaded(string scene)
    {
        yield return new WaitUntil(() => SceneManager.GetSceneByName(scene).isLoaded);
        string currentSceneName = SceneManager.GetActiveScene().name;

        Debug.Log($"getalltiles");
        
        GetAllTilemaps();

        _targetTilemap = _tilemaps.LastOrDefault();
        
        foreach (var tilemap in _tilemaps)
        {
            if (tilemap.name == "Grass")
            {
                _grassTilemap = tilemap;
                break;
            }
        }
        foreach (var tilemap in _tilemaps)
        {
            if (tilemap.name == "Ground")
            {
                _groundTilemap = tilemap;
                break;
            }
        }
        Tilemap placeableObjectTilemap = null;
        foreach (var tilemap in _tilemaps)
        {
            if (tilemap.name == "PlaceableTile")
            {
                placeableObjectTilemap = tilemap;
                break;
            }
        }
    }

    void GetAllTilemaps()
    {
        GameObject gridObject = GameObject.Find("Grid");

        if (gridObject == null)
        {
            Debug.LogError("No Grid found in the scene!");
            return;
        }

        _tilemaps.Clear();
        Tilemap[] foundTilemaps = gridObject.GetComponentsInChildren<Tilemap>();

        _tilemaps.AddRange(foundTilemaps);
        TileDatabase.Instance.SetListTilemap(_tilemaps);
        playerController.waterTilemap = TileDatabase.Instance.GetTilemapByName("Water");
        PlaceObjectManager.Instance.SetTilemapForPlaceObject();
    }


    void SetTargetTile()
    {
        // Get mouse position in world coordinates
        _mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _mouseWorldPosition.z = 0; // Ensure it's on the correct plane

        // Convert mouse world position to tile position
        _mouseTilePosition = _targetTilemap.WorldToCell(_mouseWorldPosition);

        // Get player position in tile coordinates
        _playerTilePosition = _targetTilemap.WorldToCell(transform.position);

        // Ensure the highlight stays within 1 tile range of the player
        _clampedTilePosition = new Vector3Int(
            Mathf.Clamp(_mouseTilePosition.x, _playerTilePosition.x - TargetRange, _playerTilePosition.x + TargetRange),
            Mathf.Clamp(_mouseTilePosition.y, _playerTilePosition.y - TargetRange, _playerTilePosition.y + TargetRange),
            _mouseTilePosition.z
        );

        // Only update if tile position has changed
        if (_clampedTilePosition != _previousTilePos)
        {
            RefreshTilemapCheck(playerController.noTargetStates.Contains(playerController.CurrentState) ? false : true);
        }
    }

    public void RefreshTilemapCheck(bool showTarget)
    {
        //tilemapCheck.Clear();
        if (_targetTilemap == null)
        {
            Debug.Log("Target Tilemap is not set!");
            return;
        }
        _targetTilemap.SetTile(_previousTilePos, null); // Remove previous highlight

        //foreach (Tilemap tilemap in _tilemaps)
        //{
        //    if (tilemap.HasTile(_clampedTilePosition)) 
        //    {
        //        tilemapCheck.Add(tilemap);
        //    }
        //}

        if (showTarget)
        {
            _targetTilemap.SetTile(_clampedTilePosition, _targetTile); 
        }
        _previousTilePos = _clampedTilePosition;

        CheckTileIsValidTodoSomething();

    }
    private void CheckTileIsValidTodoSomething()
    {
        CanHoe = CheckCanHoe(_clampedTilePosition);
        CanWater = TileManager.Instance.HoedTilesNetwork.ContainsKey(new NetworkVector3Int(_clampedTilePosition)) && !TileManager.Instance.WateredTilesNetwork.ContainsKey(new NetworkVector3Int(_clampedTilePosition));
        CanPlantGround = TileManager.Instance.HoedTilesNetwork.ContainsKey(new NetworkVector3Int(_clampedTilePosition));
    }
    private bool CheckCanHoe(Vector3Int pos)
    {
        
        if (_grassTilemap == null)
        {
            Debug.LogWarning("tilemap to check can hoe not found.");
            return false;
        }
        if (_grassTilemap.HasTile(pos)) return false;

        return true;
    }

    public bool CheckHarverst()
    {
        return CropManager.Instance.TryToHarverst(_clampedTilePosition);
    }
    public void UseTool(bool changeFacingDirection)
    {
        if (changeFacingDirection)
        {
            ChangePlayerFacingDirection();
            RefreshTilemapCheck(changeFacingDirection);
            LockClampedPosition();
            LockedCanHoe = CanHoe;
            LockedCanWater = CanWater;
        }

    }

    public void LockClampedPosition()
    {
        _lockedTilePosition = _clampedTilePosition;
    }
    public void PlaceTile(Item item)
    {
        switch (item.itemName)
        {
            default:
                {
                    break;
                }
            case "Hoe":
                {
                    UseHoe(item);
                    break;
                }
            case "WaterCan":
                {
                    UseWaterCan(item);
                    break;
                }
            case "Pickaxe":
                {
                    break;
                }
        }
    }
    private void ChangePlayerFacingDirection()
    {
        if (_clampedTilePosition.x < _playerTilePosition.x)
        {
            playerController.LastMovement = Vector2.left;
            playerController.IsFacingRight = false;
        }
        else if (_clampedTilePosition.x > _playerTilePosition.x)
        {
            playerController.LastMovement = Vector2.right;
            playerController.IsFacingRight = true;
        }

        if (_clampedTilePosition.y > _playerTilePosition.y)
        {
            playerController.LastMovement = Vector2.up;
        }
        else if (_clampedTilePosition.y < _playerTilePosition.y)
        {
            playerController.LastMovement = Vector2.down;
        }
    }

    private void UseHoe(Item item)
    {
        AudioManager.Instance.PlaySFX("shovel");
        if (LockedCanHoe)
        {
            Tilemap targetTilemap = null;
            foreach (Tilemap tilemap in _tilemaps)
            {
                if(tilemap.name == item.tilemap.name)
                {
                    targetTilemap = tilemap;
                    break;
                }
                    
                
            }
            if (!TileManager.Instance.HoedTilesNetwork.ContainsKey(new NetworkVector3Int(_lockedTilePosition)))
            {
                TileManager.Instance.ModifyTile(_lockedTilePosition, targetTilemap.name, item.ruleTile.name);
            }
            else if(CropManager.Instance.PlantedCropsNetwork.ContainsKey(new NetworkVector3Int(_lockedTilePosition)) &&
                CropManager.Instance.PlantedCropsNetwork[new NetworkVector3Int(_lockedTilePosition)].IsDead)
            {
                CropManager.Instance.RemoveCrop(_lockedTilePosition);
            }


        }
    }

    private void UseWaterCan(Item item)
    {
        AudioManager.Instance.PlaySFX("waterCan");
        if (LockedCanWater)
        {
            Tilemap targetTilemap = null;
            foreach (Tilemap tilemap in _tilemaps)
            {
                if (tilemap.name == item.tilemap.name)
                {
                    targetTilemap = tilemap;
                    break;
                }
                    
            }
            if (!TileManager.Instance.WateredTilesNetwork.ContainsKey(new NetworkVector3Int(_lockedTilePosition)))
            {
                TileManager.Instance.ModifyTile(_lockedTilePosition, targetTilemap.name, item.ruleTile.name);

            }
            else
            {
            }
                
        }
        else
        {
        }
    }

    public void SetTile(Item item)
    {
        switch(item.type)
        {
            default:
                break;

            case ItemType.Crop:
                {
                    if (!CanPlantGround) return;
                    GameObject.Find("CropManager").GetComponent<CropManager>().TryModifyCrop(_clampedTilePosition,item.itemName,1);
                    _inventoryManagerSO.DecreaseItemQuantityOnUse();
                    playerController.CheckAnimation();
                    break;
                }  
            case ItemType.Tile:
                {
                    if (!PlaceObjectManager.Instance.CanPlaceObject) return;
                    PlaceObjectManager.Instance.PlaceTile(item.ruleTile);
                    _inventoryManagerSO.DecreaseItemQuantityOnUse();
                    playerController.CheckAnimation();
                    break;
                }
        }
    }

    public bool CanBreakPlacedTile()
    {
        if (PlaceObjectManager.Instance.PlacedTile.ContainsKey(_clampedTilePosition))
        {
            PlaceObjectManager.Instance.BreakPlacedTile(_clampedTilePosition);
            return true;
        }
        return false;
    }

}
