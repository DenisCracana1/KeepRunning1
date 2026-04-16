using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Configuración del Nivel")]
    public int levelId = 1;

    [Header("Estado Actual")]
    private float timer = 0f;
    private int deathCount = 0;
    private bool isLevelActive = false;

    [Header("Configuración de Inicio")]
    public GameObject playerPrefab;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // 1. Localizar el punto de spawn en la escena
        GameObject sp = GameObject.FindWithTag("SpawnPoint");

        if (sp != null && playerPrefab != null)
        {
            // 2. INSTANCIAR al jugador
            GameObject newPlayerObj = Instantiate(playerPrefab, sp.transform.position, Quaternion.identity);
            PlayerController newPlayerScript = newPlayerObj.GetComponent<PlayerController>();

            // 3. VINCULAR con el GameManager (indispensable para el Respawn)
            if (GameManager.Instance != null)
            {
                GameManager.Instance.player = newPlayerScript;
                GameManager.Instance.respawnPoint = sp.transform;
            }

            // 4. VINCULAR con la Cámara (para que deje de estar estática)
            CameraController camControl = Object.FindFirstObjectByType<CameraController>();
            if (camControl != null)
            {
                camControl.SetTarget(newPlayerScript);
            }

            Debug.Log(" Todo conectado: Player, Cámara y GameManager.");

            // 5. INICIAR EL NIVEL
            // Lo llamamos aquí dentro para asegurar que el juego no empiece 
            // hasta que el jugador ya esté en su sitio.
            StartLevel();
        }

        StartLevel(); // Tu lógica de la API
    }

    void Update()
    {
        if (isLevelActive)
        {
            timer += Time.deltaTime; // Suma el tiempo real cada segundo
        }
    }

    public void StartLevel()
    {
        timer = 0f;
        deathCount = 0;
        isLevelActive = true;

    }

    // Llama a esto cada vez que el jugador muera
    public void RegisterDeath()
    {
        deathCount++;
        Debug.Log("Muertes en este nivel: " + deathCount);
    }

    // Llama a esto cuando el jugador toque la Meta (Trigger)
    public void FinishLevel()
    {
        isLevelActive = false;
        Debug.Log("¡Nivel completado! Tiempo: " + timer + "s | Muertes: " + deathCount);

        // Enviamos los datos al ServerManager (que ya sabe quién es el usuario)
        ServerManager.Instance.SaveScore(levelId, timer, deathCount);
    }
}