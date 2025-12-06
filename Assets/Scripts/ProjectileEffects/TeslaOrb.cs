using UnityEngine;
using System.Collections.Generic;

public class TeslaSphere : ProjectileEffects
{
    [Header("Tesla Ayarları")]
    public float yildirimMenzili = 3f;

    [Range(0f, 1f)]
    public float yildirimHasarOrani = 0.5f; // Ana hasarın %50'si

    public float sokSikligi = 0.5f;

    [Header("Görsel")]
    public GameObject elektrikEfekti;

    private Dictionary<GameObject, float> zapCooldowns = new Dictionary<GameObject, float>();

    // Start fonksiyonuna gerek yok, Baba (ProjectileEffects) hallediyor.

    void Update()
    {
        // 1. GÜNCEL ÇARPANI AL
        // LevelManager varsa oradaki güçlendirmeyi al, yoksa 1 kabul et.
        float enhanceMultiplier = (LevelManager.Instance != null) ? LevelManager.Instance.effectEnhance : 1f;

        // 2. ORANI HESAPLA
        // Örnek: 0.5 (Baz Oran) * 1.5 (Level Buff) = 0.75 (Yeni Oran)
        float totalRatio = yildirimHasarOrani * enhanceMultiplier;

        // 3. BABADAKİ FONKSİYONU ÇAĞIR
        ExecuteZap(yildirimMenzili, totalRatio, zapCooldowns, sokSikligi, elektrikEfekti);
    }

}