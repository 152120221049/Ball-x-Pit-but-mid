using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Ayarlar")]
    public bool isSingleUse = false; // Tek kullanımlık mı? (Limon evet, Tesla hayır)
    public ItemData projectileData;

    // Artık hasarı dışarıdan (PlayerDataManager'dan) alıp hesaplayacağız
    [HideInInspector]public float finalDamage;
    private float destroyTimer;

    public void Start()
    {
        if (projectileData != null)
        {
            destroyTimer = projectileData.destroyTimer > 0 ? projectileData.destroyTimer : 5f; // Güvenlik önlemi

            // --- 1. BASE HASAR HESABI (UPGRADE DAHİL) ---
            float baseDamage = 0;

            // Eğer DataManager varsa (Oyundaysak), YÜKSELTİLMİŞ hasarı sor
            if (PlayerDataManager.Instance != null)
            {
                // Levelına göre artmış hasarı hesapla
                baseDamage = PlayerDataManager.Instance.GetModifiedDamage(projectileData);
            }
            else
            {
                // Yoksa (Test sahnesi) ham hasarı al
                baseDamage = projectileData.damage;
            }

            // --- 2. OYUN İÇİ BUFF ÇARPANI ---
            // Level atlayınca gelen şanslı bufflar (%10 hasar artışı vb.)
            float levelBuffMultiplier = (LevelManager.Instance != null) ? LevelManager.Instance.damageMultiplier : 1f;

            // --- 3. SONUÇ ---
            finalDamage = baseDamage * levelBuffMultiplier;
        }
        else
        {
            Debug.LogError("ProjectileData EKSİK! Lütfen Inspector'dan atayın.");
            finalDamage = 10f; // Hata vermesin diye varsayılan değer
        }

        // Eğer mermi tek kullanımlık değilse (Tesla gibi), süre sonunda yok olsun
        if (!isSingleUse)
        {
            Destroy(gameObject, destroyTimer);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();

        if (enemy != null)
        {
            // HESAPLANMIŞ HASARI KULLAN
            enemy.TakeDamage(finalDamage);

            if (isSingleUse)
            {
                Destroy(gameObject); // Çarpınca yok et
            }
        }
    }
}