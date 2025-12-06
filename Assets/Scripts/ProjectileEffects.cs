using UnityEngine;
using System.Collections.Generic;

public abstract class ProjectileEffects : MonoBehaviour
{
    [Header("Veri")]
    public ItemData projectileData; // Hasar ve Hız buradan çekilecek

    // Bu değişkeni çocuklar (Hamster vb.) kullanacak
    protected float finalDamage;

    protected Rigidbody2D rb;
    [Header("Duvar Davranışı")]
    public bool duvardaYokOl = false;
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (projectileData != null)
        {
            // 1. HIZ HESABI (Cooldown Reduction hızı etkilemez, sadece atış sıklığını etkiler)
            float speed = projectileData.speed;

            // --- HASAR HESABI (KRİTİK BÖLGE) ---
            float baseDamage = 0;

            // Eğer DataManager varsa (Oyundaysak), YÜKSELTİLMİŞ hasarı sor
            if (PlayerDataManager.Instance != null)
            {
                baseDamage = PlayerDataManager.Instance.GetModifiedDamage(projectileData);
                // Debug.Log($"Modifiye Hasar Alındı: {baseDamage} (Level: {PlayerDataManager.Instance.GetItemLevel(projectileData)})");
            }
            else
            {
                // Yoksa (Test) düz hasarı al
                baseDamage = projectileData.damage;
            }

            // 2. Level Çarpanı (Oyun içi Bufflar) ile çarp
            float levelMultiplier = (LevelManager.Instance != null) ? LevelManager.Instance.damageMultiplier : 1f;

            finalDamage = baseDamage * levelMultiplier;
            // -----------------------------------

            if (rb != null) rb.linearVelocity = transform.right * speed;
        }
        else
        {
            // Eğer veri yoksa hata vermemesi için
            finalDamage = 10f;
        }
    }

    // --- ÇARPIŞMA (Aynı kalıyor) ---
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();
            if (enemy != null) OnHitEnemy(enemy);
        }
        else if (collision.gameObject.CompareTag("Obstacles"))
        {
            OnHitWall();
        }
    }

    // --- TEMEL VURUŞ ---
    protected virtual void OnHitEnemy(EnemyBase enemy)
    {
        if (duvardaYokOl) 
        { 
            enemy.TakeDamage(finalDamage);
            Destroy(gameObject);
        }
        else
        {
            enemy.TakeDamage(finalDamage);
            Destroy(gameObject,projectileData.destroyTimer); 
        }
    }

    protected virtual void OnHitWall()
    {
        if (duvardaYokOl)
        {
            
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject, projectileData.destroyTimer);
        }
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