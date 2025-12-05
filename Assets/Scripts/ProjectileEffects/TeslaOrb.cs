using UnityEngine;
using System.Collections.Generic;

public class TeslaSphere : ProjectileEffects
{
    [Header("Tesla Ayarları")]
    public float yildirimMenzili = 3f;

    // GDD: Utility hasarının %50'si kadar hasar verir.
    [Range(0f, 1f)]
    public float yildirimHasarOrani = 0.5f;

    public float sokSikligi = 0.5f;

    [Header("Görsel")]
    public GameObject elektrikEfekti;

    private Dictionary<GameObject, float> zapCooldowns = new Dictionary<GameObject, float>();

    void Update()
    {
        // Buraya dikkat: finalDamage değişkenini kullanıyoruz
        // Eğer finalDamage 50 ise (Level atlayınca arttıysa), yıldırım otomatikman 25 vuracak.
        ExecuteZap(yildirimMenzili, yildirimHasarOrani, zapCooldowns, sokSikligi, elektrikEfekti);
    }

    
}