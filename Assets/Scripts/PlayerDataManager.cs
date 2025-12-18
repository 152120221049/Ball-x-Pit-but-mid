using System;
using System.Collections.Generic;
using System.Linq; // Listelerle çalışmak için gerekli
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;
    public static event Action<int> OnXpChanged;
    [Header("Ekonomi")]
    [SerializeField] private int bankedXP = 500; 
    public int maxBudget = 10;
    public int currentBudgetUpgradeCost = 100; // Bütçeyi 10 -> 11 yapmak için gereken para
    
    public int BankedXp
    {
        get { return bankedXP; }
        set { bankedXP = value; }
    }
    [Header("Veritabanı (Tüm Eşyalar)")]
    // Inspector'dan oyunundaki BÜTÜN ItemData'ları buraya sürüklemelisin!
    public List<ItemData> gameAllItems;

    // Kilidi açılmış ve Mağazaya/Toptancıya düşebilecek eşyalar
    public List<ItemData> unlockedItemsPool = new List<ItemData>();

    // Hangi eşyadan kaç tane var? (Envanter: Eşya -> Adet)
    public Dictionary<ItemData, int> itemStock = new Dictionary<ItemData, int>();

    // Hangi eşya kaçıncı seviyede? (Upgrade: Eşya -> Level)
    public Dictionary<ItemData, int> itemLevels = new Dictionary<ItemData, int>();

    [Header("Aktif Deste")]
    public List<ItemStack> currentDeck = new List<ItemStack>();
    public int maxDeckSize = 8;

    [Header("Perk Sistemi")]
    public List<PerkBase> allPerksPool; // Oyundaki tüm perkler (Mağaza için)
    public List<PerkBase> ownedPerks = new List<PerkBase>(); // Satın alınanlar
    public List<PerkBase> equippedPerks = new List<PerkBase>(); // Takılı olanlar (Max 3)
    public int maxPerkSlots = 3;

    [Header("İlerleme")]
    public int maxLevelReached = 1; // Oyuncunun gördüğü en yüksek seviye

    // Seviye bittiğinde (GameResultManager'dan) bunu çağıracağız
    
    public void UpdateMaxLevel(int level)
    {
        if (level > maxLevelReached)
        {
            maxLevelReached = level;
            
        }
    }

    // --- PERK SATIN ALMA ---
    public bool TryBuyPerk(PerkBase perk)
    {
        // 1. Zaten sahip mi?
        if (ownedPerks.Contains(perk)) return false;

        // 2. Para yetiyor mu?
        if (TrySpendXP(perk.unlockCost))
        {
            ownedPerks.Add(perk);
            Debug.Log($"{perk.perkName} Satın Alındı!");
            return true;
        }

        return false; // Para yetmedi
    }
    public bool ToggleEquipPerk(PerkBase perk)
    {
        // Zaten takılıysa çıkar
        if (equippedPerks.Contains(perk))
        {
            equippedPerks.Remove(perk);
            return false; // Artık takılı değil
        }
        else
        {
            // Yer varsa tak
            if (equippedPerks.Count < maxPerkSlots)
            {
                equippedPerks.Add(perk);
                return true; // Takıldı
            }
        }
        return false; // Yer yok, takılamadı
    }
    void Awake()
    {
        // Singleton Yapısı (Sahneler arası yok olmasın)
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

    void Start()
    {
        // --- BAŞLANGIÇ TEST VERİSİ ---
        // Eğer oyun ilk kez açılıyorsa ve hiç açık eşya yoksa:
        if (unlockedItemsPool.Count == 0 && gameAllItems.Count > 0)
        {
            // Örnek: İlk eşyayı (Muhtemelen Limon) aç ve 10 tane ver
            UnlockItem(gameAllItems[0]);
            AddStock(gameAllItems[0], 10);

            // Varsa ikinci eşyayı da aç (Test için)
            if (gameAllItems.Count > 1)
            {

                UnlockItem(gameAllItems[1]);
                AddStock(gameAllItems[1], 1); 
                UnlockItem(gameAllItems[2]);
                AddStock(gameAllItems[2], 1);
            }
        }
    }

    // ========================================================================
    // --- EKONOMİ YÖNETİMİ ---
    // ========================================================================

    public void AddBankedXP(int amount)
    {
        bankedXP += amount;
        Debug.Log("<color=blue>Veri Güncellendi: </color>" + bankedXP);
        OnXpChanged?.Invoke(bankedXP);
    }

    public bool TrySpendXP(int amount)
    {
        if (bankedXP >= amount)
        {
            bankedXP -= amount;
            Debug.Log("<color=red>Harcama Yapıldı: </color>" + bankedXP);
            OnXpChanged?.Invoke(bankedXP);
            return true; 
        }
        return false; // Yetersiz bakiye
    }

    public void UpgradeBudget()
    {
        maxBudget++;
        // Fiyatı her seferinde %50 artır (Zorlaşsın)
        currentBudgetUpgradeCost = Mathf.RoundToInt(currentBudgetUpgradeCost * 1.5f);
    }

    // ========================================================================
    // --- ENVANTER VE STOK YÖNETİMİ ---
    // ========================================================================

    public void AddStock(ItemData item, int amount)
    {
        if (itemStock.ContainsKey(item))
        {
            itemStock[item] += amount;
        }
        else
        {
            itemStock.Add(item, amount);
        }
    }

    public void RemoveStock(ItemData item, int amount)
    {
        if (itemStock.ContainsKey(item))
        {
            itemStock[item] -= amount;
            if (itemStock[item] < 0) itemStock[item] = 0;
        }
    }

    public int GetStock(ItemData item)
    {
        if (itemStock.ContainsKey(item)) return itemStock[item];
        return 0;
    }

    // Destede kaç tane kullanıldığını hesaplar
    public int GetUsedInDeckCount(ItemData item)
    {
        int count = 0;
        foreach (var stack in currentDeck)
        {
            if (stack.itemData == item) count += stack.amount;
        }
        return count;
    }

    // ========================================================================
    // --- KİLİT AÇMA (UNLOCK) SİSTEMİ ---
    // ========================================================================

    public void UnlockItem(ItemData item)
    {
        // Eğer zaten açık değilse listeye ekle
        if (!unlockedItemsPool.Contains(item))
        {
            unlockedItemsPool.Add(item);

            // İlk açılışta Level 1 olarak kaydet
            if (!itemLevels.ContainsKey(item)) itemLevels.Add(item, 1);

            // İlk açılış hediyesi olarak 1 tane stok verelim mi? (İsteğe bağlı)
            AddStock(item, 1);
        }
    }

    public bool IsItemUnlocked(ItemData item)
    {
        return unlockedItemsPool.Contains(item);
    }

    // ========================================================================
    // --- UPGRADE (LEVEL) VE STAT HESAPLAMA SİSTEMİ ---
    // ========================================================================

    // 1. Eşyanın Levelını Getir
    public int GetItemLevel(ItemData item)
    {
        if (itemLevels.ContainsKey(item)) return itemLevels[item];
        return 1; // Kayıt yoksa Level 1 varsay
    }

    // 2. Güncel Hasarı Hesapla (Base + Upgrade)
    public float GetModifiedDamage(ItemData item)
    {
        int lvl = GetItemLevel(item);
        // Formül: Baz Hasar * (1 + (BüyümeYüzdesi * (Lvl-1)))
        return item.damage * (1f + (item.damageGrowthPercent * (lvl - 1)));
    }

    // 3. Güncel Cooldown Hesapla (Her 5 Levelda bir azalma)
    public float GetModifiedCooldown(ItemData item)
    {
        int lvl = GetItemLevel(item);

        // Kaç tane 5 level devirdik?
        int milestoneCount = lvl / 5;

        // Toplam azalma oranı
        float totalReduction = milestoneCount * item.cdReductionPer5Levels;

        // Güvenlik sınırı (%80'den fazla azalmasın)
        totalReduction = Mathf.Clamp(totalReduction, 0f, 0.8f);

        // Baz süreden düş
        return item.baseCooldown * (1f - totalReduction);
    }

    // 4. Upgrade Maliyetini Hesapla
    public int GetUpgradeCost(ItemData item)
    {
        int lvl = GetItemLevel(item);

        // Formül: BazXP * Level * BütçeMaliyeti
        // (Bütçesi yüksek olan kartı yükseltmek daha pahalı)
        return item.baseXPCost * lvl * item.budgetCost;
    }

    // 5. Upgrade İşlemi (Parayı Kes ve Level Atlat)
    public bool TryUpgradeItem(ItemData item)
    {
        int cost = GetUpgradeCost(item);

        if (TrySpendXP(cost))
        {
            if (itemLevels.ContainsKey(item))
            {
                itemLevels[item]++;
            }
            else
            {
                itemLevels.Add(item, 2);
            }

            Debug.Log($"{item.itemName} Yükseltildi! Yeni Seviye: {itemLevels[item]}");
            return true;
        }

        return false; // Para yetmedi
    }

    // --- DESTE MALİYETİ ---
    public int GetCurrentDeckCost()
    {
        int totalCost = 0;
        foreach (ItemStack stack in currentDeck)
        {
            if (stack.itemData != null)
            {
                totalCost += stack.itemData.budgetCost * stack.amount;
            }
        }
        return totalCost;
    }
}