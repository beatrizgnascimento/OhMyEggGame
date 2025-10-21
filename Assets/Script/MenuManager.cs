using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void StartGame()
    {
        // Carrega a cena do jogo
        SceneManager.LoadScene("Game");
    }

    public void ExitGame()
    {
        // Sai do jogo
        Debug.Log("Saindo do jogo...");
        
        #if UNITY_EDITOR
            // Se estiver no Editor, para a execução
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // Se for build final, fecha o aplicativo
            Application.Quit();
        #endif
    }
}