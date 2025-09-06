using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public UnityEvent<Vector2> onHit;
    private Animator animator;

    public FloatVariable playerHealth;
    [SerializeField]
    private float _maxHealth = 100;
    public float MaxHealth
    {
        get { return _maxHealth; }
        set { _maxHealth = value; }
    }

    [SerializeField]
    private float _health = 100;
    public float Health
    {
        get { return _health; }
        set
        {
            _health = value;

            if (_health <= 0)
            {
                IsAlive = false;
            }
        }
    }

    [SerializeField]
    private bool _isAlive = true;

    [SerializeField]
    private bool isInvincible = false;

    private float timeSinceHit;
    [SerializeField]
    private float invincibilityTime;

    public bool IsAlive
    {
        get { return _isAlive; }
        set
        {
            _isAlive = value;
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (GetComponent<PlayerController>() != null)
        {
            playerHealth.Value = MaxHealth;
        }
        timeSinceHit = 0;
    }

    private void Update()
    {
        if (isInvincible)
        {
            if (timeSinceHit >= invincibilityTime)
            {
                isInvincible = false;
                timeSinceHit = 0;
            }
            else
            timeSinceHit += Time.deltaTime;
        }

    }

    [ContextMenu("Test hit")]
    public void TestHit()
    {
        Hit(20, Vector2.right * 3);
    }
    public bool Hit(float damage, Vector2 knockbackVelocity)
    {
        if (IsAlive && !isInvincible)
        {
            if(GetComponent<PlayerController>() != null)
            {
                playerHealth.Value -= damage;
                if (playerHealth.Value <= 0)
                {
                    playerHealth.Value = 0;
                    IsAlive = false;
                }
            }
            else
            {
                Health -= damage;
            }
            isInvincible = true;

            onHit?.Invoke(knockbackVelocity);

            return true;
        }

        return false;
    }
}
