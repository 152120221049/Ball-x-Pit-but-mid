using UnityEngine;
using UnityEngine.SceneManagement; // Sahne geçişi için
using System.Collections.Generic;
using TMPro;
using DG.Tweening; // Hata animasyonu için
using UnityEngine.UI;
public class DeckMenuManager : MonoBehaviour
{
    [Header("Referanslar")]
    public GameObject slotPrefab;
    public Transform deckContainer;
    public Transform collectionContainer;
    public TextMeshProUGUI budgetText;
    public GameObject MainMenu;
    public GameObject deckMenu;
    private DeckSlotUI selectedCollectionSlot;
    [Header("Sabit Butonlar")]
    public Button backButton; // Geri Dön butonu referansı

    void Start()
    {
        // 1. BUTON BAĞLANTISINI KODLA YAP
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners(); // Temizle
            backButton.onClick.AddListener(TryToExit); // Fonksiyonu bağla
        }

        // 2. ARAYÜZÜ YENİLE
        RefreshUI();
    }
    void OnEnable()
    {
        // PlayerDataManager hazır mı diye kontrol et
        if (PlayerDataManager.Instance != null)
        {
            RefreshUI();
        }
        else
        {
          
            Invoke(nameof(RefreshUI), 0.1f);
        }
    }

    public void RefreshUI()
    {
        // 1. DESTE SLOTLARI
        foreach (Transform child in deckContainer) Destroy(child.gameObject);
        List<ItemStack> currentDeck = PlayerDataManager.Instance.currentDeck;

        for (int i = 0; i < PlayerDataManager.Instance.maxDeckSize; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, deckContainer);
            DeckSlotUI script = newSlot.GetComponent<DeckSlotUI>();

            // Veriyi çek
            ItemStack stack = (i < currentDeck.Count) ? currentDeck[i] : null;
            ItemData data = (stack != null) ? stack.itemData : null;
            int amount = (stack != null) ? stack.amount : 0;

            // Setup'a miktarı da gönderiyoruz
            script.Setup(data, amount, i, this);
        }

        // 2. KOLEKSİYON SLOTLARI
        // 2. KOLEKSİYON (ENVANTER) ÇİZİMİ
        foreach (Transform child in collectionContainer) Destroy(child.gameObject);

        // Sadece kilidi açılmış (unlocked) eşyaları değil, envanterde olanları göster
        // Veya unlocked olup adedi 0 olanları "gri" gösterebilirsin.
        foreach (ItemData item in PlayerDataManager.Instance.unlockedItemsPool)
        {
            GameObject newSlot = Instantiate(slotPrefab, collectionContainer);
            DeckSlotUI script = newSlot.GetComponent<DeckSlotUI>();

            // HESAPLAMA:
            int totalOwned = PlayerDataManager.Instance.GetStock(item);
            int usedInDeck = PlayerDataManager.Instance.GetUsedInDeckCount(item);
            int available = totalOwned - usedInDeck;

            // Slot'a "Kullanılabilir / Toplam" bilgisini gönder
            script.Setup(item, available, -1, this);

            // Eğer elimizde kalmadıysa (Available 0 ise) butonu pasif yap veya grileştir
            if (available <= 0)
            {
                script.iconImage.color = Color.gray; // Sönük olsun
                // script.button.interactable = false; // Tıklanmasın istersen
            }
        }

        UpdateBudgetDisplay();
    }

    // --- YENİ TIKLAMA MANTIĞI ---
    public void OnSlotClicked(DeckSlotUI clickedSlot)
    {
        var deck = PlayerDataManager.Instance.currentDeck;

        // DURUM A: AŞAĞIDAN (Koleksiyondan) TIKLAMA
        if (clickedSlot.myDeckIndex == -1)
        {
            // 1. SEÇİM İPTALİ (Toggle Logic)
            // Eğer zaten seçili olan karta tekrar tıkladıysak -> Seçimi Kaldır
            if (selectedCollectionSlot == clickedSlot)
            {
                selectedCollectionSlot.SetSelected(false);
                selectedCollectionSlot = null;
                return; // İşlem bitti
            }

            // 2. YENİ SEÇİM
            if (selectedCollectionSlot != null) selectedCollectionSlot.SetSelected(false);
            selectedCollectionSlot = clickedSlot;
            selectedCollectionSlot.SetSelected(true);
        }

        // DURUM B: YUKARIDAN (Desteden) TIKLAMA
        else
        {
            // Eğer aşağıdan bir eşya SEÇİLİYSE -> EKLE veya ARTTIR
            if (selectedCollectionSlot != null)
            {
                HandleDeckModification(clickedSlot.myDeckIndex, selectedCollectionSlot.myItem);
            }
            // Eğer aşağıdan bir şey seçili DEĞİLSE -> ÇIKART (SİL)
            else
            {
                if (clickedSlot.myDeckIndex < deck.Count)
                {
                    deck.RemoveAt(clickedSlot.myDeckIndex);
                    RefreshUI();
                }
            }
        }
    }

    // Ekleme / Arttırma / Değiştirme Mantığı
    void HandleDeckModification(int slotIndex, ItemData newItem)
    {
        var deck = PlayerDataManager.Instance.currentDeck;
        int currentTotalCost = PlayerDataManager.Instance.GetCurrentDeckCost();
        int maxBudget = PlayerDataManager.Instance.maxBudget;

        // Slot boş mu dolu mu?
        bool isSlotEmpty = (slotIndex >= deck.Count);

        // Eğer slot doluysa, içindeki ne?
        ItemStack existingStack = isSlotEmpty ? null : deck[slotIndex];
        // KONTROL: Elimizde bu eşyadan kaldı mı?
        int totalOwned = PlayerDataManager.Instance.GetStock(newItem);
        int usedInDeck = PlayerDataManager.Instance.GetUsedInDeckCount(newItem);

        if (usedInDeck >= totalOwned)
        {
            Debug.LogWarning("Stokta kalmadı! Toptancıya git.");
            // Bir uyarı popup'ı açabilirsin
            return;
        }
        // SENARYO 1: Slot Boş -> Yeni Ekle (1 Adet)
        if (isSlotEmpty)
        {
            // Bütçe Kontrolü (1 tane için)
            if (currentTotalCost + newItem.budgetCost <= maxBudget)
            {
                ItemStack newStack = new ItemStack { itemData = newItem, amount = 1 };
                deck.Add(newStack);
                RefreshUI();
            }
            else
            {
                ShowBudgetWarning();
            }
        }
        // SENARYO 2: Slot Dolu ve AYNI Eşya -> Miktarı Arttır (+1)
        else if (existingStack.itemData == newItem)
        {
            // Bütçe Kontrolü (+1 tane için)
            if (currentTotalCost + newItem.budgetCost <= maxBudget)
            {
                existingStack.amount++; // Miktarı arttır
                RefreshUI();
            }
            else
            {
                ShowBudgetWarning();
            }
        }
        // SENARYO 3: Slot Dolu ama FARKLI Eşya -> Değiştir (Swap)
        else
        {
            // Eski eşyanın toplam maliyetini çıkar, yenisinin 1 tanesini ekle
            int oldCost = existingStack.itemData.budgetCost * existingStack.amount;
            int newCost = newItem.budgetCost * 1; // Yeni eşya 1 tane ile başlar

            if (currentTotalCost - oldCost + newCost <= maxBudget)
            {
                existingStack.itemData = newItem;
                existingStack.amount = 1; // Miktarı 1'e sıfırla
                RefreshUI();
            }
            else
            {
                ShowBudgetWarning();
            }
        }
    }

    void ShowBudgetWarning()
    {
        Debug.LogWarning("Bütçe Yetersiz!");
        budgetText.transform.DOShakePosition(0.5f, 10f);
    }

    void UpdateBudgetDisplay()
    {
        if (budgetText != null)
        {
            int currentCost = PlayerDataManager.Instance.GetCurrentDeckCost();
            int maxCost = PlayerDataManager.Instance.maxBudget;
            budgetText.text = $"Bütçe: {currentCost} / {maxCost}";
            budgetText.color = (currentCost > maxCost) ? Color.red : Color.white;
        }
    }


    public void TryToExit()
    {
        var deck = PlayerDataManager.Instance.currentDeck;
        int currentCost = PlayerDataManager.Instance.GetCurrentDeckCost();
        int maxCost = PlayerDataManager.Instance.maxBudget;

        // KURAL 1: Deste Boş Olamaz
        if (deck.Count == 0)
        {
            Debug.LogWarning("Deste Boş! Savaşa gidemezsin.");
            deckContainer.DOShakePosition(0.5f, 10f); // Desteyi salla
            return;
        }

        // KURAL 2: Bütçe Aşılmamalı (Ekstra güvenlik)
        if (currentCost > maxCost)
        {
            Debug.LogWarning("Bütçe Aşılmış!");
            budgetText.transform.DOShakePosition(0.5f, 10f);
            return;
        }
        deckMenu.SetActive(false);
        MainMenu.SetActive(true);
        
    }
}