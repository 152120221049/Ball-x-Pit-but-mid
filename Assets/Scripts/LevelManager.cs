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
    public float totalKula = 0;

    [Header("Oyuncu Statları")]
    public float damageMultiplier = 1.0f;
    public float cooldownReduction = 1.0f;

    // --- YENİ EKLENENLER ---
    [Header("Özel Etki Güçlendirmesi")]
    // Hamster patlaması, Tesla çarpanı, Buz hasarı buna göre artacak.
    // 1.0 = Normal (%100), 1.5 = %50 daha güçlü
    public float effectEnhance = 1.0f;

    [Header("Ödül Ayarları")]
    public ItemData lemonData; // Hediye edilecek Limon ItemData'sı
    public GameObject confettiPrefab; // Level atlayınca çıkacak efekt

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateUI();
    }

    [System.Obsolete]
    public void AddExp(float amount)
    {
        currentExp += amount;
        totalKula += amount;
        if (currentExp >= targetExp)
        {
            LevelUp();
        }
        UpdateUI();
    }

    [System.Obsolete]
    void LevelUp()
    {
        currentExp -= targetExp;
        currentLevel++;
        targetExp *= 1.2f;

        string rewardText = "";
        bool showConfetti = false;

        // --- ŞANS ÇARKI (RNG) ---
        int roll = Random.Range(0, 100);

        // %20 İhtimal: GEÇİCİ LİMON DESTEĞİ
        if (roll < 20)
        {
            if (lemonData != null)
            {
                // Sahnedeki WeaponSystem'i bul ve limon ekle
                WeaponSystem ws = FindObjectOfType<WeaponSystem>();
                if (ws != null) ws.AddTemporaryItem(lemonData, 1);

                rewardText = "BONUS LİMON!";
                showConfetti = true;
            }
        }
        // %40 İhtimal: EFFECT ENHANCE (Tesla, Buz, Hamster güçlenir)
        else if (roll < 60)
        {
            effectEnhance += 0.2f; // %20 Güçlendir
            rewardText = "BÜYÜ GÜCÜ ARTTI! (+%20)";
            showConfetti = true;
        }
        // %40 İhtimal: KLASİK STATLAR (Hasar veya Hız)
        else
        {
            if (Random.value > 0.5f)
            {
                damageMultiplier += 0.1f;
                rewardText = "Hasar Arttı";
            }
            else
            {
                cooldownReduction += 0.05f;
                rewardText = "Hız Arttı";
            }
        }

        Debug.Log($"LEVEL {currentLevel}: {rewardText}");

        // --- KONFETİ EFEKTİ ---
        if (showConfetti && confettiPrefab != null)
        {
            // Oyuncunun üzerinde patlat
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Instantiate(confettiPrefab, player.transform.position, Quaternion.identity);
            }
        }

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
    public void CashOutXP()
    {
        int totalReward = (int)totalKula; // Örnek formül

        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.AddBankedXP(totalReward);
            Debug.Log($"KASAYA EKLENEN XP: {totalReward}");
        }
    }
}