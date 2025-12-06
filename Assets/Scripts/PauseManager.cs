using UnityEngine;
using UnityEngine.SceneManagement; // Sahne yönetimi için
using TMPro; // Hız yazısı için (Opsiyonel)
using DG.Tweening;

public class PauseManager : MonoBehaviour
{
    public static bool isGamePaused = false;

    [Header("UI Referansları")]
    public GameObject pauseMenuUI;     // Açılacak siyah panel
    public GameObject gameInterfaceUI; // Gizlenecek oyun içi butonlar (Joystick vb.)
    public TextMeshProUGUI speedText;  // "Hız: 1x" yazısı (Varsa)

    [Header("Ayarlar")]
    public float targetGameSpeed = 1f; // Seçilen hız (1x, 2x...)

    
    void Awake() // Veya Start 1x, 2x but
    {
        // Sahne her yüklendiğinde, oyunun "Durmamış" olduğunu garanti et
        isGamePaused = false;
        Time.timeScale = 1f;
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

        if (speedText != null) speedText.text = $"Hız: {speed}x";
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