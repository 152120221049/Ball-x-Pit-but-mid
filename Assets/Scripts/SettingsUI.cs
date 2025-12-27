using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsUI : MonoBehaviour
{
    [Header("Ses Sliderları")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Kontrol Ayarı")]
    public TMP_Text controlButtonText;
    public TMP_Text explanationText;
    void OnEnable() // Menü her açıldığında verileri tazele
    {
        if (PlayerDataManager.Instance == null) return;
        // 1. Sliderları mevcut verilere eşitle
        float savedMusic = PlayerPrefs.GetFloat("MusicVol", 0.75f);

        float savedSFX = PlayerPrefs.GetFloat("SFXVol", 0.75f);
        musicSlider.value = savedMusic;
        sfxSlider.value = savedSFX;
        // 2. Görseli (Yazıyı) güncelle
        UpdateControlText();

        // 3. Listenerları bağla (Tekrarlanmaması için önce çıkarıp sonra ekliyoruz)
        musicSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();
        musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);
    }

    void OnMusicSliderChanged(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
        PlayerPrefs.SetFloat("MusicVol", value);
    }

    void OnSfxSliderChanged(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
        PlayerPrefs.SetFloat("SFXVol", value);
    }

    // Toggle butonu tarafından çağırılacak fonksiyon
    public void ToggleControlMode()
    {
        ControlMode current = PlayerDataManager.Instance.currentControlMode;
        ControlMode next = (current == ControlMode.DualInput) ? ControlMode.HybridJoystick : ControlMode.DualInput;
        
        PlayerDataManager.Instance.SetControlMode(next);
        UpdateControlText();
    }

    void UpdateControlText()
    {
        if (controlButtonText != null)
        {
            controlButtonText.text = PlayerDataManager.Instance.currentControlMode == ControlMode.HybridJoystick 
                ? " TEK EL" : "ÇİFT EL";
            explanationText.text = PlayerDataManager.Instance.currentControlMode == ControlMode.HybridJoystick 
                ? "Joystickteki dış halkaya kadar kaydırarak hareket et,iç halkada tutarak Nişan al,Otomatik Atış Modunda nişan aldıktan sonra atışlar başlar,Oto atış kapalıyken ekrana tıklayarak joystickle hedeflediğin yöne atış yaparsın."
                : "joystick ile hareket kontrol edilir,Ekranda kaydırarak hedef alır,kaydırırken tıklayarak atış yapılır,Otomatik atış modunda nişan aldığında otomatik atış yapar";
        }
    }
}