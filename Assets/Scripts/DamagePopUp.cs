using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;
    private Vector3 moveVector;

    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    // Bu fonksiyonu EnemyBase çağıracak
    public void Setup(float damageAmount, bool isCriticalHit)
    {
        // Sayıyı yaz
        textMesh.text = Mathf.CeilToInt(damageAmount).ToString();

        if (isCriticalHit)
        {
            // Kritikse (Donmuşsa) Büyük ve Kırmızı/Turuncu olsun
            textMesh.fontSize = 5;
            textColor = new Color(1f, 0.5f, 0f); // Turuncu
        }
        else
        {
            // Normalse Sarı
            textMesh.fontSize = 3;
            textColor = Color.yellow;
        }

        textMesh.color = textColor;
        disappearTimer = 1f; // 1 saniyede yok olsun

        // Yukarı ve hafif rastgele sağa/sola hareket etsin
        moveVector = new Vector3(Random.Range(-1f, 1f), 3f);
    }

    void Update()
    {
        // 1. Yukarı Hareket
        transform.position += moveVector * Time.deltaTime;

        // Hareket yavaşlasın (Yerçekimi etkisi gibi)
        moveVector -= moveVector * 2f * Time.deltaTime;

        // 2. Zamanla Yok Olma
        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            // Alpha değerini düşür (Silikleş)
            float fadeSpeed = 3f;
            textColor.a -= fadeSpeed * Time.deltaTime;
            textMesh.color = textColor;

            // Tamamen görünmez olunca objeyi yok et
            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}