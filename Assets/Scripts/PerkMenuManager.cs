using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class PerkMenuManager : MonoBehaviour
{
    [Header("Market Kısmı (Aşağısı)")]
    public GameObject marketSlotPrefab; // Eski butonlu prefab
    public Transform marketContainer;   // Grid Layout (Tüm perkler)

    [Header("Kuşanılanlar Kısmı (Yukarısı)")]
    public GameObject equippedSlotPrefab; // YENİ: Butonsuz prefab
    public Transform equippedContainer;   // YENİ: Horizontal Layout (Sadece seçilenler)

    void OnEnable()
    {
        if (PlayerDataManager.Instance != null) RefreshUI();
        else Invoke(nameof(OnEnable), 0.1f);
    }

    // Her şeyi yenileyen ana fonksiyon
    void RefreshUI()
    {
        RefreshMarket();
        RefreshEquipped();
    }

    // 1. AŞAĞIYI YENİLE (Eski Kodun Benzeri)
    void RefreshMarket()
    {
        foreach (Transform child in marketContainer) Destroy(child.gameObject);

        foreach (PerkBase perk in PlayerDataManager.Instance.allPerksPool)
        {
            GameObject newSlot = Instantiate(marketSlotPrefab, marketContainer);
            newSlot.transform.localScale = Vector3.one;
            newSlot.GetComponent<PerkUI>().Setup(perk, this);
        }
    }

    // 2. YUKARIYI YENİLE (Yeni Özellik)
    void RefreshEquipped()
    {
        // Önce temizle
        foreach (Transform child in equippedContainer) Destroy(child.gameObject);

        // Sadece takılı olanları (equippedPerks) dön
        foreach (PerkBase perk in PlayerDataManager.Instance.equippedPerks)
        {
            GameObject newSlot = Instantiate(equippedSlotPrefab, equippedContainer);
            newSlot.transform.localScale = Vector3.one;

            // Yeni scripti çağır
            newSlot.GetComponent<EquippedPerkUI>().Setup(perk, this);

            // Ufak bir animasyonla gelsin
            newSlot.transform.DOScale(0f, 0f);
            newSlot.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        }
    }

    // Marketten Tıklama (Eski Fonksiyon)
    public void OnPerkClicked(PerkUI slot)
    {
        PerkBase perk = slot.myPerk; // SlotUI'daki myPerk public olmalı!

        switch (slot.currentState)
        {
            case PerkUI.SlotState.ForSale:
                if (PlayerDataManager.Instance.TryBuyPerk(perk)) RefreshUI();
                break;

            case PerkUI.SlotState.Owned:
            case PerkUI.SlotState.Equipped:
                PlayerDataManager.Instance.ToggleEquipPerk(perk);
                RefreshUI(); // Hem aşağısı hem yukarısı güncellenir
                break;
        }
    }

    // Yukarından Tıklama (Direkt Çıkarma)
    public void UnequipPerkDirectly(PerkBase perk)
    {
        // Direkt olarak listeden çıkar
        if (PlayerDataManager.Instance.equippedPerks.Contains(perk))
        {
            PlayerDataManager.Instance.equippedPerks.Remove(perk);
            RefreshUI(); // Listeleri güncelle
        }
    }
}