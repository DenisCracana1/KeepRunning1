using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AuthUIHandler : MonoBehaviour
{
    [Header("Referencias de UI")]
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TextMeshProUGUI errorText; // Opcional: un texto para avisar al usuario

    [Header("Paneles")]
    public GameObject authPanel;
    public GameObject mainMenuPanel;

    private void Start()
    {
        // Limpiamos el texto de error al empezar
        if (errorText != null) errorText.text = "";
    }

    // BOTÓN DE LOGIN
    public void OnLoginClick()
    {
        string user = usernameInput.text;
        string pass = passwordInput.text;

        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
        {
            ShowError("Introduce usuario y contraseña");
            return;
        }

        // Llamamos al ServerManager
        ServerManager.Instance.Login(user, pass, (success) => {
            if (success)
            {
                // SI EXISTE Y ES CORRECTO: Pasamos al siguiente panel
                authPanel.SetActive(false);
                mainMenuPanel.SetActive(true);
                Debug.Log("Acceso concedido.");
            }
            else
            {
                // SI NO EXISTE O FALLA: No hacemos nada y avisamos
                ShowError("Usuario o contraseña incorrectos");
                Debug.LogWarning("Acceso denegado.");
            }
        });
    }

    // BOTÓN DE REGISTRO
    public void OnRegisterClick()
    {
        string user = usernameInput.text;
        string pass = passwordInput.text;

        if (user.Length < 3 || pass.Length < 3)
        {
            ShowError("Demasiado corto (mínimo 3 caracteres)");
            return;
        }

        ServerManager.Instance.Register(user, pass, (success) => {
            if (success)
            {
                ShowError("¡Cuenta creada! Ya puedes loguear.");
                // Opcional: podrías limpiar los campos aquí
                passwordInput.text = "";
            }
            else
            {
                ShowError("El usuario ya existe o error de red");
            }
        });
    }

    private void ShowError(string message)
    {
        if (errorText != null) errorText.text = message;
        Debug.Log(message);
    }
}