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
        float survivalTime = PlayerPrefs.GetFloat("SurvivalTime", 0f);
        
        int minutes = Mathf.FloorToInt(survivalTime / 60f);
        int seconds = Mathf.FloorToInt(survivalTime % 60f);
        survivalTimeText.text = string.Format("Tempo de Sobrevivência: {0:00}:{1:00}", minutes, seconds);
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene("Game");
    }
    
    public void QuitToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}