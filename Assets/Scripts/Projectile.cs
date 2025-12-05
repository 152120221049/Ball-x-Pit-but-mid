using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Ayarlar")]
    public bool isSingleUse = false; // Tek kullanımlık mı?
    public ItemData projectileData;
    public float destroyTimer;
    public float multiplier = 1f;
    public void Start()
    {
        destroyTimer = projectileData.destroyTimer;
        if(projectileData!= null)
        {
            float baseDamage = projectileData.damage;

            // 2. LevelManager varsa çarpanı al, yoksa 1 kabul et (Hata vermesin)
            multiplier = (LevelManager.Instance != null) ? LevelManager.Instance.damageMultiplier : 1f;
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Çarptığımız obje bir düşman mı?
        // EnemyBase scriptine erişmeye çalışıyoruz
        EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();

        if (enemy != null)
        {
            // Hasar ver
            enemy.TakeDamage(projectileData.damage * multiplier); // Örnek hasar değeri

            if(isSingleUse)
            {
                Destroy(gameObject); // Mermiyi yok et
            }

        }
        if(!isSingleUse) {Destroy(gameObject,destroyTimer); }

    }
}