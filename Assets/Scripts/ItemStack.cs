using UnityEngine;

[System.Serializable] // Inspector'da görünmesi için şart!
public class ItemStack
{
    public ItemData itemData;
    public int amount;

    // Boş Constructor (Unity için gerekli)
    public ItemStack() { }

    // --- YENİ: KOPYALAMA CONSTRUCTOR'I ---
    // Bu fonksiyon, var olan bir paketi alıp aynısından yeni bir tane üretir
    public ItemStack(ItemStack original)
    {
        this.itemData = original.itemData;
        this.amount = original.amount;
    }
}