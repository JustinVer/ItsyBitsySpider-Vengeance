using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;

    [SerializeField] Slider masterVolume;
    [SerializeField] Slider soundEffectsVolume;
    [SerializeField] Slider backgroundMusicVolume;

    public void LoadSettings()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            audioMixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat("MasterVolume"));
            masterVolume.value = PlayerPrefs.GetFloat("MasterVolume");
        }
        if (PlayerPrefs.HasKey("SoundEffectsVolume"))
        {
            audioMixer.SetFloat("SoundEffectsVolume", PlayerPrefs.GetFloat("SoundEffectsVolume"));
            soundEffectsVolume.value = PlayerPrefs.GetFloat("SoundEffectsVolume");
        }
        if (PlayerPrefs.HasKey("BackgroundMusicVolume"))
        {
            audioMixer.SetFloat("BackgroundMusicVolume", PlayerPrefs.GetFloat("BackgroundMusicVolume"));
            backgroundMusicVolume.value = PlayerPrefs.GetFloat("BackgroundMusicVolume");
        }
        MenuManager.Instance.OptionsMenu(false);
    }


    public void MainMenu()
    {
        PlayerPrefs.Save();
        MenuManager.Instance.OptionsMenu(false);
    }

    public void masterVolumeSlider()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume.value);
        audioMixer.SetFloat("MasterVolume", masterVolume.value);
        PlayerPrefs.Save();
    }
    public void soundEffectsVolumeSlider()
    {
        PlayerPrefs.SetFloat("SoundEffectsVolume", soundEffectsVolume.value);
        audioMixer.SetFloat("SoundEffectsVolume", soundEffectsVolume.value);
        PlayerPrefs.Save();
    }
    public void backgroundVolumeSlider()
    {
        PlayerPrefs.SetFloat("BackgroundMusicVolume", backgroundMusicVolume.value);
        audioMixer.SetFloat("BackgroundMusicVolume", backgroundMusicVolume.value);
        PlayerPrefs.Save();
    }
}
