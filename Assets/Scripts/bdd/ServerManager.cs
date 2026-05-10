using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System.Collections.Generic;

public class ServerManager : MonoBehaviour
{
    public static ServerManager Instance;

    [Header("Configuració")]
    [SerializeField] private string urlBase = "http://keeprunning.somee.com/api/";

    [Header("Sessió")]
    public LoginResponse LocalUser;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); LoadSession(); }
        else { Destroy(gameObject); }
    }

    // --- MÈTODES PÚBLICS ---

    public void Login(string user, string pass, System.Action<bool> onResult)
    {
        UserAuth auth = new UserAuth { username = user, password = pass };
        StartCoroutine(PostRequest("Users/login", JsonUtility.ToJson(auth), (json) => {
            LocalUser = JsonUtility.FromJson<LoginResponse>(json);
            SaveSession(json);
            onResult?.Invoke(true);
        }, (err) => onResult?.Invoke(false)));
    }

    public void Register(string user, string pass, System.Action<bool> onResult)
    {
        UserAuth auth = new UserAuth { username = user, password = pass };
        StartCoroutine(PostRequest("Users/register", JsonUtility.ToJson(auth), (json) => {
            onResult?.Invoke(true);
        }, (err) => onResult?.Invoke(false)));
    }

    public void SaveScore(int levelId, float time, int deaths)
    {
        if (LocalUser == null) return;
        LevelResult result = new LevelResult
        {
            userId = LocalUser.id,
            levelId = levelId,
            timeSpent = time,
            deaths = deaths
        };
        // Segons el teu Swagger (imatge image_ceba13.png), la ruta és stats/save
        StartCoroutine(PostRequest("stats/save", JsonUtility.ToJson(result), null, null));
    }

    // --- MOTOR HTTP ---

    IEnumerator PostRequest(string endpoint, string json, System.Action<string> success, System.Action<string> error)
    {
        string fullUrl = urlBase + endpoint;
        using (UnityWebRequest request = new UnityWebRequest(fullUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("OK: " + endpoint);
                success?.Invoke(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("ERROR a " + fullUrl + ": " + request.error);
                error?.Invoke(request.error);
            }
        }
    }

    private void SaveSession(string json) { PlayerPrefs.SetString("UserSession", json); PlayerPrefs.Save(); }
    private void LoadSession()
    {
        if (PlayerPrefs.HasKey("UserSession"))
            LocalUser = JsonUtility.FromJson<LoginResponse>(PlayerPrefs.GetString("UserSession"));
    }
}