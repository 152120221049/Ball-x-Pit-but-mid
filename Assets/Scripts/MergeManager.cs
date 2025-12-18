using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MergeManager : MonoBehaviour
{
    [Header("Veriler")]
    public List<MergeRecipe> allRecipes; // Tüm tarifleri buraya sürükle
    public GameObject mainMenu; // Ana menü referansı
    public GameObject mergeMenu;
    [Header("Tezgah UI")]
    public Image slot1Image;
    public Image slot2Image;
    public Image resultImage;
    public TextMeshProUGUI costText;
    public Button mergeButton;

    [Header("Envanter Listesi UI")]
    public Transform inventoryContainer;
    public GameObject slotPrefab; // DeckSlotUI prefabı

    // Seçilen Eşyalar
    private ItemData item1;
    private ItemData item2;
    private MergeRecipe currentRecipe;

    public void toggleUI(bool isActive)
    {
        if (mainMenu != null) mainMenu.SetActive(!isActive);
        if (mergeMenu != null) mergeMenu.SetActive(isActive);
        if (isActive)
        {
            RefreshInventory();
            ClearSlots();
        }
        // Obje aktif olunca OnEnable otomatik çalışacak
    }

    // --- KRİTİK DÜZELTME: DECK MENU MANTIĞI ---
    void OnEnable()
    {
            RefreshInventory();
            ClearSlots();
    }

    // --- ENVANTERİ LİSTELE ---
    void RefreshInventory()
    {
        // SİGORTA 2: Container atanmış mı?
        if (inventoryContainer == null)
        {
            Debug.LogError("HATA: MergeManager scriptindeki 'Inventory Container' kutusu boş!");
            return;
        }

        foreach (Transform child in inventoryContainer) Destroy(child.gameObject);
        
        // PlayerData var ama liste null mı?
        if (PlayerDataManager.Instance.unlockedItemsPool == null) return;

        foreach (ItemData item in PlayerDataManager.Instance.unlockedItemsPool)
        {
            int count = PlayerDataManager.Instance.GetStock(item);
            if (count > 0)
            {
                // SİGORTA 3: Prefab atanmış mı?
                if (slotPrefab == null)
                {
                    Debug.LogError("HATA: MergeManager 'Slot Prefab' atanmamış!");
                    return;
                }

                GameObject newSlot = Instantiate(slotPrefab, inventoryContainer);
                DeckSlotUI script = newSlot.GetComponent<DeckSlotUI>();

                script.Setup(item, count, -1, null);

                script.button.onClick.RemoveAllListeners();
                script.button.onClick.AddListener(() => OnInventoryItemClicked(item));
            }
        }
    }

    // --- SEÇİM MANTIĞI ---
    void OnInventoryItemClicked(ItemData item)
    {
        // Stok kontrolü (Tezgahta kullanılanları düşme mantığı eklenebilir ama şimdilik basit tutalım)

        // Boş slota yerleştir
        if (item1 == null)
        {
            item1 = item;
            slot1Image.sprite = item.itemIcon;
            slot1Image.enabled = true;
        }
        else if (item2 == null)
        {
            item2 = item;
            slot2Image.sprite = item.itemIcon;
            slot2Image.enabled = true;
        }
        else
        {
            // İkisi de doluysa Slot 1'i değiştir (Döngü)
            item1 = item2;
            slot1Image.sprite = item1.itemIcon;
            item2 = item;
            slot2Image.sprite = item2.itemIcon;
        }

        CheckRecipe();
    }

    // --- TARİF KONTROLÜ ---
    void CheckRecipe()
    {
        currentRecipe = null;
        resultImage.enabled = false;
        costText.text = "Tarif Yok";
        mergeButton.interactable = false;

        if (item1 == null || item2 == null) return;

        // Tarifleri tara
        foreach (var recipe in allRecipes)
        {
            // A+B veya B+A kombinasyonu
            bool match = (recipe.inputA == item1 && recipe.inputB == item2) ||
                         (recipe.inputA == item2 && recipe.inputB == item1);

            if (match)
            {
                currentRecipe = recipe;
                resultImage.sprite = recipe.resultItem.itemIcon;
                resultImage.enabled = true;
                costText.text = $"{recipe.cost} XP";

                // Para yetiyor mu?
                mergeButton.interactable = PlayerDataManager.Instance.BankedXp >= recipe.cost;
                return;
            }
        }
    }

    // --- BİRLEŞTİRME ---
    public void TryMerge()
    {
        // 1. KONTROL: Tarif Var mı?
        if (currentRecipe == null)
        {
            Debug.LogError("HATA: Geçerli bir tarif seçili değil! (currentRecipe == null)");
            // Görsel uyarı ver (Butonu salla)
            if (mergeButton) mergeButton.transform.DOShakePosition(0.5f, 10f);
            return;
        }

        // 2. KONTROL: Para Yetiyor mu?
        // Maliyeti konsola yazdır
        Debug.Log($"Birleştirme deneniyor... Maliyet: {currentRecipe.cost} | Cüzdan: {PlayerDataManager.Instance.BankedXp}");

        if (PlayerDataManager.Instance.TrySpendXP(currentRecipe.cost))
        {
            // 3. İŞLEM BAŞARILI

            // Stoktan düş
            PlayerDataManager.Instance.RemoveStock(item1, 1);
            PlayerDataManager.Instance.RemoveStock(item2, 1);

            // Yeni eşyayı ver
            PlayerDataManager.Instance.UnlockItem(currentRecipe.resultItem);
            PlayerDataManager.Instance.AddStock(currentRecipe.resultItem, 1);

            Debug.Log($"BAŞARILI! Yeni Eşya: {currentRecipe.resultItem.itemName}");

            // Efekt
            if (resultImage != null) resultImage.transform.DOPunchScale(Vector3.one * 0.5f, 0.3f);

            // Temizlik
            ClearSlots();
            RefreshInventory();
        }
        else
        {
            // 4. HATA: Para Yok
            Debug.LogError("HATA: Yetersiz Bakiye!");
            if (costText) costText.transform.DOShakePosition(0.5f, 10f);
        }
    }

    public void ClearSlots()
    {
        item1 = null;
        item2 = null;
        slot1Image.enabled = false;
        slot2Image.enabled = false;
        resultImage.enabled = false;
        costText.text = "Malzeme Seç";
        mergeButton.interactable = false;
    }
}