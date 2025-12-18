using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckSlotUI : MonoBehaviour
{
    [Header("Görsel Bileşenler")]
    public Image iconImage;
    public Button button;
    public GameObject selectedBorder;

    public TextMeshProUGUI infoText;
    

    [HideInInspector] public ItemData myItem;
    [HideInInspector] public int myDeckIndex = -1;

    private DeckMenuManager menuManager;

 
    public void Setup(ItemData item, int amount, int deckIndex, DeckMenuManager manager)
    {
        myItem = item;
        myDeckIndex = deckIndex;
        menuManager = manager;

        if (item != null)
        {
            iconImage.sprite = item.itemIcon;
            iconImage.enabled = true;

          
            if (deckIndex != -1)
                infoText.text = $"{item.itemName} x{amount}";
            else
                infoText.text = item.itemName;
           
        }
        else
        {
            iconImage.enabled = false;
            iconImage.sprite = null;

            if (infoText != null)
            {
                // "Boş" yazmasın, boşluk olsun. Daha temiz durur.
                infoText.text = "";
            }
        }

        if (selectedBorder != null) selectedBorder.SetActive(false);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        if (menuManager != null) menuManager.OnSlotClicked(this);
    }

    public void SetSelected(bool isSelected)
    {
        if (selectedBorder != null) selectedBorder.SetActive(isSelected);
    }
}