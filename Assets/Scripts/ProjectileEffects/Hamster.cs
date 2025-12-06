using System.Collections.Generic;
using UnityEngine;

public class Hamster : ProjectileEffects
{
    [Header("Hamster Ayarları")]
    public float patlamaAlani = 3f;
    [Range(0f, 1f)]
    public float kanamaOrani = 0.2f;
    public int kanamaSuresi = 3;

    [Header("Görsel & Efekt")]
    public GameObject patlamaEfekti;

    // --- YENİ EKLENEN AYAR ---
    public float screenShakeAmount = 0.5f; // Sarsıntı şiddeti (0.5 ideal)
    // -------------------------

    protected override void OnHitEnemy(EnemyBase enemy)
    {
        // ... (Hasar hesaplamaları aynı) ...
        float enhanceMultiplier = (LevelManager.Instance != null) ? LevelManager.Instance.effectEnhance : 1f;

        // Kanama oranını artır (Örn: 0.2 * 1.5 = 0.3)
        float buffedBleedRatio = kanamaOrani * enhanceMultiplier;

        float calculatedBleedDamage = finalDamage * buffedBleedRatio;

        // Patlat
        List<EnemyBase> victims = ExecuteExplosion(patlamaAlani, finalDamage, patlamaEfekti);

        // --- YENİ: EKRANI SALLA ---
        if (CameraShaker.Instance != null)
        {
            // 0.2 saniye boyunca, belirlenen şiddette salla
            CameraShaker.Instance.Shake(0.2f, screenShakeAmount);
        }
        // --------------------------

        foreach (EnemyBase victim in victims)
        {
            ExecuteBleed(victim, calculatedBleedDamage *enhanceMultiplier, kanamaSuresi);
        }

        Destroy(gameObject);
    }

    protected override void OnHitWall()
    {
        // Duvara çarpınca da sallasın (Tok hissettirir)
        if (CameraShaker.Instance != null)
        {
            CameraShaker.Instance.Shake(0.2f, screenShakeAmount);
        }

        // ... (Diğer kodlar aynı) ...
        float calculatedBleedDamage = finalDamage * kanamaOrani;
        List<EnemyBase> victims = ExecuteExplosion(patlamaAlani, finalDamage, patlamaEfekti);
        foreach (EnemyBase victim in victims) ExecuteBleed(victim, calculatedBleedDamage, kanamaSuresi);
        Destroy(gameObject);
    }
}