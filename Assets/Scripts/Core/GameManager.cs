using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int deathCount;
    public Transform respawnPoint;
    public PlayerController player;

    public int userId;       
    public int totalScore;   

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void PlayerDied()
    {
        deathCount++;
        RespawnPlayer();
    }

    public void RespawnPlayer()
    {
        if (player == null || respawnPoint == null)
            return;

        player.transform.position = respawnPoint.position;
        player.rb.linearVelocity = Vector2.zero;

        player.dashesLeft = player.maxDashes;
        player.currentStamina = player.maxStamina;

        player.stateMachine.ChangeState(player.idleState);
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SendFinalScore()
    {
        var server = FindFirstObjectByType<ServerManager>();
        StartCoroutine(server.SendScore(userId, totalScore));
    }
}
