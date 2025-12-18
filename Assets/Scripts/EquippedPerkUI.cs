using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquippedPerkUI : MonoBehaviour
{
    [Header("Görsel Bileşenler")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;
    public Button removeButton; // Görünmez buton (Tüm karta tıklamak için)

    private PerkBase myPerk;
    private PerkMenuManager manager;

    public void Setup(PerkBase perk, PerkMenuManager mgr)
    {
        myPerk = perk;
        manager = mgr;

        // Verileri Yaz
        iconImage.sprite = perk.icon;
        nameText.text = perk.perkName;
        descText.text = perk.description;

        // Tıklayınca ÇIKAR (Unequip)
        removeButton.onClick.RemoveAllListeners();
        removeButton.onClick.AddListener(() =>
        {
            // Manager üzerinden çıkarma işlemini tetikle
            manager.UnequipPerkDirectly(myPerk);
        });
    }
}