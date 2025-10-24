using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverSceneManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI survivalTimeText;
    
    void Start()
    {
        // Recupera o tempo de sobrevivência salvo
        float survivalTime = PlayerPrefs.GetFloat("SurvivalTime", 0f);
        
        // Formata o tempo
        int minutes = Mathf.FloorToInt(survivalTime / 60f);
        int seconds = Mathf.FloorToInt(survivalTime % 60f);
        survivalTimeText.text = string.Format("Tempo de Sobrevivência: {0:00}:{1:00}", minutes, seconds);
    }
    
    public void RestartGame()
    {
        // Carrega a cena do jogo
        SceneManager.LoadScene("Game"); // Substitua "Game" pelo nome da sua cena principal
    }
    
    public void QuitToMenu()
    {
        // Carrega a cena do menu
        SceneManager.LoadScene("Menu"); // Substitua "Menu" pelo nome da sua cena de menu
    }
}