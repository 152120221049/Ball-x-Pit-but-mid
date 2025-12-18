using UnityEngine;

public abstract class PerkBase : ScriptableObject
{
    [Header("Görsel Bilgiler")]
    public string perkName;
    [TextArea] public string description;
    public Sprite icon;
    public int unlockCost;
    public int requiredLevel = 1;
    public virtual void OnGameStart() { }

    
    public virtual void OnLevelUp() { }

    public virtual void OnEnemyDefeated(GameObject enemy) { }
    public virtual void OnFire(WeaponSystem weaponSystem) { }
    public virtual void OnDeath() { }
    public virtual float ModifyDamage(float currentDamage, float distanceToHome) => currentDamage;
    public virtual float ModifyCooldown(float currentCD, float distanceToHome) => currentCD;
}