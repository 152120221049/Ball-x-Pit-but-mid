using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening; // Animasyon için (Opsiyonel)

public class GameLevelSelector : MonoBehaviour
{
    [Header("UI Referansları")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI levelNameText;// "Seviye 1" yazan yer
    public TextMeshProUGUI descriptionText; // "Isınma turu" yazan yer
    public Image levelImage;                // Ortadaki kare resim
    public Button leftButton;
    public TextMeshProUGUI leftext;
    public TextMeshProUGUI righttext;// < Butonu
    public Button rightButton;              // > Butonu
    public Button playButton;               // Ortadaki "Oyna/Başla" butonu (veya resmin kendisi)

    private int selectedIndex = 0; // Şu an ekranda hangi levelı gösteriyoruz?

    void Start()
    {
        // Başlangıçta 0. indeksi (Level 1) göster
        UpdateUI();

        // Butonları bağla
        leftButton.onClick.AddListener(PrevLevel);
        rightButton.onClick.AddListener(NextLevel);
        playButton.onClick.AddListener(PlaySelectedLevel);
    }

    void UpdateUI()
    {
        if (ProgressionManager.Instance == null) return;
        var allLevels = ProgressionManager.Instance.allLevels;
        if (allLevels.Count == 0) return;

        // İndeks sınırlarını koru
        if (selectedIndex < 0) selectedIndex = 0;
        if (selectedIndex >= allLevels.Count) selectedIndex = allLevels.Count - 1;

        // Veriyi çek ve UI doldur (Burası aynı)
        LevelData data = allLevels[selectedIndex];
        levelNameText.text = data.levelName;
        descriptionText.text = data.description;
        if (levelImage != null) levelImage = data.lvlImage;

        // --- BUTON KISITLAMALARI (BURASI DEĞİŞTİ) ---

        // Sol Ok: İlk levelda değilsek aktiftir
        leftButton.interactable = (selectedIndex > 0);
        if(selectedIndex > 0)
        {
            leftext.color = Color.white; // Normal renk
        }
        else
        {
            leftext.color = Color.gray; // Kısıtlı renk
        }

        // Sağ Ok: 
        // 1. Liste sonu değilse (Daha level var mı?)
        // 2. VE Kilit açık mı? (Bir sonraki levelın indeksi <= açılan en son level)
        bool isNextLevelExists = (selectedIndex < allLevels.Count - 1);
        bool isNextLevelUnlocked = ((selectedIndex + 1) <= ProgressionManager.Instance.maxUnlockedLvlIndex);

        if (isNextLevelExists && isNextLevelUnlocked)
        {
            rightButton.interactable = true;
            righttext.color = Color.white; // Normal renk
        }
        else
        {
            rightButton.interactable = false;
            righttext.color = Color.gray; // Kısıtlı renk

        }
    }

    public void NextLevel()
    {
        selectedIndex++;
        UpdateUI();
    }

    public void PrevLevel()
    {
        selectedIndex--;
        UpdateUI();
    }

    public void PlaySelectedLevel()
    {
        if (ProgressionManager.Instance != null)
        {
            Debug.Log($"Level Başlatılıyor: {selectedIndex}");
            ProgressionManager.Instance.SelectLevelAndStart(selectedIndex);
        }
    }
}