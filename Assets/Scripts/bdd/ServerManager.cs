using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class ServerManager : MonoBehaviour
{
    public static ServerManager Instance;

    [Header("Configuracion del Servidor")]
    [Tooltip("URL de la API. Cambiar localhost por la IP del servidor cuando este en la nube.")]
    [SerializeField] private string urlBase = "https://localhost:5001/api/";

    [Header("Datos de Sesion Actual")]
    public LoginResponse LocalUser; // Aquí guardamos TODO el paquete que recibimos

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSession(); // Intentamos recuperar sesión anterior
        }
        else { Destroy(gameObject); }
    }

    // --- MÉTODOS DE LA API ---

    public void Login(string user, string pass, System.Action<bool> onResult)
    {
        UserAuth auth = new UserAuth { username = user, password = pass };
        StartCoroutine(PostRequest("users/login", JsonUtility.ToJson(auth), (json) => {
            LocalUser = JsonUtility.FromJson<LoginResponse>(json);
            SaveSession(json); // Guardamos en el PC para no tener que loguear siempre
            onResult?.Invoke(true);
        }));
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

        StartCoroutine(PostRequest("users/save-score", JsonUtility.ToJson(result), (res) => {
            Debug.Log("Datos sincronizados con la nube correctamente.");
        }));
    }

    // --- UTILIDADES DE PERSISTENCIA LOCAL ---

    private void SaveSession(string json)
    {
        PlayerPrefs.SetString("UserSession", json);
        PlayerPrefs.Save();
    }

    private void LoadSession()
    {
        if (PlayerPrefs.HasKey("UserSession"))
        {
            string json = PlayerPrefs.GetString("UserSession");
            LocalUser = JsonUtility.FromJson<LoginResponse>(json);
            Debug.Log("Sesion recuperada: " + LocalUser.username);
        }
    }

    // --- MOTOR DE PETICIONES HTTP ---

    IEnumerator PostRequest(string endpoint, string json, System.Action<string> successCallback)
    {
        using (UnityWebRequest request = new UnityWebRequest(urlBase + endpoint, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                successCallback?.Invoke(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error de Red: " + request.error);
            }
        }
    }
}