using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public AudioSource[] allAudioSources; // Wszystkie �r�d�a d�wi�ku w grze
    public Button muteButton; // Przycisk UI do wyciszania
    public Sprite soundOnIcon; // Ikona w��czonego d�wi�ku
    public Sprite soundOffIcon; // Ikona wyciszonego d�wi�ku
    private bool isMuted;

    private void Start()
    {
        isMuted = PlayerPrefs.GetInt("Muted", 0) == 1; // Wczytanie stanu z pami�ci
        UpdateAudioState();
        UpdateButtonIcon();
        muteButton.onClick.AddListener(ToggleMute);
    }

    private void ToggleMute()
    {
        isMuted = !isMuted;
        PlayerPrefs.SetInt("Muted", isMuted ? 1 : 0); // Zapisanie stanu
        PlayerPrefs.Save();

        UpdateAudioState();
        UpdateButtonIcon();
    }

    private void UpdateAudioState()
    {
        foreach (AudioSource audioSource in allAudioSources)
        {
            audioSource.mute = isMuted;
        }
    }

    private void UpdateButtonIcon()
    {
        if (muteButton != null)
        {
            muteButton.image.sprite = isMuted ? soundOffIcon : soundOnIcon;
        }
    }
}
