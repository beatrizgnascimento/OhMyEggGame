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
    
    // Mude o enum para PUBLIC
    public enum PlayerState { Normal, Pulando, Dano }
    private PlayerState currentState = PlayerState.Normal;
    
    void Awake()
    {
        // Busca o SpriteRenderer em qualquer objeto filho
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            UpdateSprite();
            Debug.Log("✅ SpriteRenderer encontrado no objeto filho");
        }
        else
        {
            Debug.LogError("❌ SpriteRenderer não encontrado em nenhum objeto filho!");
        }
    }
    
    void Update()
    {
        CheckDeathConditions();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"🎯 Colidiu com: {collision.gameObject.name}");
    
        BlockController block = collision.gameObject.GetComponent<BlockController>();
    
        if(block != null)
        {
            Debug.Log($"📦 Bloco detectado. Tipo: {block.Type}");
        
            if(block.Type == BlockController.BlockType.Harmful)
            {
                if (!touchedHarmfulBlocks.Contains(collision.gameObject))
                {
                    Debug.Log("💥 Bloco PERIGOSO - Aplicando dano pela primeira vez!");
                    touchedHarmfulBlocks.Add(collision.gameObject);
                    TakeDamage();
                }
                else
                {
                    Debug.Log("🟡 Bloco PERIGOSO já tocado anteriormente - Dano evitado");
                }
            }
            else
            {
                Debug.Log("✅ Bloco SEGURO - Sem dano");
            }
        }
    }

    void TakeDamage()
    {
        Debug.Log("🔴 DANO TOMADO!");
    
        SetPlayerState(PlayerState.Dano);
        
        if (!isFlashing && spriteRenderer != null)
        {
            StartCoroutine(DamageFlash());
        }
    
        if (GameManager.Instance != null)
        {
            Debug.Log("✅ GameManager encontrado, aplicando dano");
            GameManager.Instance.TakeDamage();
        }
        else
        {
            Debug.LogError("❌ GameManager Instance é NULL!");
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
        
        if (currentState == PlayerState.Dano)
        {
            SetPlayerState(PlayerState.Normal);
        }
        
        isFlashing = false;
    }

    // Método público para outros scripts
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

        switch (currentState)
        {
            case PlayerState.Normal:
                spriteRenderer.sprite = spriteNormal;
                break;
            case PlayerState.Pulando:
                spriteRenderer.sprite = spritePulando;
                break;
            case PlayerState.Dano:
                spriteRenderer.sprite = spriteDano;
                break;
        }
    }

    public void ClearTouchedBlocks()
    {
        touchedHarmfulBlocks.Clear();
        Debug.Log("🧹 Lista de blocos tocados foi limpa");
    }
    
    void CheckDeathConditions()
    {
        if (transform.position.y >= deathYPosition)
        {
            Debug.Log($"☠️ MORTO - Atingiu o topo! Y: {transform.position.y} > {deathYPosition}");
            GameOverByTop();
        }
        else if (transform.position.y <= fallDeathY)
        {
            Debug.Log($"☠️ MORTO - Caiu demais! Y: {transform.position.y} < {fallDeathY}");
            GameOverByFall();
        }
    }
    
    void GameOverByTop()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsGameOver())
        {
            Debug.Log("💥 Aplicando dano duplo por morte no topo");
            GameManager.Instance.TakeDamage();
            GameManager.Instance.TakeDamage();
            
            if (!isFlashing && spriteRenderer != null)
            {
                StartCoroutine(DamageFlash());
            }
        }
    }
    
    void GameOverByFall()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsGameOver())
        {
            Debug.Log("💥 Aplicando dano duplo por queda");
            GameManager.Instance.TakeDamage();
            GameManager.Instance.TakeDamage();
            
            if (!isFlashing && spriteRenderer != null)
            {
                StartCoroutine(DamageFlash());
            }
        }
    }
}