using UnityEngine;
using System.Collections.Generic;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;

    [Header("Veritabanı")]
    public List<ItemData> allUnlockedItems; // Oyuncunun sahip olduğu tüm eşyalar

    [Header("Aktif Deste")]
    // WeaponSystem'deki ItemStack yapısını kullanıyoruz
    public List<ItemStack> currentDeck = new List<ItemStack>();

    public int maxDeckSize = 8; // Desteye kaç eşya koyabiliriz?

    void Awake()
    {
        // Singleton Yapısı (Sahneler arası geçişte yok olmasın)
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
        // Eğer deste boşsa ve eşyamız varsa, ilk 4 taneyi desteye koy
        if (currentDeck.Count == 0 && allUnlockedItems.Count > 0)
        {
            for (int i = 0; i < Mathf.Min(maxDeckSize, allUnlockedItems.Count); i++)
            {
                ItemStack newItem = new ItemStack();
                newItem.itemData = allUnlockedItems[i];
                newItem.amount = 1; // Varsayılan miktar
                currentDeck.Add(newItem);
            }
        }
    }
}