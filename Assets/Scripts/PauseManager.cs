using UnityEngine;
using UnityEngine.SceneManagement; // Sahne yönetimi için
using TMPro; // Hız yazısı için (Opsiyonel)

public class PauseManager : MonoBehaviour
{
    public static bool isGamePaused = false;

    [Header("UI Referansları")]
    public GameObject pauseMenuUI;     // Açılacak siyah panel
    public GameObject gameInterfaceUI; // Gizlenecek oyun içi butonlar (Joystick vb.)
    public TextMeshProUGUI speedText;  // "Hız: 1x" yazısı (Varsa)

    [Header("Ayarlar")]
    public float targetGameSpeed = 1f; // Seçilen hız (1x, 2x...)

    // --- HIZ DEĞİŞTİRME ---
    // Bu fonksiyonu UI'daki 1x, 2x butonlarına bağla
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

        Time.timeScale = 0f; // Zamanı tamamen durdur
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

        // Mevcut sahneyi baştan yükle
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // --- ÇIKIŞ (QUIT) ---
    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        isGamePaused = false;

        // "MainMenu" adlı sahneyi yükle (Adı neyse onu yazmalısın)
        // SceneManager.LoadScene("MainMenu"); 
        Debug.Log("Ana Menüye Dönülüyor...");
    }
}