using UnityEngine;
using TMPro;

public class XpDisplay : MonoBehaviour
{
    private TextMeshProUGUI xpText;

    void Awake()
    {
        xpText = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        // Başlangıçta veriyi hemen çek
        RefreshManual();
    }

    private void OnEnable()
    {
        // Önce temizle (Çift aboneliği önlemek için)
        PlayerDataManager.OnXpChanged -= UpdateXpUI;
        // Sonra abone ol
        PlayerDataManager.OnXpChanged += UpdateXpUI;

        RefreshManual();
    }

    private void OnDisable()
    {
        PlayerDataManager.OnXpChanged -= UpdateXpUI;
    }

    // Harici veya manuel yenileme için
    public void RefreshManual()
    {
        if (PlayerDataManager.Instance != null)
        {
            UpdateXpUI(PlayerDataManager.Instance.BankedXp);
        }
    }

    private void UpdateXpUI(int amount)
    {
        if (xpText != null)
        {
            xpText.text = amount.ToString();
            // BURASI ÇIKMIYORSA ABONELİK KOPUKTUR
            Debug.Log("<color=green>UI GÜNCELLENDİ: </color>" + amount);
        }
    }
}