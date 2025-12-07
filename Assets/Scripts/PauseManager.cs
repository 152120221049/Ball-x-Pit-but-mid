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
    

    public Image X1Button;
    public Image X2Button;
    public Image XhalfButton;
    public Color activeColor = Color.green; // Seçiliyken ne renk olsun?
    public Color inactiveColor = Color.white; // Seçili değilken ne renk olsun?
    [Header("Ayarlar")]
    public float targetGameSpeed = 1f; // Seçilen hız (1x, 2x...)

    
    void Awake() // Veya Start 1x, 2x but
    {
       
        isGamePaused = false;
        Time.timeScale = 1f;
        UpdateSpeedButtonVisuals(targetGameSpeed);
    }
    public void SetGameSpeed(float speed)
    {
        targetGameSpeed = speed;

        // Eğer oyun şu an DURAKLATILMAMIŞSA, hızı hemen uygula.
        // Eğer duraklatılmışsa uygulama, ResumeGame yapınca uygulanacak.
        if (!isGamePaused)
        {
            Time.timeScale = targetGameSpeed;
        }

        UpdateSpeedButtonVisuals(speed);

    }
    void UpdateSpeedButtonVisuals(float speed)
    {
        // Önce hepsini pasif renge çek (Temizle)
        if (X1Button) X1Button.color = inactiveColor;
        if (X2Button) X2Button.color = inactiveColor;
        if (XhalfButton) XhalfButton.color = inactiveColor;

        // Sonra sadece seçili olanı aktif renk yap
        if (speed == 0.5f && XhalfButton) XhalfButton.color = activeColor;
        else if (speed == 1f && X1Button ) X1Button .color = activeColor;
        else if (speed == 2f && X2Button) X2Button.color = activeColor;
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

    // --- YENİDEN BAŞLAT (RESTART) ---
    public void RestartLevel()
    {
        Time.timeScale = 1f; // Hızı normale döndür
        isGamePaused = false;

        LevelManager.Instance.CashOutXP();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // --- ÇIKIŞ (QUIT) ---
    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        isGamePaused = false;
        LevelManager.Instance.CashOutXP();
        SceneManager.LoadScene("MainMenu");
    }
}