using UnityEngine;

[System.Serializable] // Inspector'da görünmesi için şart!
public class ItemStack
{
    public ItemData itemData; // Hangi eşya? (Limon)
    public int amount = 1;    // Kaç tane? (3)
}