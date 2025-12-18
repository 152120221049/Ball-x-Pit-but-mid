using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PerkUI : MonoBehaviour
{
    [Header("Görsel Bileşenler")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI statusText; // "Açık", "500 XP", "Lvl 5" yazacak
    public Button button;
    public Image buttonImage; // Butonun rengini değiştirmek için

    [HideInInspector]public PerkBase myPerk;
    private PerkMenuManager manager;

    // Durumlar
    public enum SlotState { Locked, ForSale, Owned, Equipped }
    public SlotState currentState;

    public void Setup(PerkBase perk, PerkMenuManager mgr)
    {
        myPerk = perk;
        manager = mgr;

        // Görselleri Ata
        iconImage.sprite = perk.icon;
        nameText.text = perk.perkName;

        // --- DURUM BELİRLEME ---
        DetermineState();
        UpdateVisuals();

        // Tıklama
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => manager.OnPerkClicked(this));
    }

    void DetermineState()
    {
        // 1. Zaten takılı mı?
        if (PlayerDataManager.Instance.equippedPerks.Contains(myPerk))
        {
            currentState = SlotState.Equipped;
        }
        // 2. Satın alınmış mı?
        else if (PlayerDataManager.Instance.ownedPerks.Contains(myPerk))
        {
            currentState = SlotState.Owned;
        }
        // 3. Seviye yetiyor mu?
        else if (PlayerDataManager.Instance.maxLevelReached >= myPerk.requiredLevel)
        {
            currentState = SlotState.ForSale;
        }
        // 4. Seviye yetmiyorsa kilitli
        else
        {
            currentState = SlotState.Locked;
        }
    }

    void UpdateVisuals()
    {
       
        button.interactable = true;

        switch (currentState)
        {
            case SlotState.Equipped:
                statusText.text = "ÇIKAR";
                buttonImage.color = Color.green; // Takılıysa Yeşil
                
                break;

            case SlotState.Owned:
                statusText.text = "KUŞAN";
                buttonImage.color = Color.white; // Sahipse Beyaz
                break;

            case SlotState.ForSale:
                statusText.text = $"{myPerk.unlockCost} XP";
                buttonImage.color = Color.yellow; // Satılıksa Sarı
                break;

            case SlotState.Locked:
                statusText.text = $"Lvl {myPerk.requiredLevel}";
                buttonImage.color = Color.gray; // Kilitliyse Gri
                button.interactable = false; // Tıklanamaz
                iconImage.color = Color.black; // İkonu karart
                break;
        }

        // Kilitli değilse ikonun rengini aç
        if (currentState != SlotState.Locked) iconImage.color = Color.white;
    }
}