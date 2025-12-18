using UnityEngine;
using UnityEngine.SceneManagement; // Sahne yönetimi için
using TMPro; // Hız yazısı için (Opsiyonel)
using DG.Tweening;
using UnityEngine.UI;
public class PauseManager : MonoBehaviour
{
    public static bool isGamePaused = false;

    [Header("UI Referansları")]
    public GameObject pauseMenuUI;     // Açılacak siyah panel
    public GameObject gameInterfaceUI; // Gizlenecek oyun içi butonlar (Joystick vb.)
    public Slider speedSlider;
    public GameObject settingsUI;
    public TextMeshProUGUI speedText;
    [Header("Ayarlar")]
    public static float targetGameSpeed = 1f; // Seçilen hız (1x, 2x...)

    private void Start()
    {
        if (speedSlider != null)
        {
            // DÜZELTME 1: Slider değeri, Hızın 2 katı olmalı (Ters İşlem)
            // Örn: Hız 1.5 ise Slider 3 olmalı.
            speedSlider.value = targetGameSpeed * 2;

            UpdateSpeedText(speedSlider.value);
        }

        // Sahne başladığında seçili hızı hemen uygula (Restart sonrası için)
        Time.timeScale = targetGameSpeed;
    }

    void UpdateSpeedText(float sliderValue)
    {
        if (speedText != null)
            speedText.text = $"{sliderValue / 2f}x";
    }

    public void OnSpeedSliderChanged(float value)
    {
        // Slider 1, 2, 3, 4 değerlerini verir.
        // Hız 0.5, 1.0, 1.5, 2.0 olur.
        targetGameSpeed = value / 2f;

        UpdateSpeedText(value);

        if (!isGamePaused)
        {
            Time.timeScale = targetGameSpeed;
        }
    }
    // --- DURDUR (PAUSE) ---
    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        if (gameInterfaceUI != null) gameInterfaceUI.SetActive(false);
        Time.timeScale = 0f; 
        isGamePaused = true;
    }

    // --- DEVAM ET (RESUME) ---
    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        if (gameInterfaceUI != null) gameInterfaceUI.SetActive(true);

        // ÖNEMLİ: 1f değil, oyuncunun seçtiği hızı geri yüklüyoruz
        Time.timeScale = targetGameSpeed;

        isGamePaused = false;
    }
    public void Settings(bool isActive)
    {
        pauseMenuUI.SetActive(!isActive);
        settingsUI.SetActive(isActive);
    }
    // --- YENİDEN BAŞLAT (RESTART) ---
    public void RestartLevel()
    {
        Time.timeScale = targetGameSpeed/2; // Hızı normale döndür
        isGamePaused = false;

        LevelManager.Instance.CashOutXP();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // --- ÇIKIŞ (QUIT) ---
    public void QuitToMainMenu()
    {
        Time.timeScale = targetGameSpeed/2;
        isGamePaused = false;
        LevelManager.Instance.CashOutXP();
        SceneManager.LoadScene("MainMenu");
    }
}