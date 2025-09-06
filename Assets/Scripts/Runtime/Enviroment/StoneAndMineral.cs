using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoneAndMineral : ItemDropableEntity
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _onHitTime;
    private Coroutine _hitCoroutine;
    [SerializeField] private GameEvent _onMineralsDestroy;
    public enum StoneAndMineralType
    {
        Small,
        Big
    }

    public StoneAndMineralType stoneAndMineralType;
    protected override void Awake()
    {
        base.Awake();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
    }

    [ClientRpc]
    public void InitializeMineralClientRpc(string entityId)
    {
        if (ItemDropableEntityDatabase.Instance.GetEntity(entityId) == null)
        {

            Debug.LogError("Entity not found in database: " + entityId);
            return;
        }
        entityInfo = ItemDropableEntityDatabase.Instance.GetEntity(entityId);
        _spriteRenderer.sprite = entityInfo.mineBlockIdleSprite;
    }

    public override void OnHit(Vector2 knockback)
    {
        AudioManager.Instance.PlaySFX("Pickaxe_blow");
        if (!damageable.IsAlive)
        {
            DropItem(false);
            if(SceneManager.GetActiveScene().name == Loader.Scene.MineScene.ToString())
            _onMineralsDestroy.Raise(this,null);
        }
    }

}
