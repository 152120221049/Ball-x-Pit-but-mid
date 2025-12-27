using UnityEngine;
using System.Collections.Generic;

public abstract class ProjectileEffects : MonoBehaviour
{
    [Header("Ayarlar")]
    public ItemData projectileData;

    // Eski scriptindeki 'isSingleUse' mantığını buraya taşıdık.
    // true ise: Çarptığı an yok olur (Örn: Domates)
    // false ise: İçinden geçer veya seker (Örn: Tesla, Fan)
    public bool isSingleUse = true;

    [HideInInspector] public float finalDamage;

    protected Rigidbody2D rb;

    // --- Fan Mermisi İçin Gerekli Değişken ---
    [HideInInspector] public bool isBuffedByFan = false;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (projectileData != null)
        {
            float speed = projectileData.speed;

            // --- 1. HASAR HESABI (Senin eski kodun aynısı) ---
            float baseDamage = 0;
            if (PlayerDataManager.Instance != null)
                baseDamage = PlayerDataManager.Instance.GetModifiedDamage(projectileData);
            else
                baseDamage = projectileData.damage;

            // --- 2. LEVEL BUFFLARI ---
            float levelBuffMultiplier = (LevelManager.Instance != null) ? LevelManager.Instance.damageMultiplier : 1f;

            finalDamage = baseDamage * levelBuffMultiplier;
            // ---------------------------------------------

            // Hız ver
            if (rb != null) rb.linearVelocity = transform.right * speed;

            // Eğer çarpınca yok olmuyorsa (Tesla gibi), süreyle yok olsun
            if (!isSingleUse)
            {
                float timer = projectileData.destroyTimer > 0 ? projectileData.destroyTimer : 5f;
                Destroy(gameObject, timer);
            }
        }
        else
        {
            finalDamage = 10f; // Güvenlik
        }
    }

    // --- ÇARPIŞMA MANTIĞI ---
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        // Not: Physics Matrix ayarını yaptığımız için Trigger kullanıyoruz.
        // Eğer Matrix ayarı yapmadıysan OnCollisionEnter2D kullanabilirsin.

        if (other.CompareTag("Enemy"))
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                OnHitEnemy(enemy);
            }
        }
        else if (other.CompareTag("Obstacles")) // Duvar vb.
        {
            OnHitWall();
        }
    }

    // --- VURUŞ FONKSİYONU ---
    protected virtual void OnHitEnemy(EnemyBase enemy)
    {
        // 1. Hasar ver
        enemy.TakeDamage(finalDamage);

        // 2. Varsa Özel Efekti Çalıştır (Inheritance ile gelen kısım)
        ApplyUniqueEffect(enemy);

        // 3. Eğer tek kullanımlıksa yok et
        if (isSingleUse)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnHitWall()
    {
        if (isSingleUse) Destroy(gameObject);
    }

    // --- MİRAS ALANLARIN DOLDURACAĞI FONKSİYON ---
    public virtual void ApplyUniqueEffect(EnemyBase target)
    {
        // Varsayılan olarak boş.
        // Standart mermi burayı boş bırakır.
    }

    // ========================================================================
    // --- YETENEK KÜTÜPHANESİ (Toolkit) ---
    // ========================================================================

    // PATLAMA (Hasarı parametre olarak alır)
    protected List<EnemyBase> ExecuteExplosion(float radius, float explosionDamage, GameObject specificVFX)
    {
        List<EnemyBase> victims = new List<EnemyBase>();

        if (specificVFX != null)
        {
            GameObject vfx = Instantiate(specificVFX, transform.position, Quaternion.identity);
            vfx.transform.localScale = Vector3.one * (radius * 2);
            Destroy(vfx, 1f);
        }

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D col in hitEnemies)
        {
            if (col.CompareTag("Enemy"))
            {
                EnemyBase target = col.GetComponent<EnemyBase>();
                if (target != null)
                {
                    target.TakeDamage(explosionDamage);
                    victims.Add(target);
                }
            }
        }
        return victims;
    }

    // KANAMA
    protected void ExecuteBleed(EnemyBase target, float bleedDmg, int ticks)
    {
        target.ApplyBleed(bleedDmg, ticks);
    }

    // DONDURMA
    protected void ExecuteFreeze(EnemyBase target, int power)
    {
        target.ApplyFrost(power);
    }

    protected List<EnemyBase> ExecuteZap(float range, float ratio, Dictionary<GameObject, float> cooldownList, float cooldownTime, GameObject specificVFX)
    {
        List<EnemyBase> zappedEnemies = new List<EnemyBase>();

        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, range);
        foreach (Collider2D col in enemiesInRange)
        {
            if (!col.CompareTag("Enemy")) continue;

            GameObject enemyObj = col.gameObject;

            if (!cooldownList.ContainsKey(enemyObj) || Time.time >= cooldownList[enemyObj])
            {
                EnemyBase enemyScript = col.GetComponent<EnemyBase>();
                if (enemyScript != null)
                {
                    // --- BURASI ÇOK ÖNEMLİ ---
                    // 'finalDamage' değişkeni Start() fonksiyonunda PlayerDataManager'dan çekilip hesaplanmıştı.
                    // Burada onu kullanıyoruz. (Örn: Lvl 5 ise 50 hasar * 0.5 ratio = 25 vurur)

                    float damageToDeal = finalDamage * ratio;
                    enemyScript.TakeDamage(damageToDeal);
                    // -------------------------

                    cooldownList[enemyObj] = Time.time + cooldownTime;
                    zappedEnemies.Add(enemyScript);

                    // Görsel Efekt (LightningBolt)
                    if (specificVFX != null)
                    {
                        GameObject bolt = Instantiate(specificVFX, transform.position, Quaternion.identity);
                        LightningVisual visual = bolt.GetComponent<LightningVisual>();
                        if (visual != null)
                        {
                            visual.Zap(transform.position, enemyObj.transform.position);
                        }
                    }
                }
            }
        }
        return zappedEnemies;
    }
}