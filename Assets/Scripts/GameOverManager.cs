using UnityEngine;
using UnityEngine.SceneManagement; // Sahne yüklemek için
using DG.Tweening; // Animasyon için

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [Header("UI Referansları")]
    public GameObject gameOverPanel;
    public GameObject gameInterface; // Joystick, Pause butonu vb. gizlemek için

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