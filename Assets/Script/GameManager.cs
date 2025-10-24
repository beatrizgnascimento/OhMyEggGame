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
    [SerializeField] private Slider healthSlider; // ✅ REFERÊNCIA DO SLIDER
    [SerializeField] private Image healthFill;    // ✅ REFERÊNCIA DA COR
    
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
        UpdateHealthUI(); // ✅ INICIALIZA A UI
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
        if (isGameOver) 
        {
            Debug.Log("⚠️ Já está em Game Over, ignorando dano");
            return;
        }
    
        currentHealth -= damageAmount;
        Debug.Log($"💥 Dano aplicado! Vida: {currentHealth}/{maxHealth}");
    
        UpdateHealthUI();
    
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("🎯 Vida chegou a 0! Chamando GameOver...");
            GameOver();
        }
        else
        {
            Debug.Log($"🩸 Vida restante: {currentHealth}");
        }
    }

    // ✅ MÉTODO NOVO - ATUALIZA A BARRA DE VIDA
    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / maxHealth;
        }
        
        // ✅ ATUALIZA COR (VERDE → AMARELO → VERMELHO)
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
        
        Debug.Log($"🩸 Vida: {currentHealth}/{maxHealth} - Slider: {healthSlider?.value}");
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    public void GameOver()
    {
        Debug.Log("🎮 INICIANDO GAME OVER...");
    
        isGameOver = true;
    
        // Salva o tempo de sobrevivência
        PlayerPrefs.SetFloat("SurvivalTime", survivalTime);
        PlayerPrefs.Save();
    
        Debug.Log($"💾 Tempo salvo: {survivalTime} segundos");
        Debug.Log("🔄 Carregando cena GameOverScene...");
    
        // Carrega a cena de Game Over
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