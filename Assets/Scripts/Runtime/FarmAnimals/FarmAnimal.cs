using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Animator))]
public abstract class FarmAnimal : MonoBehaviour
{
    public enum FarmAnimalKind
    {
        Chicken,
        FemaleCow,
        MaleCow,
        FemaleSheep,
        MaleSheep
    }

    public enum Food
    {
        None,
        Seeds,
        Wheat
    }
    #region Components
    [SerializeField] protected Rigidbody2D _body;
    [SerializeField] protected Animator _animator;
    [SerializeField] protected FarmAnimalSO _animalInfo;
    #endregion

    #region Variables


    [SerializeField] protected Gender gender;

    [SerializeField]
    public bool IsInteractable = true;
    // needs to save
    [SerializeField]
    protected FarmAnimalKind animalKind;

    [SerializeField]
    public bool isFed = false;

    [SerializeField]
    protected int resetFedTime = 1000;

    [SerializeField] 
    protected bool canMakeProduct = false;

    [SerializeField]
    protected int fedTimeCounter = 0;

    // current stage and position too

    //end

    protected float maxRadius = 5f;
    protected float speed = 2f;
    protected Vector2 targetPosition;

    [SerializeField] protected bool isMoving = false;

    [SerializeField] protected bool _canMove = true;
    public bool CanMove
    {
        get => _canMove;
        set
        {
            _canMove = value;
            if (!_canMove)
            {
                _body.linearVelocity = Vector2.zero;
                isMoving = false;
            }
        }
    }

    public Food FoodToEat;

    [SerializeField]
    protected List<AudioClip> animalSounds = new();

    [Header("Obstacle Detection")]
    [SerializeField] protected LayerMask collisionMask;
    [SerializeField] private float rayDistance = 1.5f;
    [SerializeField] private float rayAngleSpread = 30f; // total cone angle

    private bool isPlayingSound = false;
    private bool canPlaySound = false;
    private float distance;
    private float maxDistance = 10f;
    private Vector2 viewPos;
    private bool isInView;

    #endregion

    #region Animation variables
    protected Vector2 _lastMovement;
    public Vector2 LastMovement
    {
        get => _lastMovement;
        set
        {
            _lastMovement = value;
            float clampedX = Mathf.Abs(Mathf.Round(_lastMovement.x));
            float clampedY = Mathf.Round(_lastMovement.y);
            _animator.SetFloat(HorizontalParameter, clampedX);
            _animator.SetFloat(VerticalParameter, clampedY);

            if (_lastMovement.x > 0 && !_isFacingRight) IsFacingRight = true;
            else if (_lastMovement.x < 0 && _isFacingRight) IsFacingRight = false;
        }
    }

    private bool _isFacingRight = false;
    public bool IsFacingRight
    {
        get => _isFacingRight;
        set
        {
            _isFacingRight = value;
            transform.localScale = _isFacingRight ? new Vector3(-1, 1, 1) : Vector3.one;
        }
    }

    public string HorizontalParameter => "Horizontal";
    public string VerticalParameter => "Vertical";
    #endregion

    #region Reference
    [SerializeField]
    private EmojiController _emoji;
    #endregion

    private Coroutine stopMovingCoroutine;

    protected void OnEnable()
    {
        FarmAnimalManager.Instance.RegisterAnimal(this);
    }

    protected void OnDisable()
    {
        FarmAnimalManager.Instance.UnregisterAnimal(this);
    }


    private void Awake()
    {
        _emoji = GetComponentInChildren<EmojiController>(true);
    }
    protected virtual void Start()
    {
        StartCoroutine(PlaySoundAfterAFewTimes());
    }

    protected void Update()
    {
        distance = Vector2.Distance(Camera.main.transform.position, transform.position);
        viewPos = Camera.main.WorldToViewportPoint(transform.position);
        isInView = viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1 && distance <= maxDistance;

        if (isInView)
        {
            canPlaySound = true;
        }
        else
        {
            canPlaySound = false;
        }

        SetAnimator();
        if (!CanMove) return;

        if (!isMoving)
            ChooseNewTarget();
        else
            MoveToTarget(); 
    }

    private void ChooseNewTarget()
    {
        Vector2 currentPosition = transform.position;
        float randomAngle = Random.Range(0f, 2f * Mathf.PI);
        float randomDistance = Random.Range(0f, maxRadius);
        Vector2 offset = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)) * randomDistance;
        targetPosition = currentPosition + offset;
        isMoving = true;
    }

    private void MoveToTarget()
    {
        if (_body.linearVelocity.sqrMagnitude > 0.01f && CheckFanObstacle())
        {
            isMoving = false;
            StartStopMoving(5);
            return;
        }

        Vector2 currentPosition = _body.position;
        Vector2 direction = (targetPosition - currentPosition).normalized;
        _body.linearVelocity = direction * speed;

        if (direction != Vector2.zero)
            LastMovement = direction;

        if (Vector2.Distance(currentPosition, targetPosition) < 0.1f)
        {
            isMoving = false;
            StartStopMoving(5);
        }

        Debug.DrawRay(currentPosition, LastMovement * rayDistance, Color.yellow);
    }


    private bool CheckFanObstacle()
    {
        if (LastMovement == Vector2.zero) return false;

        Vector2 origin = _body.position;
        Vector2 forward = LastMovement.normalized;
        float half = rayAngleSpread * 0.5f;
        float[] angles = { -half, 0f, half };

        foreach (float a in angles)
        {
            Vector2 dir = Quaternion.Euler(0, 0, a) * forward;
            Debug.DrawRay(origin, dir * rayDistance, Color.red);
            if (Physics2D.Raycast(origin, dir, rayDistance, collisionMask))
                return true;
        }
        return false;
    }

    private IEnumerator StopMoving(int seconds)
    {
        CanMove = false;
        yield return new WaitForSeconds(seconds);
        CanMove = true;
    }
    private void StartStopMoving(int seconds = 5)
    {
        if (stopMovingCoroutine != null)
            StopCoroutine(stopMovingCoroutine);
        stopMovingCoroutine = StartCoroutine(StopMoving(seconds));
    }

    // Draw the fan cone in the Scene view when selected
    private void OnDrawGizmosSelected()
    {
        if (_body == null) return;
        Vector2 origin = Application.isPlaying ? _body.position : (Vector2)transform.position;
        Vector2 forward = LastMovement.normalized;
        if (forward == Vector2.zero)
            forward = Vector2.right;

        Gizmos.color = Color.red;
        float half = rayAngleSpread * 0.5f;
        float[] angles = { -half, 0f, half };
        foreach (float a in angles)
        {
            Vector2 dir = Quaternion.Euler(0, 0, a) * forward;
            Gizmos.DrawLine(origin, origin + dir * rayDistance);
        }
    }

    public void SetAnimator()
    {
        _animator.SetFloat("Speed", _body.linearVelocity.magnitude);
    }

    [ContextMenu("Eat")]
    public virtual bool Eat()
    {
        if (isFed) return false;
        _animator.SetTrigger("Eat");
        _emoji.SetEmojiSprite(EmojiType.VeryHappy);
        _emoji.gameObject.SetActive(true);
        StartStopMoving(5);
        isFed = true;
        if(this is Chicken)
        PlayAnimalSound(1);
        else
            PlayAnimalSound();
        return true;
    }

    protected void ChangeResetFedTime(int value = 1000)
    {
        resetFedTime = value;
    }
    public abstract void FedTimeHandler(int minute);
    protected virtual void MakeProduct() { }
    protected virtual void GetProduct() { }
    protected virtual void InteractWithAnimal() { }
    public abstract void IncreaseGrowStage();

    protected virtual void ApplyStage(string stage)
    {
        _animator.SetTrigger(stage);
    }

    
    public virtual void Interact()
    {
        if(!IsInteractable) return;
        _emoji.SetEmojiSprite(EmojiType.Happy);
        _emoji.gameObject.SetActive(true);
        StartCoroutine(ResetInteractable(3));
        if (this is Chicken)
            PlayAnimalSound(1);
        else
            PlayAnimalSound();
    }

    private IEnumerator ResetInteractable(float seconds)
    {
        IsInteractable = false;
        CanMove = false;
        yield return new WaitForSeconds(seconds);
        IsInteractable = true;
        CanMove = true;
    }

    protected virtual IEnumerator PlaySoundAfterAFewTimes()
    {
        while (true)
        {
            // Wait until we're in the correct scene before doing anything
            while (SceneManager.GetActiveScene().name != Loader.Scene.WorldScene.ToString())
            {
                yield return new WaitForSeconds(1f); // wait a bit, not every frame
            }

            // Once in WorldScene, start the timer
            float waitTime = Random.Range(7f, 20f);
            float elapsed = 0f;

            // Timer loop: wait only while still in the WorldScene
            while (elapsed < waitTime)
            {
                if (SceneManager.GetActiveScene().name != Loader.Scene.WorldScene.ToString())
                    break; // abort this cycle if we leave the scene

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Only play the sound if we finished the timer **and** still in the scene
            if (SceneManager.GetActiveScene().name == Loader.Scene.WorldScene.ToString())
            {
                if (canPlaySound) PlayAnimalSound();
            }
        }
    }


    protected void PlayAnimalSound(int? specificAudioClipIndex = null)
    {
        if(animalSounds.Count == 0) return;
        AudioClip audioClip = specificAudioClipIndex == null ? animalSounds[Random.Range(0, animalSounds.Count)] : animalSounds[(int)specificAudioClipIndex];
        AudioManager.Instance.PlaySFX(audioClip);
    }


    public void LoadData(FarmAnimalSaveData farmAnimalSaveData)
    {
        var data = farmAnimalSaveData.GetData();
        this.isFed = data.IsFed;
        this.resetFedTime = data.ResetFedTime;
        this.canMakeProduct = data.CanMakeProduct;
        this.fedTimeCounter = data.FedTimeCounter;
        transform.position = data.Position;

    }

    public FarmAnimalSaveData GetDataToSave()
    {
        return new FarmAnimalSaveData(animalKind, resetFedTime, canMakeProduct, fedTimeCounter, isFed, transform.position);
    }


    public void SetCurrentGrowthStage(FarmAnimalSaveData farmAnimalSaveData)
    {
        switch (this)
        {
            case Chicken chicken:
                chicken.SetChickenGrowthStage(farmAnimalSaveData.GetChickenGrowthStage());
                break;
            case Cow cow:
                cow.SetCowGrowthStage(farmAnimalSaveData.GetCowGrowthStage());
                break;
            case Sheep sheep:
                sheep.SetSheepGrowthStage(farmAnimalSaveData.GetSheepGrowthStage());
                break;
            default:
                Debug.LogWarning($"Unknown farm animal type: {this.GetType().Name}. Cannot set growth stage.");
                break;
        }
    }
}
