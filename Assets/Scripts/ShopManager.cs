using DG.Tweening;
using System.Collections.Generic;
using System.Linq; // Random seçim için gerekli
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("AnaMenu Referansı")]
    public GameObject mainMenu;
    [Header("UI Konteynerleri")]
    public Transform unlockShopContainer; // Üst Kırmızı Alan (Grid)
    public Transform wholesalerContainer; // Alt Yeşil Alan (Grid)

    [Header("Prefablar")]
    public GameObject shopSlotPrefab; // Standart slot

    [Header("Genel UI")]
    public TextMeshProUGUI playerMoneyText;

    void Start()
    {
        RefreshAllShops();
    }
    public void TogglePanels(bool isActive)
    {
        if(mainMenu != null)
        {
            isActive = mainMenu.activeSelf;
            mainMenu.SetActive(!isActive);
            gameObject.SetActive(isActive);
            if (!isActive)
            {
                RefreshAllShops();
            }
        }
    }
    public void RefreshAllShops()
    {
        UpdateMoneyUI();
        GenerateUnlockShop(); 
        GenerateWholesaler(); 
    }

    void UpdateMoneyUI()
    {
        if (PlayerDataManager.Instance != null)
            playerMoneyText.text = $"{PlayerDataManager.Instance.BankedXp}";
    }

    // --- 1. MAĞAZA (UNLOCK) SİSTEMİ ---
    void GenerateUnlockShop()
    {
        foreach (Transform child in unlockShopContainer) Destroy(child.gameObject);

        // Veritabanındaki HER eşyaya bak
        foreach (ItemData item in PlayerDataManager.Instance.gameAllItems)
        {
            // Eğer zaten açıksa mağazada gösterme! (Veya "Satıldı" diye göster)
            if (PlayerDataManager.Instance.IsItemUnlocked(item)) continue;

            // Slotu oluştur
            GameObject slot = Instantiate(shopSlotPrefab, unlockShopContainer);
            SetupSlotVisuals(slot, item, item.unlockCost, "AÇ");

            // Butonu Bağla
            Button btn = slot.GetComponent<Button>(); // Veya içindeki buton
            btn.onClick.AddListener(() => BuyUnlock(item, slot));
        }
    }

    void BuyUnlock(ItemData item, GameObject slotObj)
    {
        if (PlayerDataManager.Instance.TrySpendXP(item.unlockCost))
        {
            // Satın alındı!
            PlayerDataManager.Instance.UnlockItem(item);

            // Efekt ve Yenileme
            slotObj.transform.DOPunchScale(Vector3.one * 0.2f, 0.2f).OnComplete(() => RefreshAllShops());
            PlayerDataManager.Instance.SaveGame();
        }
        else
        {
            CantAffordEffect();
        }
    }

    // --- 2. TOPTANCI (WHOLESALER) SİSTEMİ ---
    void GenerateWholesaler()
    {
        foreach (Transform child in wholesalerContainer) Destroy(child.gameObject);

        List<ItemData> pool = PlayerDataManager.Instance.unlockedItemsPool;
        if (pool.Count == 0) return; // Hiç açık eşya yoksa boş kalsın

        // Rastgele 6 tane seç (Shuffle)
        List<ItemData> dailyOffer = pool.OrderBy(x => Random.value).Take(6).ToList();

        foreach (ItemData item in dailyOffer)
        {
            // DİNAMİK FİYATLANDIRMA
            // Formül: Baz Fiyat + (Stok Sayısı * Enflasyon)
            int currentStock = PlayerDataManager.Instance.GetStock(item);
            int dynamicPrice = item.wholesaleBaseCost + (currentStock * 5); // Örn: Her stok başı 5 XP artar

            // Slotu oluştur
            GameObject slot = Instantiate(shopSlotPrefab, wholesalerContainer);

            // Görseli ayarla (Stok bilgisini de gösterelim)
            SetupSlotVisuals(slot, item, dynamicPrice, $"+5 Stok\n(Elde: {currentStock})");

            // Butonu Bağla
            Button btn = slot.GetComponent<Button>();
            btn.onClick.AddListener(() => BuyWholesaleStock(item, 5, dynamicPrice, slot));
        }
    }

    void BuyWholesaleStock(ItemData item, int amount, int price, GameObject slotObj)
    {
        if (PlayerDataManager.Instance.TrySpendXP(price))
        {
            PlayerDataManager.Instance.AddStock(item, amount);

            // Efekt ve Yenileme (Fiyat artacağı için yenilemek lazım)
            slotObj.transform.DOPunchScale(Vector3.one * 0.2f, 0.2f).OnComplete(() => RefreshAllShops());
            PlayerDataManager.Instance.SaveGame();
        }
        else
        {
            CantAffordEffect();
        }
    }

    // --- YARDIMCI FONKSİYONLAR ---
    void SetupSlotVisuals(GameObject slot, ItemData item, int price, string extraInfo)
    {
        // SlotPrefab'ının içindeki Image ve Textlere ulaşma (İsimlendirmelerine dikkat et)
        // Örnek yapı: Slot -> Icon (Image), NameText (TMP), PriceText (TMP)

        Transform iconTr = slot.transform.Find("Icon"); // Obje adın neyse onu yaz
        if (iconTr) iconTr.GetComponent<Image>().sprite = item.itemIcon;

        Transform nameTr = slot.transform.Find("NameText");
        if (nameTr) nameTr.GetComponent<TextMeshProUGUI>().text = item.itemName;

        Transform priceTr = slot.transform.Find("PriceText");
        if (priceTr) priceTr.GetComponent<TextMeshProUGUI>().text = $"{price} XP\n{extraInfo}";
    }

    void CantAffordEffect()
    {
        playerMoneyText.transform.DOShakePosition(0.5f, 10f);
        playerMoneyText.color = Color.red;
        playerMoneyText.DOColor(Color.white, 0.5f);
    }
}