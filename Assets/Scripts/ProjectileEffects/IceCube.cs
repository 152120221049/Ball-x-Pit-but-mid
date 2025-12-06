using UnityEngine;

public class IceCube : ProjectileEffects
{
    [Header("Görsel")]
    public GameObject kirilmaEfekti; 
    public ItemData IceData;
    public int freezeAmount = 1;
    float enhanceMultiplier = (LevelManager.Instance != null) ? LevelManager.Instance.effectEnhance : 1f;
    protected override void OnHitEnemy(EnemyBase enemy)
    {
        ExecuteFreeze(enemy, (int)(freezeAmount * enhanceMultiplier));

        if (kirilmaEfekti != null)
        {
            GameObject vfx = Instantiate(kirilmaEfekti, transform.position, Quaternion.identity);
            Destroy(vfx, 1f); 
        }
        
    }
}