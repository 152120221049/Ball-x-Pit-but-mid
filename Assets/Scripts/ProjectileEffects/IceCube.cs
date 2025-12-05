using UnityEngine;

public class IceCube : ProjectileEffects
{
    [Header("Görsel")]
    public GameObject kirilmaEfekti; // Buz kırılma efekti (Particle System)
    public ItemData IceData;
    public int freezeAmount = 1; // Uygulanacak donma miktarı
    // Not: Start() fonksiyonuna gerek yok, Base sınıftaki Initialize hızı ayarlıyor.

    protected override void OnHitEnemy(EnemyBase enemy)
    {
        // 1. DİNAMİK HASAR (Base sınıftan gelen currentDamage)
        enemy.TakeDamage(IceData.damage);

        // 2. DONDURMA (Base sınıftaki fonksiyon)
        ExecuteFreeze(enemy,freezeAmount);

        // 3. EFEKT (Varsa kırılma efekti çıkar)
        if (kirilmaEfekti != null)
        {
            GameObject vfx = Instantiate(kirilmaEfekti, transform.position, Quaternion.identity);
            Destroy(vfx, 1f); // Efekti temizle
        }
        
    }
}