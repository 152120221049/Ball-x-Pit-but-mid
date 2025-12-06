using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class UpgradeManager : MonoBehaviour
{
    [Header("Panel Referansları")]
    public GameObject sanayiPanel;
    public GameObject mainMenu;

    [Header("Bütçe UI")]
    public TextMeshProUGUI budgetInfoText;
    public TextMeshProUGUI bankInfoText;
    public TextMeshProUGUI budgetCostText;
    public TextMeshProUGUI bankInfoText2;

    [Header("Eşya Upgrade UI")]
    public Transform itemsListContent; // Scroll View Content'i
    public GameObject itemSlotPrefab;  // DeckSlotUI prefabını kullanabiliriz

    [Header("Detay Paneli")]
    public GameObject detailsPanel; // Seçim yapılınca açılacak kısım
    public Image detailIcon;
    public TextMeshProUGUI detailNameText;
    public TextMeshProUGUI detailStatsText;
    public Button itemUpgradeButton;
    public TextMeshProUGUI itemUpgradeButtonText;

    private ItemData selectedItem; // Şu an hangisine bakıyoruz?

    void Start()
    {
        UpdateBudgetUI();
    }

    public void TogglePanel(bool isOpen)
    {
        mainMenu.SetActive(!isOpen);
        sanayiPanel.SetActive(isOpen);
        if (isOpen)
        {
            UpdateBudgetUI();
            GenerateItemList(); // Listeyi tazele
            detailsPanel.SetActive(false); // Başlangıçta detay kapalı
        }
    }

    // --- LİSTELEME SİSTEMİ ---
    void GenerateItemList()
    {
        // Önce temizle
        foreach (Transform child in itemsListContent) Destroy(child.gameObject);

        // Tüm eşyaları diz
        foreach (ItemData item in PlayerDataManager.Instance.allUnlockedItems)
        {
            GameObject slot = Instantiate(itemSlotPrefab, itemsListContent);

            // DeckSlotUI scriptini kullanarak görseli ayarla
            // Not: Miktar önemli değil, sadece resmi görünsün
            var script = slot.GetComponent<DeckSlotUI>();

            // DeckMenuManager referansı yerine 'null' veriyoruz, tıklamayı biz elle yöneteceğiz
            script.Setup(item, 1, -1, null);

            // Tıklama olayını override et (Ez)
            script.button.onClick.RemoveAllListeners();
            script.button.onClick.AddListener(() => OnItemSelected(item));
        }
    }

    // --- SEÇİM SİSTEMİ ---
    void OnItemSelected(ItemData item)
    {
        selectedItem = item;
        detailsPanel.SetActive(true);
        UpdateDetailsUI();
    }

    void UpdateDetailsUI()
    {
        if (selectedItem == null) return;

        int currentLvl = PlayerDataManager.Instance.GetItemLevel(selectedItem);
        float currentDmg = PlayerDataManager.Instance.GetModifiedDamage(selectedItem);
        float currentCD = PlayerDataManager.Instance.GetModifiedCooldown(selectedItem);

        // Gelecek seviyenin hesapları (Önizleme için)
        // Geçici bir hesaplama yapıyoruz:
        float nextDmg = selectedItem.damage * (1f + (selectedItem.damageGrowthPercent * currentLvl)); // lvl yerine lvl+1 mantığı (lvl zaten 1 tabanlı)

        int cost = PlayerDataManager.Instance.GetUpgradeCost(selectedItem);
        int playerMoney = PlayerDataManager.Instance.SariKulaReserves;

        // Görselleri Bas
        detailIcon.sprite = selectedItem.itemIcon;
        detailNameText.text = $"{selectedItem.itemName} (Lvl {currentLvl})";

        string stats = $"Hasar: {currentDmg:F1} -> <color=green>{nextDmg:F1}</color>\n";
        stats += $"CD: {currentCD:F2}s";

        // 5 Levelda bir bonus bilgisi
        if ((currentLvl + 1) % 5 == 0) stats += "\n<color=yellow>SONRAKİ LEVELDA CD AZALACAK!</color>";

        detailStatsText.text = stats;

        // Buton Ayarı
        itemUpgradeButtonText.text = $"YÜKSELT\n{cost} XP";
        itemUpgradeButton.interactable = (playerMoney >= cost);

        // Butonu Bağla
        itemUpgradeButton.onClick.RemoveAllListeners();
        itemUpgradeButton.onClick.AddListener(TryUpgradeItem);
    }

    // --- UPGRADE İŞLEMİ ---
    public void TryUpgradeItem()
    {
        if (selectedItem == null) return;

        if (PlayerDataManager.Instance.TryUpgradeItem(selectedItem))
        {
            // Efektler
            detailIcon.transform.DOPunchScale(Vector3.one * 0.3f, 0.3f);

            UpdateBudgetUI(); // Para azaldı, güncelle
            UpdateDetailsUI(); // Level arttı, güncelle
        }
        else
        {
            // Para yok efekti
            bankInfoText.transform.DOShakePosition(0.5f, 10f);
        }
    }
    public void TryBuyBudget()
    {
        int cost = PlayerDataManager.Instance.geliştirmeMasrafi;

        // Parayı çekmeye çalış
        if (PlayerDataManager.Instance.TrySpendXP(cost))
        {
            // Başarılıysa Bütçeyi Artır
            PlayerDataManager.Instance.UpgradeBudget();

            // Efekt (Juice)
            budgetInfoText.transform.DOPunchScale(Vector3.one * 0.5f, 0.3f);

            UpdateBudgetUI();
        }
        else
        {
            // Yetersiz Bakiye
            Debug.Log("Para Yetmiyor!");
            bankInfoText.transform.DOShakePosition(0.5f, 10f); // Parayı salla
            bankInfoText.color = Color.red;
            bankInfoText.DOColor(Color.white, 0.5f);
        }
    }

    void UpdateBudgetUI()
    {
        if (PlayerDataManager.Instance == null) return;

        budgetInfoText.text = $"Mevcut bütçe = {PlayerDataManager.Instance.maxBudget}";
        bankInfoText.text = $"= {PlayerDataManager.Instance.SariKulaReserves}";
        budgetCostText.text = $"= {PlayerDataManager.Instance.geliştirmeMasrafi} ";
        bankInfoText2.text = $"= {PlayerDataManager.Instance.SariKulaReserves}";
    }
}
