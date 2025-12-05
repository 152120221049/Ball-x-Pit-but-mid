using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance;

    [Header("Zorluk Ayarları")]
    public float difficultyScaling = 0.1f; // Her dakika %10 zorlaşsın
    public float timeAlive = 0f;

    // Diğer scriptlerin okuyacağı çarpan
    public float currentDifficultyFactor = 1f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        // Oyuncunun hayatta kaldığı süreyi artır
        timeAlive += Time.deltaTime;

        // Formül: 1 + (Dakika * Scaling)
        // Örn: 2. dakikada -> 1 + (2 * 0.1) = 1.2 (%20 daha zor)
        currentDifficultyFactor = 1f + (timeAlive / 60f) * difficultyScaling;
    }

    // Düşman Canı için Çarpan
    public float GetHealthMultiplier()
    {
        return currentDifficultyFactor;
    }

    // Düşman Hızı için Çarpan (Can kadar hızlı artmasın, yarısı kadar artsın)
    public float GetSpeedMultiplier()
    {
        return 1f + ((currentDifficultyFactor - 1f) * 0.5f);
    }
}