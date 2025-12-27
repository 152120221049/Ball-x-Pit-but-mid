using UnityEngine;

[CreateAssetMenu(menuName = "Perks/Passive: Recurring Item")]
public class Perk_RecurringItem : PerkBase
{
    public ItemData itemToGive;
    public int amount = 1;
    public bool doesEcon = true;
    public override void OnLevelUp()
    {
        int randomBonus = Random.Range(0, 1); // 0, 1, or 2
        WeaponSystem ws = Object.FindFirstObjectByType<WeaponSystem>();
        if (ws != null)
        {
            ws.AddTemporaryItem(itemToGive, amount);
            Debug.Log($"[PERK] TESLİMAT BAŞARILI: {itemToGive.itemName} sisteme eklendi.");
        }
        else
        {
            // HATA BURADA MI? Konsola kırmızı yazar.
            Debug.LogError($"[PERK HATASI] '{perkName}' çalıştı ama WeaponSystem bulunamadı! Player sahnede mi?");
        }
    }
    public override void OnFire(WeaponSystem weaponSystem)
    {
        if (PlayerDataManager.Instance != null&&doesEcon)
        {
            PlayerDataManager.Instance.AddBankedXP(1);
        }
    }
}