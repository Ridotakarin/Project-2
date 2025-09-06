using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviromentTree : ItemDropableEntity
{
    [SerializeField] private Animator _animator;
    protected override void Awake()
    {
        base.Awake();
        _animator = GetComponent<Animator>();
    }
    protected void OnEnable()
    {
        GameEventsManager.Instance.enviromentStatusEvents.onChangeSeason += ChangeBySeason;
    }

    protected void OnDisable()
    {
        GameEventsManager.Instance.enviromentStatusEvents.onChangeSeason -= ChangeBySeason;
    }
    protected void ChangeBySeason(ESeason season)
    {
        switch (season)
        {
            case ESeason.Spring:
                {
                    _animator.Play(AnimationStrings.springIdle);
                    break;
                }
            case ESeason.Summer:
                {
                    _animator.Play(AnimationStrings.summerIdle);
                    break;
                }
            case ESeason.Autumn:
                {
                    _animator.Play(AnimationStrings.autumnIdle);
                    break;
                }
            case ESeason.Winter:
                {
                    _animator.Play(AnimationStrings.winterIdle);
                    break;
                }
        }
    }
    public override void OnHit(Vector2 knockback)
    {
        AudioManager.Instance.PlaySFX("Chop3");
        if (!damageable.IsAlive)
        {
            DropItem(!damageable.IsAlive);
            Destroy(gameObject);
        }else if (damageable.Health == 20)
        {
            _animator.Play("Root_Idle");
            DropItem(!damageable.IsAlive);
        }
        else _animator.SetTrigger("Hit");
    }
}
