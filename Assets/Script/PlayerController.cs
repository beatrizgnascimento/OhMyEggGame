using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Death Settings")]
    [SerializeField] private float deathYPosition = 10f;
    [SerializeField] private float fallDeathY = -10f;
    
    [Header("Damage Effects")]
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private float flashDuration = 0.3f;
    [SerializeField] private int numberOfFlashes = 3;
    
    [Header("Player Sprites")]
    [SerializeField] private Sprite spriteNormal;
    [SerializeField] private Sprite spritePulando;
    [SerializeField] private Sprite spriteDano;
    
    private HashSet<GameObject> touchedHarmfulBlocks = new HashSet<GameObject>();
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isFlashing = false;
    
    public enum PlayerState { Normal, Pulando, Dano }
    private PlayerState currentState = PlayerState.Normal;
    
    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            UpdateSprite();
        }
    }
    
    void Update()
    {
        CheckDeathConditions();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
    
        BlockController block = collision.gameObject.GetComponent<BlockController>();

        if (block == null) return;
        if (block.Type != BlockController.BlockType.Harmful) return;
        if (touchedHarmfulBlocks.Contains(collision.gameObject)) return;
        
        touchedHarmfulBlocks.Add(collision.gameObject);
        TakeDamage();
    }

    void TakeDamage()
    {
        SetPlayerState(PlayerState.Dano);
        
        if (!isFlashing && spriteRenderer != null)
        {
            StartCoroutine(DamageFlash());
        }
    
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TakeDamage();
        }
    }

    IEnumerator DamageFlash()
    {
        isFlashing = true;
        
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRenderer.color = damageColor;
            yield return new WaitForSeconds(flashDuration / (numberOfFlashes * 2));
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashDuration / (numberOfFlashes * 2));
        }
        
        spriteRenderer.color = originalColor;
        
        isFlashing = false;
        
        if (currentState == PlayerState.Dano)
        {
            SetPlayerState(PlayerState.Normal);
        }
    }

    public void SetPlayerState(PlayerState newState)
    {
        if (currentState == PlayerState.Dano && newState != PlayerState.Dano && isFlashing)
        {
            return;
        }
        
        currentState = newState;
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        if (spriteRenderer == null) return;

        spriteRenderer.sprite = currentState switch
        {
            PlayerState.Normal => spriteNormal,
            PlayerState.Pulando => spritePulando,
            PlayerState.Dano => spriteDano,
            _ => spriteRenderer.sprite
        };
    }

    public void ClearTouchedBlocks()
    {
        touchedHarmfulBlocks.Clear();
    }
    
    void CheckDeathConditions()
    {
        if (transform.position.y >= deathYPosition)
        {
            GameOverByTop();
        }
        else if (transform.position.y <= fallDeathY)
        {
            GameOverByFall();
        }
    }
    
    void GameOverByTop()
    {
        if (GameManager.Instance == null || GameManager.Instance.IsGameOver()) return;
        
        GameManager.Instance.TakeDamage();
        GameManager.Instance.TakeDamage();
            
        if (!isFlashing && spriteRenderer != null)
        {
            StartCoroutine(DamageFlash());
        }
    }
    
    void GameOverByFall()
    {
        if (GameManager.Instance == null || GameManager.Instance.IsGameOver()) return;
        
        GameManager.Instance.TakeDamage();
        GameManager.Instance.TakeDamage();
            
        if (!isFlashing && spriteRenderer != null)
        {
            StartCoroutine(DamageFlash());
        }
    }
}