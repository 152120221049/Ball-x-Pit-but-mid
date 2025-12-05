using UnityEngine;

public class ExpOrb : MonoBehaviour
{
    [Header("Değer Ayarları")]
    public float expAmount = 10f;

    [Header("Mıknatıs Ayarları")]
    public float magnetRange = 4f;    // Oyuncu ne kadar yaklaşınca harekete geçsin?
    public float moveSpeed = 2f;      // Başlangıç hızı (Yavaş)
    public float acceleration = 10f;   // Zamanla ne kadar hızlansın? (İvme)

    private Transform playerTransform;
    private bool isMagnetized = false; // Çekim başladı mı?

    void Start()
    {
        // Oyuncuyu Tag ile bul (PlayerHealth.Instance üzerinden de bulabiliriz)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // 1. MESAFE KONTROLÜ
        // Eğer henüz çekilmeye başlamadıysa mesafeye bak
        if (!isMagnetized)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);

            // Menzile girdiyse bayrağı kaldır
            if (distance < magnetRange)
            {
                isMagnetized = true;
            }
        }

        // 2. HAREKET MANTIĞI
        // Bir kere çekilmeye başladıysa artık durmaz, oyuncuya kadar gider
        if (isMagnetized)
        {
            // Hızı zamanla artır (Doğal görünüm için)
            moveSpeed += acceleration * Time.deltaTime;

            // Oyuncuya doğru adım at
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Karakter toplarsa
        if (other.CompareTag("Player"))
        {
            if (LevelManager.Instance != null)
                LevelManager.Instance.AddExp(expAmount);

            Destroy(gameObject);
        }
        else if (other.CompareTag("Home"))
        {
            // Eğer toplayamadan aşağı düşerse yok olsun
            Destroy(gameObject);
        }
    }

    // Editörde çekim alanını görmek için yeşil çember çizer
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, magnetRange);
    }
}