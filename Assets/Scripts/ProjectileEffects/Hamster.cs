using UnityEngine;
using System.Collections.Generic;

public class Hamster : ProjectileEffects
{
    [Header("Hamster Ayarları")]
    public float patlamaAlani = 3f;

    [Range(0f, 1f)] // Inspector'da kaydırma çubuğu yapar (0 ile 1 arası)
    public float kanamaOrani = 0.2f; // Ana hasarın %20'si kadar kanatır
    public int kanamaSuresi = 3;

    [Header("Görsel")]
    public GameObject patlamaEfekti;

    protected override void OnHitEnemy(EnemyBase enemy)
    {
        // 1. Hasarı Hesapla (Scale Et)
        // Eğer mermi 100 vuruyorsa, kanama 20 vuracak.
        float calculatedBleedDamage = finalDamage * kanamaOrani;

        // 2. Patlat (Patlama ana hasarı vursun)
        List<EnemyBase> victims = ExecuteExplosion(patlamaAlani, finalDamage, patlamaEfekti);

        // 3. Kanat (Hesaplanan küçük hasarı kullan)
        foreach (EnemyBase victim in victims)
        {
            ExecuteBleed(victim, calculatedBleedDamage, kanamaSuresi);
        }

        Destroy(gameObject);
    }

    protected override void OnHitWall()
    {
        // Duvara çarpınca da aynı mantık
        float calculatedBleedDamage = finalDamage * kanamaOrani;

        List<EnemyBase> victims = ExecuteExplosion(patlamaAlani, finalDamage, patlamaEfekti);
        foreach (EnemyBase victim in victims)
        {
            ExecuteBleed(victim, calculatedBleedDamage, kanamaSuresi);
        }
        Destroy(gameObject);
    }
}