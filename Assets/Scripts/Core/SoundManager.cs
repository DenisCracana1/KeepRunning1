using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public Slider slider;

    void Start()
    {
        slider = Object.FindAnyObjectByType<Slider>();
        // Carregar volum guardat (0.5 per defecte)
        float savedVolume = PlayerPrefs.GetFloat("volume", 0.5f);

        // Aplicar volum
        AudioListener.volume = savedVolume;

        // Si hi ha slider a l'escena, actualitzar-lo
        if (slider != null) 
            slider.value = savedVolume;
    }

    public void ChangeVolume(float valor)
    {
        // Aplicar volum
        AudioListener.volume = valor;

        // Guardar volum
        PlayerPrefs.SetFloat("volume", valor);
    }
}