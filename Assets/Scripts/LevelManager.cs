using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public PlayerHealth playerHealth;
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
        if (PlayerDataManager.Instance != null)
        {
            foreach (PerkBase perk in PlayerDataManager.Instance.equippedPerks)
            {
                // Her takılı perkin 'Level Atladı' fonksiyonunu çalıştır
                perk.OnLevelUp();
            }
        }


        if (Random.value > 0.5f)
            {
                damageMultiplier += 0.1f;
                
            }
            else
            {
                cooldownReduction += 0.05f;
               
            }
        
        if (confettiPrefab != null)
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