using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;
using UnityEngine.UI; // Buton Image rengi için

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [Header("UI Referansları")]
    public GameObject resultPanel;   // Siyah Panel (Eski GameOverPanel)
    public GameObject gameInterface; // Joystick vb.

    [Header("Değişen Alanlar")]
    public TextMeshProUGUI titleText;       // "Oyun Bitti" yazan yer
    public TextMeshProUGUI survivalTimeText; // Süre yazısı
    public Button actionButton;             // Ortadaki ana buton (Restart/Next)
    public TextMeshProUGUI actionButtonText; // Butonun içindeki yazı
    public Image panelBackground;           // Panelin rengini değiştirmek istersen

    private bool isVictory = false; // O anki durum

    void Awake()
    {
        Instance = this;
    }

    // Bu fonksiyonu hem PlayerHealth (false) hem WaveSpawner (true) çağıracak
    public void ShowResult(bool hasWon)
    {
        isVictory = hasWon;

        // 1. Arayüzü Temizle
        if (gameInterface != null) gameInterface.SetActive(false);
        resultPanel.SetActive(true);

        // 2. Duruma Göre Metin ve Renk Ayarla
        if (isVictory)
        {
            // --- KAZANMA DURUMU ---
            titleText.text = "PAYDOS!\n(BAŞARILI)";
            titleText.color = Color.green;

            actionButtonText.text = "SANAYİYE DÖN"; // Veya "SONRAKİ İŞ"

            // Eğer paneli yeşertmek istersen:
            // if(panelBackground) panelBackground.color = new Color(0, 0.5f, 0, 0.9f);

            // Verileri Kaydet (Seviye İlerlemesi)
            if (ProgressionManager.Instance != null)
            {
                // Şu an oynanan levelın indeksini al
                int currentLvl = ProgressionManager.Instance.currentLevelIndex;

                // Bir sonrakini aç
                ProgressionManager.Instance.UnlockNextLevel(currentLvl);
            }
        }
        else
        {
            // --- KAYBETME DURUMU ---
            titleText.text = "İŞ KAZASI\n(YENİLGİ)";
            titleText.color = Color.red;

            actionButtonText.text = "TEKRAR DENE";

            // if(panelBackground) panelBackground.color = new Color(0.5f, 0, 0, 0.9f);
        }

        // 3. Süreyi Yaz (Ortak)
        if (DifficultyManager.Instance != null && survivalTimeText != null)
        {
            float time = DifficultyManager.Instance.timeAlive;
            int minutes = Mathf.FloorToInt(time / 60F);
            int seconds = Mathf.FloorToInt(time % 60F);
            survivalTimeText.text = string.Format("Mesai Süresi: {0:00}:{1:00}", minutes, seconds);
        }

        // 4. Animasyon ve Durdurma (Ortak)
        resultPanel.transform.localScale = Vector3.zero;
        resultPanel.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);

        // XP'yi Kasaya At (Her iki durumda da kazandığını alır)
        if (LevelManager.Instance != null) LevelManager.Instance.CashOutXP();

        Time.timeScale = 0f;
    }

    // Butona Tıklanınca Çalışacak Fonksiyon
    public void OnActionClicked()
    {
        Time.timeScale = 1f; // Zamanı düzelt

        if (isVictory)
        {
            // Kazanıldıysa -> Ana Menüye / Sanayiye dön (GDD Akışı)
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            // Kaybedildiyse -> Bölümü Baştan Başlat
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    // Ana Menü Butonu (Varsa ayrıca çalışır)
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}