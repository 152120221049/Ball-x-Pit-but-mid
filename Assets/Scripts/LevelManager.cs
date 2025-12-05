using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("UI Referansları")]
    public Slider expSlider;
    public TextMeshProUGUI levelText;

    [Header("Level Durumu")]
    public int currentLevel = 1;
    public float currentExp = 0;
    public float targetExp = 100;

    [Header("Oyuncu Statları (Ödüller)")]
    public float damageMultiplier = 1.0f;   // Hasar Çarpanı
    public float cooldownReduction = 1.0f;  // Atış Hızı Çarpanı (1 = %100, 1.2 = %20 daha hızlı)

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateUI();
    }

    public void AddExp(float amount)
    {
        currentExp += amount;
        if (currentExp >= targetExp)
        {
            LevelUp();
        }
        UpdateUI();
    }

    void LevelUp()
    {
        currentExp -= targetExp;
        currentLevel++;
        targetExp *= 1.2f; // Sonraki level %20 daha zor olsun

        // --- RASTGELE GÜÇLENDİRME (RNG) ---
        int roll = Random.Range(0, 100);
        string upgradeName = "";

        if (roll < 60)
        {
            // %60 İhtimal: HASAR ARTIŞI (%5 - %15 arası)
            float randomDmg = Random.Range(0.05f, 0.15f);
            damageMultiplier += randomDmg;
            upgradeName = $"Hasar +%{Mathf.RoundToInt(randomDmg * 100)}";
        }
        else if (roll < 90)
        {
            // %30 İhtimal: ATIŞ HIZI (%2 - %8 arası)
            float randomCdr = Random.Range(0.02f, 0.08f);
            cooldownReduction += randomCdr;
            upgradeName = $"Atış Hızı +%{Mathf.RoundToInt(randomCdr * 100)}";
        }
        else
        {
            // %10 İhtimal: BALLI LOKMA (İkisi Birden)
            damageMultiplier += 0.10f;
            cooldownReduction += 0.05f;
            upgradeName = "SÜPER GÜÇLENDİRME! (Hasar & Hız)";
        }

        Debug.Log($"LEVEL UP! ({currentLevel}) - Kazanılan: {upgradeName}");

        // Eğer exp çok geldiyse tekrar kontrol et
        if (currentExp >= targetExp) LevelUp();
    }

    void UpdateUI()
    {
        if (expSlider != null)
        {
            expSlider.maxValue = targetExp;
            expSlider.value = currentExp;
        }
        if (levelText != null)
        {
            levelText.text = "Lvl " + currentLevel;
        }
    }
}