using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor.PackageManager;
#endif
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerController : NetworkBehaviour, IDataPersistence
{
    public static PlayerController LocalInstance { get; private set; }

    [SerializeField]
    private List<AudioClip> hurtSounds = new List<AudioClip>();

    private bool isPlayerLoadedData = false;

    public Tilemap waterTilemap;
    public bool CanFish
    {
        get => CanFish;
        set
        {
            animator.SetBool("CanFish", value);
            if (value)
                ChooseFishingTime();
        }
    }

    public bool IsFishing = false;

    public bool IsHookingFish = false;

    [SerializeField]
    [Range(5f, 7f)]
    private float fishingTimeSetting = 5;
    [SerializeField]
    private float chosenFishingTime;

    [SerializeField]
    private float fishingTimer = 0f;
    #region Setup Everything
    #region Components
    [Header("Components")]
    [SerializeField] private TileTargeter tileTargeter;
    [SerializeField] private InventoryController _inventoryController;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private TextMeshPro playerNameText;
    [SerializeField] private Damageable _damageable;
    #endregion

    #region Reference
    [Header("Reference")]
    public PlayerDataSO playerDataSO;
    public PlayerDataNetwork playerDataNetwork;


    [Header("Other Data")]
    public string otherPlayerName;
    public int otherCharacterId;
    #endregion

    #region PlayerStatus
    [Header("Player Status")]
    [SerializeField] private FloatVariable _playerMana;
    [SerializeField] private FloatVariable _playerStamina;
    [SerializeField] private float _staminaDrainRate;
    [SerializeField] private float _acceleration;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float _vehicleSpeed;

    public float VehicleSpeed
    {
        get { return _vehicleSpeed; }
        set { _vehicleSpeed = value; }
    }
    private string _currentState;
    public string CurrentState
    {
        get { return _currentState; }
        set { _currentState = value; }
    }
    public string[] noTargetStates;
    public string[] toolsAndWeapon;

    //[SerializeField]
    //private bool _isHurting = false;
    //public bool IsHurting 
    //{
    //    get { return _isHurting; }
    //    set { _isHurting = value; }
    //}

    [SerializeField]
    private bool _canMove = true;
    public bool CanMove
    {
        get { return _canMove; }
        set
        {
            _canMove = value;
        }
    }

    [SerializeField]
    private float _currentSpeed;
    public float CurrentSpeed
    {
        get
        {
            return _currentSpeed = CanMove ? IsRidingVehicle ? _vehicleSpeed : IsRunning ? runSpeed : walkSpeed : 0;
        }
    }
    [SerializeField]
    private Vector2 _movement;
    public Vector2 Movement
    {
        get { return _movement; }
        set { _movement = value; }
    }
    private Vector2 _lastMovement;
    public Vector2 LastMovement // Keep the last animation
    {
        get { return _lastMovement; }
        set
        {
            _lastMovement = value;
            animator.SetFloat("Horizontal", Mathf.Abs(_lastMovement.x));
            animator.SetFloat("Vertical", _lastMovement.y);
        }

    }
    [SerializeField]
    private bool _isFacingRight = true;
    public bool IsFacingRight
    {
        get { return _isFacingRight; }
        set
        {

            _isFacingRight = value;
            transform.localScale = _isFacingRight ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
        }
    }

    private bool _canRun = true;
    public bool CanRun
    {
        get { return _canRun; }
        private set { _canRun = value; }
    }

    private bool _isRunning = false;
    public bool IsRunning
    {
        get { return _isRunning; }
        private set
        {
            _isRunning = value;
            animator.SetBool("IsRunning", _isRunning);
        }
    }

    private bool _canRide = false;
    public bool CanRide
    {
        get { return _canRide; }
        private set { _canRide = value; }
    }


    private bool _isRidingVehicle = false;
    public bool IsRidingVehicle
    {
        get { return _isRidingVehicle; }
        private set
        {
            _isRidingVehicle = value;
            if (value)
            {
                if (CurrentVehicle.tag == "Bicycle")
                    animator.SetBool("UseDevice", true);

                else if (CurrentVehicle.tag == "Horse")
                    animator.SetBool("UseHorse", true);
            }
            else
            {
                animator.SetBool("UseDevice", false);
                animator.SetBool("UseHorse", false);
            }

        }
    }

    private bool _isHoldingItem = false;
    public bool IsHoldingItem
    {
        get { return _isHoldingItem; }
        private set
        {
            _isHoldingItem = value;
        }
    }

    private bool _canAttack = true;
    public bool CanAttack
    {
        get { return _canAttack; }
        set { _canAttack = value; }
    }

    [SerializeField]
    private bool _canSleep = false;
    public bool CanSleep
    {
        get { return _canSleep; }
        private set { _canSleep = value; }
    }

    private bool _isSleeping = false;
    public bool IsSleeping
    {
        get { return _isSleeping; }
        private set { _isSleeping = value; }
    }

    [SerializeField]
    private bool _hadTarget;
    public bool HadTarget
    {
        get { return _hadTarget; }
        private set { _hadTarget = value; }
    }
    #endregion

    #region Dependencies Scripts
    [Header("Dependencies")]
    public InputReader _inputReader;
    //private PlayerData player;

    [SerializeField]
    private VehicleController _currentVehicle;
    public VehicleController CurrentVehicle
    {
        get { return _currentVehicle; }
        private set
        {
            _currentVehicle = value;
            if (_currentVehicle == null)
            {
                HadTarget = false;
                CanRide = false;
            }
            else
            {
                HadTarget = true;
                CanRide = true;
            }
        }
    }



    private BedScript _currentBed;
    public BedScript CurrentBed
    {
        get { return _currentBed; }
        private set
        {
            _currentBed = value;
            if (_currentBed == null)
            {
                HadTarget = false;
                CanSleep = false;
            }
            else
            {
                HadTarget = true;
                CanSleep = true;
            }
        }
    }

    [SerializeField]
    private ItemOnHand _itemOnHand;

    [SerializeField] private InventoryManagerSO _inventoryManagerSO;
    #endregion

    #region Game Events
    [SerializeField] private GameEvent onPlayerLoadEvent;
    [SerializeField] private GameEvent onPlayerSaveEvent;
    [SerializeField] private GameEvent onSubmit;
    #endregion
    #endregion

    #region Setup Before Game Start
    private void Awake()
    {
        _inventoryController = GetComponent<InventoryController>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        _damageable = GetComponent<Damageable>();

    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            _inputReader.playerActions.moveEvent += OnMove;
            _inputReader.playerActions.attackEvent += OnAttack;
            _inputReader.playerActions.interactEvent += OnInteract;
            _inputReader.playerActions.secondInteractEvent += OnSecondInteract;
            _inputReader.playerActions.runEvent += OnRun;
            _inputReader.playerActions.submitEvent += OnSubmit;
            _inventoryManagerSO.onChangedSelectedSlot += CheckAnimation;

            LocalInstance = this;
            DontDestroyOnLoad(gameObject);

            DataPersistenceManager.Instance.LoadGame();

            StartCoroutine(WaitForLoadedData());
        }
    }

    private IEnumerator WaitForLoadedData()
    {
        yield return new WaitUntil(() => isPlayerLoadedData);
        playerDataNetwork = new PlayerDataNetwork(NetworkManager.Singleton.LocalClientId,
                                                      playerDataSO.characterId,
                                                      playerDataSO.playerName,
                                                      playerDataSO.playerId);
        this.transform.position = playerDataSO.position;
        playerNameText.text = playerDataSO.playerName.ToString();
        animator.runtimeAnimatorController = GameMultiplayerManager.Instance.GetCharactersAnimator(playerDataSO.characterId);

        GameEventsManager.Instance.playerEvents.OnPlayerSpawned(this);
        StartCoroutine(WaitForNetworkControllerSpawn());
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        NetworkManager.Singleton.SceneManager.ActiveSceneSynchronizationEnabled = false;
        NetworkManager.Singleton.SceneManager.SetClientSynchronizationMode(UnityEngine.SceneManagement.LoadSceneMode.Single);

        _inputReader.playerActions.moveEvent -= OnMove;
        _inputReader.playerActions.attackEvent -= OnAttack;
        _inputReader.playerActions.interactEvent -= OnInteract;
        _inputReader.playerActions.secondInteractEvent -= OnSecondInteract;
        _inputReader.playerActions.runEvent -= OnRun;
        _inputReader.playerActions.submitEvent -= OnSubmit;
        _inventoryManagerSO.onChangedSelectedSlot -= CheckAnimation;

        DataPersistenceManager.Instance.SaveGame();

        bool isHost = NetworkManager.Singleton.IsHost && IsServer; // true only on host machine
        if (isHost)
        {
            DataPersistenceManager.Instance.CaptureScreenshot();
        }
    }

    private IEnumerator WaitForNetworkControllerSpawn()
    {
        yield return new WaitUntil(() => NetworkConnectManager.Instance.IsSpawned);
        NetworkConnectManager.Instance.OnClientConnectedServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc]
    public void SetCharacterIdServerRpc(int id)
    {
        playerDataNetwork.characterId = id;
        animator.runtimeAnimatorController = GameMultiplayerManager.Instance.GetCharactersAnimator(id);
    }

    #endregion

    #region Game Loop
    void Update()
    {
        UpdatePlayerStatus();
        UpdateFishingProgress();
    }


    private void FixedUpdate()
    {
        MovementHandler();
    }

    public void EnableControl()
    {         
        if (_inputReader != null) _inputReader.EnableControl();
    }
    
    public void DisableControl()
    {
        if (_inputReader != null) _inputReader.DisableControl();
    }

    #endregion

    #region Bed Setup
    public void SetCurrentBed(BedScript bed)
    {
        if (HadTarget) return;
        CurrentBed = bed;
        CurrentBed.GetComponent<SpriteRenderer>().color = Color.red;
    }

    public void ClearBed()
    {
        CurrentBed.GetComponent<SpriteRenderer>().color = Color.white;
        CurrentBed = null;

    }
    #endregion

    #region Vehicle Setup
    public void SetCurrentVehicle(VehicleController vehicle)
    {
        if (HadTarget) return;
        CurrentVehicle = vehicle;
        CurrentVehicle.GetComponent<SpriteRenderer>().color = Color.red;
    }

    public void ClearVehicle()
    {
        CurrentVehicle.GetComponent<SpriteRenderer>().color = Color.white;
        CurrentVehicle = null;
    }

    //[ServerRpc(RequireOwnership = false)]
    //private void RequestToRideVehicleServerRpc(NetworkObjectReference playerRef, NetworkObjectReference vehicleRef, ServerRpcParams rpcParams = default)
    //{
    //    if (playerRef.TryGet(out NetworkObject playerObj) && vehicleRef.TryGet(out NetworkObject vehicleObj))
    //    {
    //        var player = playerObj.GetComponent<PlayerController>();
    //        var vehicle = vehicleObj.GetComponent<VehicleController>();

    //        vehicle.SetRiding(true, playerRef);
    //        vehicle.transform.SetParent(playerObj.transform,true);

    //        FixVehicleLocalScaleClientRpc(vehicleRef, playerRef);
    //    }
    //}


    //[ClientRpc]
    //private void FixVehicleLocalScaleClientRpc(NetworkObjectReference vehicleRef, NetworkObjectReference playerRef)
    //{
    //    if (vehicleRef.TryGet(out NetworkObject vehicleObj) && playerRef.TryGet(out NetworkObject playerObj))
    //    {
    //        var player = playerObj.GetComponent<PlayerController>();
    //        var vehicle = vehicleObj.GetComponent<VehicleController>();
    //        if (vehicle.transform.localScale.x < 0) vehicle.transform.localScale = new Vector3(1, 1, 1);
    //        player.IsFacingRight = vehicle.IsFacingRight.Value;
    //    }

    //}

    //[ServerRpc]
    //private void RequestToUnRideVehicleServerRpc(NetworkObjectReference vehicleRef)
    //{
    //    if (vehicleRef.TryGet(out NetworkObject vehicleObj))
    //    {
    //        SceneManager.MoveGameObjectToScene(vehicleObj.gameObject, SceneManager.GetActiveScene());
    //        //vehicleObj.transform.SetParent(null, true);
    //    }
    //    RequestToUnRideVehicleClientRpc(vehicleRef);
    //}

    //[ClientRpc]
    //private void RequestToUnRideVehicleClientRpc(NetworkObjectReference vehicleRef)
    //{
    //    if (vehicleRef.TryGet(out NetworkObject vehicleObj))
    //        vehicleObj.GetComponent<VehicleController>().SetRiding(false, GetComponent<NetworkObject>());
    //}
    #endregion

    #region Movement

    private void MovementHandler()
    {
        if (!CanMove)
        {
            if (rb.linearVelocity.magnitude > 0.1f)
                rb.AddForce(rb.linearVelocity * -_acceleration, ForceMode2D.Force);
            else
                rb.linearVelocity = Vector2.zero;

            return;
        }
        if (_movement != Vector2.zero)
        {

            rb.AddForce(_movement * _acceleration, ForceMode2D.Force);
            if (!IsRidingVehicle)
            {
                if (!IsRunning)
                {
                    if (rb.linearVelocity.magnitude > walkSpeed)
                    {
                        rb.linearVelocity = rb.linearVelocity.normalized * walkSpeed;
                    }
                }
                else
                {
                    if (rb.linearVelocity.magnitude > runSpeed)
                    {
                        rb.linearVelocity = rb.linearVelocity.normalized * runSpeed;
                    }
                }
            }
            else
            {
                if (rb.linearVelocity.magnitude > _vehicleSpeed)
                {
                    rb.linearVelocity = rb.linearVelocity.normalized * _vehicleSpeed;
                }
            }
        }
        else // do deceleration
        {

            if (rb.linearVelocity.magnitude > 0.1f)
                rb.AddForce(rb.linearVelocity * -_acceleration, ForceMode2D.Force);
            else
                rb.linearVelocity = Vector2.zero;
        }
    }
    public void OnMove(Vector2 inputMovement)
    {
        _movement = inputMovement.normalized;

        if (_movement != Vector2.zero && CanMove)
        {
            LastMovement = _movement;

            if (_movement.x > 0 && !IsFacingRight) IsFacingRight = true;
            else if (_movement.x < 0 && IsFacingRight) IsFacingRight = false;
        }

        animator.SetFloat("Speed", _movement.magnitude);

        if (IsRidingVehicle)
        {
            CurrentVehicle.SetMovement(_movement);
            if (_movement == Vector2.zero) return;
            CurrentVehicle.IsFacingRight = IsFacingRight;
            //SetCurrentVehicleMovementServerRpc(CurrentVehicle.GetComponent<NetworkObject>(), _movement, IsFacingRight);
        }

    }

    #endregion

    #region Actions Block
    public void StopAllAction()
    {
        CanMove = false;
        CanAttack = false;
    }

    public void StartAllAction()
    {
        CanMove = true;
        CanAttack = true;
        OnMove(_movement);
        CheckAnimation();
    }
    #endregion

    #region Animation
    public void CheckAnimation()
    {
        Debug.Log("after knockback the can attack is: " + CanAttack);
        if (!CanAttack || IsRidingVehicle) return;

        Item item = _inventoryManagerSO.GetCurrentItem();
        _itemOnHand.ActivateItemOnHand(null, false);
        _inventoryManagerSO.ShowPlaceableObject(false);
        tileTargeter.TargetRange = 1;
        if (item != null)
        {
            IsHoldingItem = true;
        }
        else
        {
            IsHoldingItem = false;
        }

        if (IsHoldingItem)
        {
            switch (item.type)
            {
                default:
                    {
                        ChangeAnimationState("Idle");

                        break;
                    }
                case ItemType.Tool:
                    {
                        ChangeAnimationState(item.name);

                        break;
                    }
                case ItemType.Crop:
                case ItemType.Food:
                case ItemType.Resouce:
                    {
                        _itemOnHand.ActivateItemOnHand(item.image, true);
                        ChangeAnimationState("Pickup_idle");
                        break;
                    }
                case ItemType.Tile:
                    {
                        _itemOnHand.ActivateItemOnHand(item.image, true);
                        ChangeAnimationState("Pickup_idle");
                        _inventoryManagerSO.ShowPlaceableObject(true);
                        tileTargeter.TargetRange = 100;
                        break;
                    }

            }
        }
        else ChangeAnimationState(AnimationStrings.idle);
    }

    private void ChangeAnimationState(string newState)
    {
        //if (CurrentState == newState) return;

        animator.Play(newState);
        CurrentState = newState;
        tileTargeter.RefreshTilemapCheck(!noTargetStates.Contains(newState));

    }
    #endregion

    #region Actions
    public void PlayerOnHit(Vector2 knockBackDirection)
    {
        if (!_damageable.IsAlive) animator.SetTrigger(AnimationStrings.dead);
        else
        {
            animator.SetTrigger(AnimationStrings.hurt);
            AudioManager.Instance.PlaySFX(hurtSounds[Random.Range(0, hurtSounds.Count)]);
            StartCoroutine(ApplyKnockback(knockBackDirection));
        }
    }

    private IEnumerator ApplyKnockback(Vector2 knockBackDirection)
    {
        yield return new WaitUntil(() => CanMove == false);
        knockBackDirection *= 15;
        rb.AddForce(knockBackDirection, ForceMode2D.Impulse);
        Debug.Log("knockBack apply: " + knockBackDirection);
    }
    private void UpdatePlayerStatus()
    {

        if (_playerStamina.Value <= 0)
            IsRunning = false;

        if (IsRunning && _movement != Vector2.zero)
            _playerStamina.Value -= _staminaDrainRate * Time.deltaTime;
        else
            _playerStamina.Value += _staminaDrainRate * Time.deltaTime;

        if(_playerStamina.Value < 0) 
            _playerStamina.Value = 0;
        if (_playerStamina.Value > 100)
            _playerStamina.Value = 100;

    }
    private void OnAttack()
    {
        if (!IsRidingVehicle && IsHoldingItem && CanAttack && Input.GetMouseButton(0) && !_inventoryManagerSO.IsPointerOverUI)
        {
            UseCurrentItem();
        }
        else if(IsFishing || IsHookingFish)
        {
            FinishFishing();
        }

    }

    public void GetFish() // on end fishing animation
    {
        Item fish = FishingManager.Instance.GetRandomFish();

        GameEventsManager.Instance.inventoryEvents.AddItem(fish.itemName);
        
    }

    public void ChooseFishingTime()
    {
        chosenFishingTime = Random.Range(fishingTimeSetting - 2, fishingTimeSetting + 2);
    }

    private void UpdateFishingProgress()
    {
        if (!IsFishing) return;
        fishingTimer += Time.deltaTime;
        if (fishingTimer >= chosenFishingTime)
        {
            animator.SetTrigger("HookedFish");
            fishingTimer = 0f;
            AudioManager.Instance.PlaySFX("hooked_fish");
            return;
        }
    }

    private void FinishFishing()
    {

        fishingTimer = 0f;
        if (!IsHookingFish)
        {
            animator.SetTrigger("StopFishing");

        }
        else
        {
            animator.SetTrigger("StartCaptureFish");
        }
    }
    public void DeactivateAttack2()
    {
        animator.SetBool("CanAttack2", false);
    }

    private void UseCurrentItem()
    {
        Item item = _inventoryManagerSO.GetCurrentItem();
        if (item == null) return;
        switch (item.type)
        {
            default:
                {

                    break;
                }
            case ItemType.Tool:
                {
                    animator.SetTrigger("Attack");
                    if(CurrentState != "FishingRod")
                        tileTargeter.UseTool(!noTargetStates.Contains(CurrentState));
                    if(CurrentState == "Sword")
                        AudioManager.Instance.PlaySFX("sword_swing");

                    break;
                }
            case ItemType.Crop:
            case ItemType.Tile:
                {
                    tileTargeter.SetTile(item);
                    break;
                }

        }

    }


    private void OnInteract()
    {
        if (!IsRidingVehicle && CanAttack)
        {
            if (tileTargeter.CanBreakPlacedTile()) return;
            else if (tileTargeter.CheckHarverst())
            {
                AudioManager.Instance.PlaySFX("pop");
                animator.SetTrigger(AnimationStrings.pickup);
                _itemOnHand.ActivateItemOnHand(null, false);
                CurrentState = null;
            }
            else
            {
                InteractWithFarmAnimal();
            }
        }
    }

    private void InteractWithFarmAnimal()
    {
        Vector2 clampedMousePosition = new Vector2 (    
            Mathf.Clamp(tileTargeter.MouseWorldPosition.x, transform.position.x - tileTargeter.TargetRange, transform.position.x + tileTargeter.TargetRange),
            Mathf.Clamp(tileTargeter.MouseWorldPosition.y, transform.position.y - tileTargeter.TargetRange, transform.position.y + tileTargeter.TargetRange)
        );

        int farmAnimalLayerMask = 1 << LayerMask.NameToLayer("FarmAnimal");
        Collider2D[] hit = Physics2D.OverlapCircleAll(clampedMousePosition, 1, farmAnimalLayerMask);
        Debug.DrawLine(transform.position, clampedMousePosition, Color.red, 1f);
        Item item = _inventoryManagerSO.GetCurrentItem();
        if (item == null || item.type != ItemType.Food)  // neu ko cam food
        {
            foreach (Collider2D collider in hit)
            {
                if (collider.TryGetComponent(out FarmAnimal farmAnimal))
                {
                    if (farmAnimal.IsInteractable)
                    {
                        farmAnimal.Interact();
                        return;
                    }
                }
            }
        }
        else
        {

            foreach (Collider2D collider in hit)
            {
                if (collider.TryGetComponent(out FarmAnimal farmAnimal))
                {
                    if (farmAnimal.FoodToEat == FarmAnimal.Food.None || farmAnimal.FoodToEat != item.foodType) continue;

                    if (farmAnimal.FoodToEat == item.foodType)
                    {
                        if(farmAnimal.Eat())
                        {
                            _inventoryManagerSO.DecreaseItemQuantityOnUse();
                            CheckAnimation();
                            return;
                        }
                        continue;
                    }
                }
            }
        }
    }

    private void OnSubmit()
    {
        
        onSubmit.Raise(this, ActionMap.Player);
    }

    // Load & Save
    private void OnSecondInteract()
    {
        if (CanRide && CurrentVehicle != null)
        {
            IsRidingVehicle = !IsRidingVehicle;
            if (IsRidingVehicle)
            {
                _itemOnHand.ActivateItemOnHand(null, false);
                ChangeAnimationState("Idle");
                StartAllAction();
                CurrentVehicle.SetRiding(true, this);
            }
            else
            {
                CurrentVehicle.SetRiding(false,this);
                CheckAnimation();

            }
        }

        if (CanSleep)
        {
            if (IsSleeping)
            {
                StartAllAction();
                IsSleeping = false;
                CurrentBed.SetSleep(IsSleeping);
                animator.SetBool(AnimationStrings.isSleep, false);
            }
            else
            {
                _itemOnHand.ActivateItemOnHand(null, false);
                StopAllAction();
                animator.SetBool(AnimationStrings.isSleep, true);

                IsSleeping = true;
                CurrentBed.SetSleep(IsSleeping);
            }
        }
    }
    private void OnRun(InputAction.CallbackContext context)
    {
        if (!CanRun) return;
        if (context.phase == InputActionPhase.Started) IsRunning = true;
        else if (context.phase == InputActionPhase.Canceled) IsRunning = false;
    }
    #endregion

    #region Save and Load

    public void LoadData(GameData gameData)
    {
        this.transform.position = gameData.PlayerData.Position;

        if (!gameData.GameFlowData.HasChoosenCharacter) return;

        playerDataSO.characterId = gameData.PlayerData.PlayerDataNetwork.characterId;
        playerDataSO.playerName = gameData.PlayerData.PlayerDataNetwork.playerName;
        playerDataSO.playerId = gameData.PlayerData.PlayerDataNetwork.playerId;
        playerDataSO.maxHealth = gameData.PlayerData.MaxHealth;
        playerDataSO.currentHealth = gameData.PlayerData.CurrentHealth;
        playerDataSO.maxMana = gameData.PlayerData.MaxMana;
        playerDataSO.currentMana = gameData.PlayerData.CurrentMana;
        playerDataSO.maxStamina = gameData.PlayerData.MaxStamina;
        playerDataSO.currentStamina = gameData.PlayerData.CurrentStamina;
        playerDataSO.money = gameData.PlayerData.Money;
        playerDataSO.position = gameData.PlayerData.Position;

        isPlayerLoadedData = true;
    }

    public void SaveData(ref GameData gameData)
    {
        gameData.SetPlayerData(new PlayerData(
            new PlayerDataNetwork(
                                  NetworkManager.Singleton.LocalClientId,
                                  playerDataSO.characterId,
                                  playerDataSO.playerName,
                                  playerDataSO.playerId),                        
            playerDataSO.maxHealth,
            playerDataSO.currentHealth,
            playerDataSO.maxMana,
            playerDataSO.currentMana,
            playerDataSO.maxStamina,
            playerDataSO.currentStamina,
            playerDataSO.money,
            this.transform.position));
    }
    #endregion
}