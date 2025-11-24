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

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}