using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Configuración del Nivel")]
    public int levelId = 1;

    [Header("Referencias")]
    public PlayerController player;
    public Transform spawnPoint;

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
        GameObject sp = GameObject.FindWithTag("SpawnPoint");

        if (sp != null)
        {
            spawnPoint = sp.transform;
        }
        else
        {
            Debug.LogWarning("No se encontró ningún objeto con el tag SpawnPoint");
            return;
        }

        if (playerPrefab != null)
        {
            // CORREGIDO: "Quaternion.identity" en lugar de la palabra rara de antes
            GameObject newPlayerObj = Instantiate(
                playerPrefab,
                spawnPoint.position,
                Quaternion.identity
            );

            player = newPlayerObj.GetComponent<PlayerController>();

            if (GameManager.Instance != null)
            {
                GameManager.Instance.player = player;
            }

            CameraController camControl = Object.FindFirstObjectByType<CameraController>();

            if (camControl != null)
            {
                camControl.SetTarget(player);
            }

            Debug.Log("Todo conectado: Player, Cámara y GameManager.");
            StartLevel();
        }
        else
        {
            Debug.LogWarning("Player Prefab no asignado en LevelManager");
        }
    }

    void Update()
    {
        if (isLevelActive)
        {
            timer += Time.deltaTime;
        }
    }

    public void StartLevel()
    {
        timer = 0f;
        deathCount = 0;
        isLevelActive = true;
        Debug.Log("Nivel iniciado");
    }

    public void RegisterDeath()
    {
        deathCount++;
        Debug.Log("Muertes en este nivel: " + deathCount);
        if (player != null && spawnPoint != null) player.transform.position = spawnPoint.position;
    }

    public void FinishLevel()
    {
        isLevelActive = false;
        Debug.Log("¡Nivel completado! Tiempo: " + timer + "s | Muertes: " + deathCount);

        if (ServerManager.Instance != null)
        {
            ServerManager.Instance.SaveScore(levelId, timer, deathCount);
        }
    }
}