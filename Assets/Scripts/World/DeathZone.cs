using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Comprobamos si lo que ha caído es el jugador
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Jugador caído al vacío. Procesando muerte...");

            // Llamamos al método que ya tienes en el GameManager
            // Este método ya suma la muerte, avisa a la API y te hace respawn
            GameManager.Instance.PlayerDied();
        }
    }
}