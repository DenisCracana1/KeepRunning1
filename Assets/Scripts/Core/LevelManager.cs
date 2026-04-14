using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int levelNumber = 1;

    private float timer = 0f;
    private int attempts = 0;
    private int levelScore = 0;

    private bool levelRunning = true;

    void Update()
    {
        if (levelRunning)
            timer += Time.deltaTime;
    }

    public void AddAttempt()
    {
        attempts++;
    }

    public void AddScore(int amount)
    {
        levelScore += amount;
    }

    public void CompleteLevel()
    {
        levelRunning = false;

        int userId = GameManager.Instance.userId;

        StartCoroutine(FindFirstObjectByType<ServerManager>().SendLevelScore(userId, levelNumber, levelScore));
        StartCoroutine(FindFirstObjectByType<ServerManager>().SendLevelAttempts(userId, levelNumber, attempts));
        StartCoroutine(FindFirstObjectByType<ServerManager>().SendLevelTime(userId, levelNumber, timer));

        Debug.Log("Nivel completado");
    }
}
