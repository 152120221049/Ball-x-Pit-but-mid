using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Ayarlar")]
    public List<WavePattern> patterns;
    public float timeBetweenWaves = 5f;
    public LayerMask enemyLayer; // "Enemy" layerını buradan seçeceksin

    private float countdown = 2f;

    void Update()
    {
        if (countdown <= 0f)
        {
            SpawnRandomPattern();
            float difficulty = (DifficultyManager.Instance != null) ? DifficultyManager.Instance.currentDifficultyFactor : 1f;
            countdown = timeBetweenWaves / difficulty;
        }
        countdown -= Time.deltaTime;
    }
    void SpawnRandomPattern()
    {
        if (patterns.Count == 0) return;

        // 1. Rastgele bir desen seç
        int randomIndex = Random.Range(0, patterns.Count);
        WavePattern pattern = patterns[randomIndex];

        // Grid'i sanal olarak temizle
        if (GridManager.Instance != null) GridManager.Instance.ClearGrid();

        // Desendeki her slotu gez
        for (int col = 0; col < pattern.enemies.Length; col++)
        {
            GameObject enemyObj = pattern.enemies[col];

            // Eğer bu slotta bir düşman tanımlıysa
            if (enemyObj != null)
            {
                EnemyBase enemyScript = enemyObj.GetComponent<EnemyBase>();

                if (enemyScript != null)
                {
                    int width = enemyScript.gridWidth;
                    int height = enemyScript.gridHeight;

                    // A. GridManager'a sor: Matematiksel olarak sığıyor muyum?
                    if (GridManager.Instance.CanSpawnAt(col, 0, width, height))
                    {
                        Vector3 spawnPos = GridManager.Instance.GetWorldPosition(col, 0, width, height);

                        // B. Fiziksel Kontrol: Doğacağım yerde yaşayan bir düşman var mı?
                        if (IsPositionFree(spawnPos, width, height))
                        {
                            // Düşmanı Oluştur
                            GameObject newEnemy = Instantiate(enemyObj, spawnPos, Quaternion.identity);

                            // C. Düşmanı Güçlendir (Zorluk Çarpanı)
                            ApplyDifficultyScaling(newEnemy);

                            // Grid'i işgal et
                            GridManager.Instance.OccupyGrid(col, 0, width, height);
                        }
                        else
                        {
                            // Debug.LogWarning($"Yer dolu! (Fiziksel Çakışma) Sütun: {col}");
                        }
                    }
                }
            }
        }
    }
    void ApplyDifficultyScaling(GameObject enemy)
    {
        if (DifficultyManager.Instance == null) return;

        // 1. Canı Artır
        EnemyBase enemyStats = enemy.GetComponent<EnemyBase>();
        if (enemyStats != null)
        {
            float hpMultiplier = DifficultyManager.Instance.GetHealthMultiplier();
            enemyStats.maxHealth *= hpMultiplier;
            enemyStats.currentHealth = enemyStats.maxHealth; // Canı fulle

            // Eğer üzerinde can yazısı varsa güncellemek için bir fonksiyon çağırabilirsin
            // enemyStats.UpdateHealthUI(); // (Public ise)
        }

        // 2. Hızı Artır
        EnemyMovement enemyMove = enemy.GetComponent<EnemyMovement>();
        if (enemyMove != null)
        {
            float speedMultiplier = DifficultyManager.Instance.GetSpeedMultiplier();
            enemyMove.speed *= speedMultiplier;
        }
    }
    // O bölgeye hayali bir kutu atıp çarpan var mı diye bakar
    bool IsPositionFree(Vector3 centerPos, int width, int height)
    {
        // Kutunun boyutu (Grid boyutuna göre)
        // Biraz küçültüyoruz (0.9f) ki yanındaki düşmana yanlışlıkla değmesin.
        Vector2 boxSize = new Vector2(
            width * GridManager.Instance.cellSize * 0.9f,
            height * GridManager.Instance.cellSize * 0.9f
        );

        // Physics2D.OverlapBox ile o bölgeyi tara
        Collider2D hit = Physics2D.OverlapBox(centerPos, boxSize, 0f, enemyLayer);

        // Eğer 'hit' null ise orası boştur, true döner.
        // Eğer bir şeye çarptıysa doludur, false döner.
        return hit == null;
    }

    // Alanı görmek için (Debug)
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // Gizmo çizimi sadece runtime'da çalışır çünkü GridManager.Instance lazım
    }
}