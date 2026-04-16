using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // El GameManager ya sabe que debe ir al siguiente índice + enviar a la nube
            GameManager.Instance.NextLevel();

            // Desactivamos el portal para evitar que se active varias veces por error
            GetComponent<Collider2D>().enabled = false;
        }
    }
}