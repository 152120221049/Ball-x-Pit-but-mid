using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;

    [Header("Veritabanı")]
    public List<ItemData> allUnlockedItems;
    public Dictionary<ItemData, int> itemLevels = new Dictionary<ItemData, int>();
    // Oyuncunun sahip olduğu tüm eşyalar

    [Header("Aktif Deste")]
    // WeaponSystem'deki ItemStack yapısını kullanıyoruz
    public List<ItemStack> currentDeck = new List<ItemStack>();
    [Header("Ekonomi")]
    public int SariKulaReserves = 0;
    public int geliştirmeMasrafi = 100;
    public int maxDeckSize = 8; // Desteye kaç eşya koyabiliriz?
    public int maxBudget = 10;
    void Awake()
    {
        // Singleton Yapısı (Sahneler arası geçişte yok olmasın)
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public int GetItemLevel(ItemData item)
    {
        // Eğer sözlükte bu eşya varsa, levelını döndür
        if (itemLevels.ContainsKey(item))
        {
            return itemLevels[item];
        }

        
        return 1;
    }
    public float GetModifiedDamage(ItemData item)
    {
        int lvl = GetItemLevel(item);
        // Formül: Baz Hasar * (1 + (Büyüme * (Level - 1)))
        // Örnek: 10 hasar, %10 büyüme, Lvl 3 -> 10 * (1 + 0.2) = 12 Hasar
        return item.damage * (1f + (item.damageGrowthPercent * (lvl - 1)));
    }

    // 2. GÜNCEL COOLDOWN HESABI (Her 5 Levelda Bir)
    public float GetModifiedCooldown(ItemData item)
    {
        int lvl = GetItemLevel(item);

        // Kaç tane 5 level devirdik? (Integer bölmesi: 9/5 = 1, 10/5 = 2)
        int milestoneCount = lvl / 5;

        // Toplam azalma oranı
        float totalReduction = milestoneCount * item.cdReductionPer5Levels;

        // Asla %80'den fazla azalmasın (Güvenlik sınırı)
        totalReduction = Mathf.Clamp(totalReduction, 0f, 0.8f);

        // Baz süreden düş
        return item.baseCooldown * (1f - totalReduction);
    }

    // 3. UPGRADE MALİYETİ (Level + Bütçe Bağımlı)
    public int GetUpgradeCost(ItemData item)
    {
        int lvl = GetItemLevel(item);

        // GDD Formülü: Eşyalar için gereken exp eşyaların hem seviyesine hem de bütçesine bağlı.
        // Formül: BazXP * Level * BütçeMaliyeti
        // Örn: Limon (Bütçe 1) Lvl 5 -> 50 * 5 * 1 = 250 XP
        // Örn: Hamster (Bütçe 3) Lvl 5 -> 50 * 5 * 3 = 750 XP
        return item.baseXPCost * lvl * item.budgetCost;
    }
    public bool TryUpgradeItem(ItemData item)
    {
        // Önce maliyeti öğren
        int cost = GetUpgradeCost(item);

        // Parayı (XP) harcamayı dene
        if (TrySpendXP(cost))
        {
            // Para yettiyse ve harcandıysa:

            // Eğer eşya sözlükte varsa seviyesini artır
            if (itemLevels.ContainsKey(item))
            {
                itemLevels[item]++;
            }
            else
            {
                // Sözlükte yoksa (Hata koruması), Level 2 yapıp ekle
                itemLevels.Add(item, 2);
            }

            Debug.Log($"{item.itemName} Yükseltildi! Yeni Seviye: {itemLevels[item]}");
            return true; // İşlem Başarılı
        }

        // Para yetmedi
        return false;
    }
    public void AddBankedXP(int amount)
    {
        SariKulaReserves += amount;
        // Kayıt sistemi (PlayerPrefs) buraya eklenebilir
    }

    public bool TrySpendXP(int amount)
    {
        if (SariKulaReserves >= amount)
        {
            SariKulaReserves -= amount;
            return true; // İşlem Başarılı
        }
        return false; // Yetersiz Bakiye
    }

    public void UpgradeBudget()
    {
        maxBudget++; // Kapasiteyi artır
        geliştirmeMasrafi = Mathf.RoundToInt(geliştirmeMasrafi * 1.5f); // Fiyatı katla (%50 zam)
    }
    void Start()
    {
        // Eğer deste boşsa ve eşyamız varsa, ilk 4 taneyi desteye koy
        if (currentDeck.Count == 0 && allUnlockedItems.Count > 0)
        {
            for (int i = 0; i < Mathf.Min(maxDeckSize, allUnlockedItems.Count); i++)
            {
                ItemStack newItem = new ItemStack();
                newItem.itemData = allUnlockedItems[i];
                newItem.amount = 1; // Varsayılan miktar
                currentDeck.Add(newItem);
            }
        }
    }
    public int GetCurrentDeckCost()
    {
        int totalCost = 0;
        foreach (ItemStack stack in currentDeck)
        {
            if (stack.itemData != null)
            {
                // Maliyet = Eşya Maliyeti * Adet
                totalCost += stack.itemData.budgetCost * stack.amount;
            }
        }
        return totalCost;
    }
}