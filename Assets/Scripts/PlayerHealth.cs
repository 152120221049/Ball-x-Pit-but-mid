using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance; // Diğer scriptlerden ulaşmak için

    [Header("Can Ayarları")]
    public float maxHealth = 10f;
    public float currentHealth;
    public PauseManager pauseManager;
    public GameOverManager gameOverManager;
    [Header("iFrame (Hasar Almazlık)")]
    public float invincibilityDuration = 1f; // Hasar aldıktan sonra 1 saniye koruma
    private bool isInvincible = false;

    [Header("UI")]
    public Slider healthSlider;
    public Image damageFlashImage; // Opsiyonel: Hasar yiyince ekran kızarsın

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    // Hasar Alma Fonksiyonu (Dışarıdan çağrılabilir)
    public void TakeDamage(float amount)
    {
        if (isInvincible) return; // Koruma varsa hasar alma

        currentHealth -= amount;
        UpdateUI();

        // Titreme veya Ses efekti buraya eklenebilir
        Debug.Log($"Hasar Alındı! Kalan Can: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityRoutine());
        }
    }

    // Hasar Almazlık (Yanıp Sönme)
    IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;

        // Basit yanıp sönme efekti (Sprite Renderer'ı aç kapa)
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>(); // Visuals altındaysa oradan al
        if (sr == null) sr = GetComponent<SpriteRenderer>();

        for (float i = 0; i < invincibilityDuration; i += 0.2f)
        {
            if (sr != null) sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(0.1f);
        }

        if (sr != null) sr.enabled = true; // Görünür olduğundan emin ol
        isInvincible = false;
    }

    void UpdateUI()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.DOValue(currentHealth, 0.5f);
        }
    }

    void Die()
    {
        gameOverManager.ShowGameOver();
    }

    // --- DÜŞMAN KARAKTERE ÇARPARSA ---
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Düşman gövdeye çarptı -> 1 Hasar ver
            TakeDamage(1f);

            // Çarpan düşmanı geri itebiliriz veya yok edebiliriz?
            // Şimdilik sadece hasar alıyoruz, düşman itmeye devam ediyor.
        }
    }
}