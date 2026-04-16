using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Referencias de Jugador")]
    public Transform respawnPoint;
    public PlayerController player;

    [Header("Estado Global")]
    public int deathCount;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- LÓGICA DE MUERTE ---
    public void PlayerDied()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.RegisterDeath();
        }
        RespawnPlayer();
    }

    public void RespawnPlayer()
    {
        if (player == null || respawnPoint == null) return;
        player.transform.position = respawnPoint.position;
        if (player.rb != null) player.rb.linearVelocity = Vector2.zero;

        // Reset de mecánicas (ajusta según tu PlayerController)
        player.dashesLeft = player.maxDashes;
        player.currentStamina = player.maxStamina;

        if (player.stateMachine != null)
            player.stateMachine.ChangeState(player.idleState);
    }

    // --- LÓGICA DE NIVELES AUTOMÁTICA ---
    public void NextLevel()
    {
        // 1. Enviamos los datos del nivel actual a la nube antes de salir
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.FinishLevel();
        }

        // 2. Calculamos el índice de la siguiente escena
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        // 3. Comprobamos si existe una siguiente escena en el Build Settings
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log("Cargando siguiente nivel índice: " + nextSceneIndex);
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("¡Has llegado al final del juego! No hay más niveles en Build Settings.");
            // Aquí podrías cargar una escena de "Créditos" o "Victoria"
        }
    }
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartLevel();
        }
    }

    public void RestartLevel()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}