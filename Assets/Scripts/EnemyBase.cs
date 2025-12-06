using UnityEngine;
using System.Collections;
using DG.Tweening;
public class EnemyBase : MonoBehaviour
{
    [Header("Statlar")]
    public float maxHealth = 10f;
    public float currentHealth;
    public int gridWidth = 2;
    public int gridHeight = 2;

    [Header("Görsel Ayarlar")]
    // Can azaldıkça dönüşeceği renk (Örn: Koyu Gri)
    public Color damagedColor = new Color(0.3f, 0.3f, 0.3f, 1f);

    [Header("Görsel Efekt Referansları")]
    public GameObject bleedVFXPrefab;
    public GameObject freezeVFXPrefab;
    public GameObject ExpOrbPrefab;
    public GameObject damagePopupPrefab;
    public GameObject hitVfx;
    // Durumlar
    private int frostStacks = 0;
    private bool isFrozen = false;
    private SpriteRenderer spriteRenderer;
    private Color originalColor; // Düşmanın en baştaki orijinal rengi
    private Vector3 baseScale;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseScale = transform.localScale;
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color; // Başlangıç rengini kaydet

        UpdateVisuals(); // İlk rengi ayarla
    }

    public void TakeDamage(float amount)
    {
        bool isCritical = false;
        transform.DOKill();
        transform.localScale = baseScale;
        transform.DOPunchScale(baseScale * 0.3f, 0.2f, 10, 1)
                 .OnComplete(() => transform.localScale = baseScale);
        float enhanceMultiplier = (LevelManager.Instance != null) ? LevelManager.Instance.effectEnhance : 1f;
        if (isFrozen)
        {
            amount *= 3.6f;
            isCritical = true; // Kritik vuruş!
            Debug.Log("<color=cyan>KRİTİK HASAR! (Donmuş)</color>");
            BreakFreeze();
        }
        else if (frostStacks > 0)
        {
            amount *= 1f+(0.2f*enhanceMultiplier);
        }

        currentHealth -= amount;

        // --- HASAR YAZISINI OLUŞTUR ---
        if (damagePopupPrefab != null)
        {
            // Düşmanın pozisyonunda oluştur
            GameObject popup = Instantiate(damagePopupPrefab, transform.position, Quaternion.identity);

            // Scriptine ulaş ve ayarla
            DamagePopup popupScript = popup.GetComponent<DamagePopup>();
            if (popupScript != null)
            {
                popupScript.Setup(amount, isCritical);
            }
        }
        if (hitVfx != null)
        {
            // 1. Efekti Oluştur
            GameObject vfx = Instantiate(hitVfx, transform.position, Quaternion.identity);

            // 2. Particle System Bileşenine Ulaş
            ParticleSystem ps = vfx.GetComponent<ParticleSystem>();

            if (ps != null && spriteRenderer != null)
            {
                // 3. RENGİ DEĞİŞTİR
                // Particle System'in "Main" modülüne erişiyoruz
                var mainSettings = ps.main;

                // Efektin başlangıç rengini, düşmanın ŞU ANKİ rengine eşitle
                mainSettings.startColor = spriteRenderer.color;
            }

            // Sahneyi temizle
            Destroy(vfx, 1f);
        }
        // ------------------------------

        StartCoroutine(FlashColor(Color.white, 0.1f));

        if (currentHealth <= 0) Die();
        else UpdateVisuals();
    }

    
    void UpdateVisuals()
    {
        if (spriteRenderer == null) return;

        // 1. ÖNCELİK: Donma Durumu
        if (isFrozen)
        {
            spriteRenderer.color = Color.cyan; // Tam Mavi
            return;
        }

        // 2. ÖNCELİK: Buzlanma Yükü
        if (frostStacks > 0)
        {
            // Orijinal ile Mavi arasında geçiş yap
            spriteRenderer.color = Color.Lerp(originalColor, Color.cyan, frostStacks * 0.3f);
            return;
        }

        // 3. ÖNCELİK: Hasar Durumu (Normal)
        // Can yüzdesini hesapla (0 ile 1 arası)
        float healthPercent = currentHealth / maxHealth;

        // Can azaldıkça DamagedColor'a (Griye), can çoksa OriginalColor'a yaklaş
        spriteRenderer.color = Color.Lerp(damagedColor, originalColor, healthPercent);
    }

    // --- DONMA (FROST) İŞLEMLERİ ---
    public void ApplyFrost(int amount)
    {
        
        if (isFrozen) return;

        frostStacks += amount;

        if (frostStacks >= 3)
        {
            ApplyFreeze();
        }
        else
        {
            UpdateVisuals(); 
        }
    }

    void ApplyFreeze()
    {
        isFrozen = true;
        frostStacks = 0;
        Debug.Log("DÜŞMAN DONDU!");

        if (freezeVFXPrefab != null)
        {
            GameObject vfx = Instantiate(freezeVFXPrefab, transform.position, Quaternion.identity);
            vfx.transform.SetParent(transform);
            Destroy(vfx, 2f);
        }


        UpdateVisuals(); 
    }

    void BreakFreeze()
    {
        isFrozen = false;
        UpdateVisuals(); 
    }

    // --- KANAMA (BLEED) İŞLEMLERİ ---
    public void ApplyBleed(float damagePerTick, int ticks)
    {
        Debug.Log("Kanama Başladı!");
        StartCoroutine(BleedRoutine(damagePerTick, ticks));
    }

    IEnumerator BleedRoutine(float dmg, int count)
    {
        GameObject activeBleedVFX = null;
        if (bleedVFXPrefab != null)
        {
            activeBleedVFX = Instantiate(bleedVFXPrefab, transform.position, Quaternion.identity);
            activeBleedVFX.transform.SetParent(transform);
        }

        for (int i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(1f);

            if (this == null || currentHealth <= 0) break;

            TakeDamage(dmg);
            // Not: TakeDamage içinde zaten FlashColor ve UpdateVisuals çağrıldığı için
            // burada ekstra bir şey yapmana gerek yok.
        }

        if (activeBleedVFX != null) Destroy(activeBleedVFX);
    }

    // GÜNCELLENMİŞ FLASH COLOR
    IEnumerator FlashColor(Color flashColor, float duration)
    {
        if (spriteRenderer == null) yield break;

        // 1. Anlık Beyaz Yap
        spriteRenderer.color = flashColor;

        yield return new WaitForSeconds(duration);

        // 2. Bekleme bitince "Eski Renge" değil "GÜNCEL DOĞRU RENGE" dön
        // (Çünkü can azaldığı için renk değişmiş olmalı)
        UpdateVisuals();
    }

    protected virtual void Die()
    {
        if (ExpOrbPrefab != null)
        {
            Instantiate(ExpOrbPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}