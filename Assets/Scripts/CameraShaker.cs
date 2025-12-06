using UnityEngine;
using DG.Tweening; // DOTween kütüphanesi

public class CameraShaker : MonoBehaviour
{
    public static CameraShaker Instance;

    // Kameranın orijinal pozisyonunu saklayalım ki titreme bitince kaymasın
    private Vector3 originalPos;

    void Awake()
    {
        Instance = this;
        originalPos = transform.position;
    }

    public void Shake(float duration, float strength)
    {
        // 1. Önceki titremeyi durdur (Üst üste binip kamerayı uzaya fırlatmasın)
        transform.DOKill();

        // 2. Kamerayı orijinal yerine döndür (Garanti olsun)
        transform.position = originalPos;

        // 3. SALLA!
        // Duration: Ne kadar sürsün? (0.2sn genelde iyidir)
        // Strength: Ne kadar şiddetli? (0.5 hafif, 2.0 deprem)
        // Vibrato: Ne kadar sıklıkla titresin? (10-20 arası iyidir)
        transform.DOShakePosition(duration, strength, 20, 90, false, true)
                 .OnComplete(() => transform.position = originalPos); // Bitince yerine oturt
    }
}