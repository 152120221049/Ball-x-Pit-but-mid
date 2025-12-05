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
    public float budgetCost = 1f;
    public float destroyTimer = 3f;
    public float speed = 10f;
}
