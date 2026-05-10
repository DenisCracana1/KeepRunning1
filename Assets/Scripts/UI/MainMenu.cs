using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        // Cambia "Nivel1" por el nombre exacto de tu escena de juego
        SceneManager.LoadScene("Tutorial");
    }

    public void OpenConfig()
    {
        // Aquí podrías activar un Panel de configuración
    }   

    public void ExitGame()
    {
        Application.Quit();
    }
}