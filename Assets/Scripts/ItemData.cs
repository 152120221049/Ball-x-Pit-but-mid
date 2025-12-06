using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    [Header("Visuals")]
    public GameObject projectilePrefab; 
    public Sprite itemIcon;
   
    [Header("Stats")]
    public float damage = 3f;
    public float baseCooldown = 0.1f; 
    public int   budgetCost = 1;
    public float destroyTimer = 3f;
    public float speed = 10f;
    [Header("Upgrade Kuralları")]
    [Tooltip("Her seviyede hasar % kaç artsın? (Örn: 0.1 = %10)")]
    public float damageGrowthPercent = 0.1f;
    [Tooltip("Her 5 seviyede CD % kaç azalsın? (Örn: 0.05 = %5)")]
    public float cdReductionPer5Levels = 0.05f;
    [Tooltip("Level 1 -> 2 için gereken Baz XP")]
    public int baseXPCost = 50;
}
