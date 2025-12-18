using UnityEngine;

[CreateAssetMenu(menuName = "Perks/Passive: Stat Booster")]
public class Perk_EnhEffBooster : PerkBase
{
    public float enhanceIncrease = 0.1f; // Effect Enhance artışı
    [Range(0, 100)] public int chance = 50; // %50 ihtimalle

    public override void OnLevelUp()
    {
        // Şans kontrolü
        if (Random.Range(0, 100) < chance)
        {
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.effectEnhance += enhanceIncrease;
                Debug.Log($"PERK AKTİF: Büyü Gücü arttı! Yeni: {LevelManager.Instance.effectEnhance}");
            }
        }
    }
}