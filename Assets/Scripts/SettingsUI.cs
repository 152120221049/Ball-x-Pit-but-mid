using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        // 1. Önce kayıtlı ses değerlerini yükleyelim (varsayılan 0.75f)
        float savedMusic = PlayerPrefs.GetFloat("MusicVol", 0.75f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVol", 0.75f);

        // 2. Slider'ları bu değerlere eşitleyelim
        musicSlider.value = savedMusic;
        sfxSlider.value = savedSFX;

        // 3. Oyun açıldığında sesi mixer'a uygulayalım
        AudioManager.Instance.SetMusicVolume(savedMusic);
        AudioManager.Instance.SetSFXVolume(savedSFX);

        // 4. Slider değiştikçe fonksiyon çalışsın (Kodla bağlama)
        musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);
    }

    void OnMusicSliderChanged(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
        // Anlık kaydetmek istersen:
        PlayerPrefs.SetFloat("MusicVol", value);
    }

    void OnSfxSliderChanged(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
        PlayerPrefs.SetFloat("SFXVol", value);
    }
}