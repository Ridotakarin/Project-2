using UnityEngine;
using UnityEngine.Events;

public class FakeHeightObject : MonoBehaviour
{
    [Header("Speed Settings")]
    [SerializeField] private float _baseSpeed;
    [SerializeField] private float _minSpeed;
    [SerializeField] private float _maxSpeed;

    [Header("Bounce Settings")]
    [SerializeField] private float _minimumBounceDistance = 1.2f;
    [SerializeField] private float _bounceDrag = 0.25f;

    [Header("References")]
    [SerializeField] private Transform _shadowObject;
    [SerializeField] private float _minShadowScale;

    [Header("Events")]
    public UnityEvent OnArrivedDestination;

    private Animator _animator;
    private Transform _mainObject;

    private Vector2 _lastDestination;
    private Vector2 _direction;
    private Vector2 _mainVelocity;

    private float _travelDistance;
    private float _peakHeight;
    private float _finalSpeed;
    private float _timeToDestination;
    private float _verticalVelocity;
    private float _initialVerticalVelocity;
    private float _gravity;

    private bool _isArrived;
    private bool _canMove;

    private void Awake()
    {
        _mainObject = transform.root;
        _animator = GetComponent<Animator>();
        _animator.speed = 0;
    }

    private void Update()
    {
        if (!_canMove || _isArrived) return;

        UpdateMovement();
    }

    public void Initialize(Vector2 destination)
    {
        _lastDestination = destination;
        _direction = (_lastDestination - (Vector2)_mainObject.position);
        _travelDistance = _direction.magnitude;

        // Dynamic speed scaling
        _finalSpeed = Mathf.Clamp(_baseSpeed * _travelDistance, _minSpeed, _maxSpeed);
        _timeToDestination = _travelDistance / _finalSpeed;
        _mainVelocity = _direction.normalized * _finalSpeed;

        // Arc setup
        _peakHeight = _travelDistance / 3f;
        _initialVerticalVelocity = (2 * _peakHeight) / (_timeToDestination / 2f);
        _gravity = (2 * _peakHeight) / Mathf.Pow((_timeToDestination / 2f), 2);

        _verticalVelocity = _initialVerticalVelocity;
        _animator.speed = 0;

        _isArrived = false;
        _canMove = true;
    }

    private void UpdateMovement()
    {
        // Horizontal movement
        _mainObject.Translate(_mainVelocity * Time.deltaTime);

        // Vertical movement
        _verticalVelocity -= _gravity * Time.deltaTime;
        transform.Translate(Vector3.up * _verticalVelocity * Time.deltaTime);

        // Calculate current vertical height above ground
        float currentHeight = transform.position.y - _mainObject.position.y;

        // Remap height to 0 (grounded) → 1 (max height)
        float heightRatio = Mathf.Clamp01(currentHeight / _peakHeight);

        // Invert to get scale (1 at ground, smaller in air)
        float shadowScale = Mathf.Lerp(1f, _minShadowScale, heightRatio);

        // Apply to shadow scale
        _shadowObject.localScale = new Vector3(shadowScale, shadowScale, shadowScale);

        // Ground check
        if (transform.position.y <= _mainObject.position.y)
        {
            transform.position = _mainObject.position;
            Arrive();
        }
    }

    private void Arrive()
    {
        _isArrived = true;
        _canMove = false;

        // Trigger bounce or explosion
        if (_travelDistance > _minimumBounceDistance)
        {
            OnArrivedDestination?.Invoke(); // Do bounce
        }
        else
        {
            _animator.speed = 1; // Play explosion animation
        }
    }

    public void Bounce()
    {
        // Halfway to last direction for bounce
        Vector2 bounceTarget = _lastDestination + _direction * _bounceDrag;
        Initialize(bounceTarget);
    }
}