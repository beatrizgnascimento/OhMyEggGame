using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float damageAmount = 20f;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image healthFill;
    
    [Header("Game Speed Settings")]
    [SerializeField] private float speedIncreaseRate = 0.05f; 
    [SerializeField] private float maxSpeedMultiplier = 1.5f; 
    public float GlobalSpeedMultiplier { get; private set; } = 1f;
    
    [Header("Heart Spawn Settings")]
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private float spawnXRange = 2.5f; 
    [SerializeField] private float minSpawnTime = 3f;
    [SerializeField] private float maxSpawnTime = 8f;

    private bool hasSpawnedHeart = false;
    private float heartTimer = 0f;
    private float targetSpawnTime = 0f;
    
    private float currentHealth;
    private float survivalTime;
    private bool isGameOver;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentHealth = maxHealth;
        isGameOver = false;
        UpdateHealthUI();
    }

    void Update()
    {
        if (!isGameOver)
        {
            survivalTime += Time.deltaTime;
            
            float calculatedSpeed = 1f + (survivalTime * speedIncreaseRate);

            GlobalSpeedMultiplier = Mathf.Min(calculatedSpeed, maxSpeedMultiplier);
            
            HandleHeartSpawning();
        }
    }

    public void TakeDamage()
    {
        if (isGameOver) return; 
        
        currentHealth -= damageAmount;
    
        UpdateHealthUI();
    
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            GameOver();
        }
    }
    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / maxHealth;
        }
        
        if (healthFill != null)
        {
            if (currentHealth > 50f)
            {
                healthFill.color = Color.Lerp(Color.yellow, Color.green, (currentHealth - 50f) / 50f);
            }
            else
            {
                healthFill.color = Color.Lerp(Color.red, Color.yellow, currentHealth / 50f);
            }
        }
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    public void GameOver()
    {
        isGameOver = true;
    
        PlayerPrefs.SetFloat("SurvivalTime", survivalTime);
        PlayerPrefs.Save();
    
        SceneManager.LoadScene("Game Over");
    }
    
    void HandleHeartSpawning()
    {
        // Reset the condition if player is fully healed
        if (currentHealth >= maxHealth)
        {
            hasSpawnedHeart = false;
            heartTimer = 0f;
            return;
        }

        // Check if HP is 40% or less
        if (currentHealth <= (maxHealth * 0.4f) && !hasSpawnedHeart)
        {
            // Initialize the random target time if starting the timer
            if (heartTimer == 0f)
            {
                targetSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
            }

            heartTimer += Time.deltaTime;

            if (heartTimer >= targetSpawnTime)
            {
                SpawnHeart();
                hasSpawnedHeart = true;
            }
        }
    }

    void SpawnHeart()
    {
        if (heartPrefab != null)
        {
            float randomX = Random.Range(-spawnXRange, spawnXRange);
            Vector2 spawnPos = new Vector2(randomX, 10f); // Spawn above the screen
            Instantiate(heartPrefab, spawnPos, Quaternion.identity);
        }
    }

    public void HealFull()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}