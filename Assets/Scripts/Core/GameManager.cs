using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<GameManager>();

                if (_instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    _instance = go.AddComponent<GameManager>();
                }
            }

            return _instance;
        }
    }

    public GameObject playerPrefab;
    public PlayerController player;
    public Transform respawnPoint;

    public AudioClip respawnSound;

    private AudioSource audioSource;

    private TextMeshProUGUI timerText;
    private TextMeshProUGUI deathText;

    public int deathCount;
    public float timeElapsed;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject sp = GameObject.Find("SpawnPoint");

        if (sp != null)
        {
            respawnPoint = sp.transform;
        }
        else
        {
            Debug.LogError("No existe un objeto llamado SpawnPoint");
        }

        GameObject timerObj = GameObject.Find("TimerText");

        if (timerObj != null)
        {
            timerText = timerObj.GetComponent<TextMeshProUGUI>();
        }

        GameObject deathObj = GameObject.Find("DeathText");

        if (deathObj != null)
        {
            deathText = deathObj.GetComponent<TextMeshProUGUI>();
        }

        HandlePlayer();

        UpdateUI();
    }

    void HandlePlayer()
    {
        player = FindFirstObjectByType<PlayerController>();

        if (player == null)
        {
            if (playerPrefab != null && respawnPoint != null)
            {
                GameObject newPlayer = Instantiate(
                    playerPrefab,
                    respawnPoint.position,
                    Quaternion.identity
                );

                player = newPlayer.GetComponent<PlayerController>();
            }
        }
        else
        {
            if (respawnPoint != null)
            {
                player.transform.position = respawnPoint.position;

                if (player.rb != null)
                {
                    player.rb.linearVelocity = Vector2.zero;
                }
            }
        }

        CameraController cam =
            Object.FindFirstObjectByType<CameraController>();

        if (cam != null && player != null)
        {
            cam.SetTarget(player);
        }
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;

        UpdateUI();

        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartLevel();
        }
    }

    void UpdateUI()
    {
        if (timerText != null)
        {
            // --- CÁLCULO DE TIEMPO CON MILISEGUNDOS ---
            int min = Mathf.FloorToInt(timeElapsed / 60);
            int sec = Mathf.FloorToInt(timeElapsed % 60);
            int fraction = Mathf.FloorToInt((timeElapsed * 100) % 100); // Milisegundos (0-99)

            // Formato: 00:00:00
            timerText.text = string.Format("{0:00}:{1:00}:{2:00}", min, sec, fraction);
        }

        if (deathText != null)
        {
            deathText.text = "Muertes: " + deathCount;
        }
    }

    public void PlayerDied()
    {
        deathCount++;

        RespawnPlayer();
    }

    public void RespawnPlayer()
    {
        if (player != null && respawnPoint != null)
        {
            player.transform.position = respawnPoint.position;

            if (player.rb != null)
            {
                player.rb.linearVelocity = Vector2.zero;
            }

            if (respawnSound != null)
            {
                audioSource.PlayOneShot(respawnSound);
            }
        }
    }

    public void RestartLevel()
    {
        timeElapsed = 0f;
        deathCount = 0;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    public void NextLevel()
    {
        if (ServerManager.Instance != null)
        {
            ServerManager.Instance.SaveScore(
                SceneManager.GetActiveScene().buildIndex,
                timeElapsed,
                deathCount
            );
        }

        int nextIndex =
            SceneManager.GetActiveScene().buildIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            timeElapsed = 0f;
            deathCount = 0;

            SceneManager.LoadScene(nextIndex);
        }
    }
}