using UnityEngine;

public class LightningVisual : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private float lifeTime = 0.2f; // Ekranda ne kadar kalsın? (Çok kısa olmalı)
    private float timer;

    [Header("Efekt Ayarları")]
    public float textureScrollSpeed = 10f; // Elektriğin akma hızı
    public float jitterAmount = 0.1f; // Titreme miktarı (Düz çizgi olmasın)

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Bu fonksiyonu dışarıdan çağıracağız
    public void Zap(Vector3 startPos, Vector3 endPos)
    {
        lineRenderer.positionCount = 2; // Basit başlangıç
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);

        // Zikzak etkisi için ortaya rastgele noktalar eklenebilir ama
        // şimdilik texture kaydırma yeterli olacaktır.

        timer = lifeTime;
    }

    void Update()
    {
        // Süre sayacı
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Destroy(gameObject);
            return;
        }

        // Fade Out (Yavaşça silinme)
        Color startColor = lineRenderer.startColor;
        startColor.a = timer / lifeTime;
        lineRenderer.startColor = startColor;
        lineRenderer.endColor = startColor;

        // Texture Kaydırma (Akım Hissi)
        if (lineRenderer.material != null)
        {
            float offset = Time.time * textureScrollSpeed;
            lineRenderer.material.mainTextureOffset = new Vector2(offset, 0);
        }

        // Opsiyonel: Uç noktanın hafif titremesi (Daha vahşi durur)
        // Eğer hareketli hedefe kilitli kalmasını istiyorsan buraya hedef takibi eklenmeli.
    }
}