using UnityEngine;
using UnityEngine.SceneManagement; // Sahne yüklemek için
using DG.Tweening;
using TMPro; // Animasyon için

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [Header("UI Referansları")]
    public GameObject gameOverPanel;
    public GameObject gameInterface; // Joystick, Pause butonu vb. gizlemek için
    public TextMeshProUGUI survivalTimeText;
    void Awake()
    {
        Instance = this;
    }

    
    public void ShowGameOver()
    {
        LevelManager.Instance.CashOutXP();
        if (gameInterface != null)
            gameInterface.SetActive(false);

        // 2. Paneli Aç
        gameOverPanel.SetActive(true);
        if (DifficultyManager.Instance != null && survivalTimeText != null)
        {
            float time = DifficultyManager.Instance.timeAlive;

            // Saniyeyi Dakika ve Saniyeye çevirme matematiği
            int minutes = Mathf.FloorToInt(time / 60F);
            int seconds = Mathf.FloorToInt(time % 60F);

            // Format: "02:05" şeklinde yazdır
            survivalTimeText.text = string.Format("Hayatta Kalınan Süre\n<size=150%>{0:00}:{1:00}</size>", minutes, seconds);
        }

        // 3. DOTween ile Animasyonlu Giriş (Büyüyerek gelme)
        gameOverPanel.transform.localScale = Vector3.zero; // Önce küçült
        gameOverPanel.transform.DOScale(1f, 0.5f)
            .SetEase(Ease.OutBack)
            .SetUpdate(true); // KRİTİK: Oyun durunca da çalışsın diye

        // 4. Oyunu Durdur (Biraz gecikmeli durdurabiliriz ama şimdilik anında dursun)
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Zamanı düzeltmeyi unutma!
        // Şu anki sahneyi yeniden yükle
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}