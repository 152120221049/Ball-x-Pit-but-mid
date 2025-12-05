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

        // 1. BASE STATLARI HESAPLA
        if (projectileData != null)
        {
            float baseDamage = projectileData.damage;
            float speed = projectileData.speed; // Veya projectileData.speed

            // 2. LEVEL ÇARPANI (LevelManager varsa al, yoksa 1)
            float levelMultiplier = (LevelManager.Instance != null) ? LevelManager.Instance.damageMultiplier : 1f;

            // 3. FİNAL HASARI BELİRLE
            finalDamage = baseDamage * levelMultiplier;

            // Hızı Ayarla
            if (rb != null) rb.linearVelocity = transform.right * speed;

            // Ömrünü ayarla (Veride varsa onu kullan, yoksa 5sn)
            // Destroy(gameObject, projectileData.destroyTimer > 0 ? projectileData.destroyTimer : 5f);
        }
        else
        {
            Debug.LogError("ProjectileData Eksik! Lütfen Inspector'dan atama yapın.");
            finalDamage = 10f; // Fallback değer
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
                    enemyScript.TakeDamage(projectileData.damage * ratio);
                    cooldownList[enemyObj] = Time.time + cooldownTime;
                    zappedEnemies.Add(enemyScript);

                    // --- GÖRSEL KISIM BURADA DEĞİŞTİ ---
                    if (specificVFX != null)
                    {
                        // Efekti merminin merkezinde oluştur
                        GameObject boltObj = Instantiate(specificVFX, transform.position, Quaternion.identity);

                        // Efektin içindeki LightningVisual scriptini al
                        LightningVisual visual = boltObj.GetComponent<LightningVisual>();

                        if (visual != null)
                        {
                            // Ona "Buradan (Benim konumum) -> Oraya (Düşman konumu)" çiz diyoruz
                            visual.Zap(transform.position, enemyObj.transform.position);
                        }
                    }
                    // ------------------------------------
                }
            }
        }
        return zappedEnemies;
    }
}