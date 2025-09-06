using UnityEngine;

public class SimpleBounceObject : MonoBehaviour
{
    public Vector2 testdir = Vector2.right;
    public float testDistance = 3f;

    [Header("Bounce Settings")]
    public float baseSpeed = 5f;
    public float minSpeed = 3f;
    public float maxSpeed = 10f;
    public float heightFactor = 0.33f;

    [Header("Decay Settings")]
    public float bounceDampening = 0.6f;     // Distance *= this each bounce
    public float minBounceDistance = 0.1f;   // Stop bouncing if next distance < this

    private Vector3 _startPos;
    private Vector3 _targetPos;
    private float _travelDistance;
    private float _travelTime;
    private float _bounceHeight;
    private float _elapsedTime;

    private Vector2 _bounceDirection;
    private float _currentDistance;
    private bool _isBouncing;

    private void Update()
    {
        if (!_isBouncing) return;

        _elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(_elapsedTime / _travelTime);

        // Horizontal movement
        Vector3 horizontalPos = Vector3.Lerp(_startPos, _targetPos, t);

        // Vertical arc
        float height = 4f * _bounceHeight * t * (1 - t);

        transform.position = horizontalPos + Vector3.up * height;

        if (t >= 1f)
        {
            _isBouncing = false;

            // Decay distance
            _currentDistance *= bounceDampening;

            if (_currentDistance >= minBounceDistance)
            {
                StartBounce(_bounceDirection, _currentDistance);
            }
        }
    }

    [ContextMenu("Test Bounce")]
    public void TestBounce()
    {
        StartBounce(testdir.normalized, testDistance);
        _currentDistance = testDistance;
    }

    public void Bounce(Vector2 direction, float initialDistance)
    {
        _bounceDirection = direction.normalized;
        _currentDistance = initialDistance;

        StartBounce(_bounceDirection, _currentDistance);
    }

    private void StartBounce(Vector2 direction, float distance)
    {
        _bounceDirection = direction;
        _startPos = transform.position;
        _targetPos = _startPos + (Vector3)(direction * distance);
        _travelDistance = distance;

        float actualSpeed = Mathf.Clamp(baseSpeed * _travelDistance, minSpeed, maxSpeed);
        _travelTime = _travelDistance / actualSpeed;
        _bounceHeight = _travelDistance * heightFactor;

        _elapsedTime = 0f;
        _isBouncing = true;
    }
}
