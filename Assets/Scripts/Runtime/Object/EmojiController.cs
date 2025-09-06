using System.Collections.Generic;
using UnityEngine;

public enum EmojiType
{
    Heart,
    Happy,
    VeryHappy,
    Sad,
    Angry,
    Normal
}


[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class EmojiController : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    [SerializeField]
    private List<Sprite> _heartEmojis = new();

    [SerializeField]
    private List<Sprite> _happyEmojis = new();
    [SerializeField]
    private List<Sprite> _sadEmojis = new();
    [SerializeField]
    private List<Sprite> _normalEmojis = new();
    [SerializeField]
    private List<Sprite> _angryEmojis = new();
    [SerializeField]
    private List<Sprite> _veryHappyEmojis = new();

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        gameObject.SetActive(false);
    }

    public void SetEmojiSprite(EmojiType type)
    {
        switch(type)
        {
            case EmojiType.Heart:
                int heartIndex = Random.Range(0, _heartEmojis.Count);
                _spriteRenderer.sprite = _heartEmojis[heartIndex];
                break;
            case EmojiType.Happy:
                int happyIndex = Random.Range(0, _happyEmojis.Count);
                _spriteRenderer.sprite = _happyEmojis[happyIndex];
                break;
            case EmojiType.VeryHappy:
                int veryHappyIndex = Random.Range(0, _veryHappyEmojis.Count);
                _spriteRenderer.sprite = _veryHappyEmojis[veryHappyIndex];
                break;
            case EmojiType.Sad:
                int sadIndex = Random.Range(0, _sadEmojis.Count);
                _spriteRenderer.sprite = _sadEmojis[sadIndex];
                break;
            case EmojiType.Angry:
                int angryIndex = Random.Range(0, _angryEmojis.Count);
                _spriteRenderer.sprite = _angryEmojis[angryIndex];
                break;
            case EmojiType.Normal:
                int normalIndex = Random.Range(0, _normalEmojis.Count);
                _spriteRenderer.sprite = _normalEmojis[normalIndex];
                break;

            default:
                Debug.LogWarning("Emoji type not recognized: " + type);
                break;
        }


    }

    public void DeactivateEmoji()
    {
        gameObject.SetActive(false);
    }
}
