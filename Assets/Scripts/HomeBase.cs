using UnityEngine;

public class HomeBase : MonoBehaviour
{
    public PlayerHealth pluh;
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Not: Collision2D'de objeye ulaşmak için '.gameObject' eklemeliyiz
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // 1. Oyuncuya Hasar Ver
            if (pluh != null)
            {
                // Düşman eve girdiği için 1 hasar veriyoruz
                pluh.TakeDamage(1f);
            }

            // 2. Düşmanı Yok Et
            Destroy(collision.gameObject);
        }
    }
}