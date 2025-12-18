using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Ayarlar")]
    // Bu liste GameProgressionManager tarafından otomatik doldurulur.
    // Ama test için elle de doldurabilirsin.
    public List<WavePattern> patterns;
    public float timeBetweenWaves = 5f;
    public LayerMask enemyLayer; // "Enemy" layerı seçili olmalı!

    [Header("Seviye Ayarları")]
    public int totalWavesInLevel = 5; // Bu seviye kaç dalga sürecek?

    [Header("Görsel")]
    public SpriteRenderer backgroundRenderer; // Sahne arkaplanı (Levela göre renk değişimi için)

    // Değişkenler
    private int wavesSpawnedCount = 0;
    private float countdown = 2f;
    private bool levelIsOver = false;

    void Start()
    {
        
        if (ProgressionManager.Instance != null)
        {
            LevelData data = ProgressionManager.Instance.GetCurrentLevelData();

            if (data != null)
            {
                // 1. Düşman Desenlerini Al
                this.patterns = data.wavePatterns;

                // 2. Dalga Sayısını Al
                this.totalWavesInLevel = data.totalWaves;

                // 3. Arka Plan Rengini Değiştir
                if (backgroundRenderer != null)
                {
                    backgroundRenderer.color = data.backgroundColor;
                }

                Debug.Log($"LEVEL YÜKLENDİ: {data.levelName} (Zorluk: {data.difficultyMultiplier}x)");

                // İstersen DifficultyManager'a levelın zorluk çarpanını da gönderebilirsin
                // DifficultyManager.Instance.currentDifficultyFactor = data.difficultyMultiplier;
            }
        }
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBattleMusic();
        }
    }

    void Update()
    {
        if (levelIsOver) return;

        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            Debug.Log($"RAPOR: Atılan Dalga: {wavesSpawnedCount}/{totalWavesInLevel} | Kalan Düşman: {enemies.Length}");
            foreach (var e in enemies) Debug.LogWarning($"DÜŞMAN BULUNDU: {e.name}");
        }
        

        // --- 1. AŞAMA: SPAWN (DOĞURMA) ---
        if (wavesSpawnedCount < totalWavesInLevel)
        {
            if (countdown <= 0f)
            {
                SpawnRandomPattern();

                wavesSpawnedCount++;

                // Zorluk arttıkça spawn süresi kısalır
                float difficulty = (DifficultyManager.Instance != null) ? DifficultyManager.Instance.currentDifficultyFactor : 1f;
                countdown = timeBetweenWaves / difficulty;
            }
            countdown -= Time.deltaTime;
        }
        // --- 2. AŞAMA: TEMİZLİK KONTROLÜ ---
        else
        {
            // Dalgalar bitti. Sahnede düşman kaldı mı?
            // "wavesSpawnedCount > 0" kontrolü, oyun başlar başlamaz bitmesini engeller.
            if (wavesSpawnedCount > 0 && AreAllEnemiesDead())
            {
                LevelComplete();
            }
        }
    }

    void SpawnRandomPattern()
    {
        if (patterns == null || patterns.Count == 0) return;

        int randomIndex = Random.Range(0, patterns.Count);
        WavePattern pattern = patterns[randomIndex];

        if (GridManager.Instance != null) GridManager.Instance.ClearGrid();

        for (int col = 0; col < pattern.enemies.Length; col++)
        {
            GameObject enemyObj = pattern.enemies[col];

            if (enemyObj != null)
            {
                EnemyBase enemyScript = enemyObj.GetComponent<EnemyBase>();
                if (enemyScript != null)
                {
                    int width = enemyScript.gridWidth;
                    int height = enemyScript.gridHeight;

                    // 1. Grid Kontrolü
                    if (GridManager.Instance.CanSpawnAt(col, 0, width, height))
                    {
                        Vector3 spawnPos = GridManager.Instance.GetWorldPosition(col, 0, width, height);

                        // 2. Fiziksel Kontrol (İç içe geçmeyi önler)
                        if (IsPositionFree(spawnPos, width, height))
                        {
                            GameObject newEnemy = Instantiate(enemyObj, spawnPos, Quaternion.identity);

                            // 3. Güçlendirme Uygula
                            ApplyDifficultyScaling(newEnemy);

                            GridManager.Instance.OccupyGrid(col, 0, width, height);
                        }
                    }
                }
            }
        }
    }

    bool AreAllEnemiesDead()
    {
        // Sahnede "Enemy" etiketli obje var mı?
        return GameObject.FindGameObjectWithTag("Enemy") == null;
    }

    void LevelComplete()
    {
        Debug.Log("SEVİYE TAMAMLANDI! Zafer Ekranı Açılıyor...");
        levelIsOver = true;

        if (GameOverManager.Instance != null)
        {
            // TRUE = Zafer (Yeşil Ekran)
            GameOverManager.Instance.ShowResult(true);
        }
    }

    bool IsPositionFree(Vector3 centerPos, int width, int height)
    {
        // GridManager yoksa varsayılan boyut kullan
        float cellSize = (GridManager.Instance != null) ? GridManager.Instance.cellSize : 1.5f;

        Vector2 boxSize = new Vector2(
            width * cellSize * 0.9f,
            height * cellSize * 0.9f
        );

        Collider2D hit = Physics2D.OverlapBox(centerPos, boxSize, 0f, enemyLayer);
        return hit == null;
    }

    void ApplyDifficultyScaling(GameObject enemy)
    {
        if (DifficultyManager.Instance == null) return;

        EnemyBase enemyStats = enemy.GetComponent<EnemyBase>();
        if (enemyStats != null)
        {
            float hpMultiplier = DifficultyManager.Instance.GetHealthMultiplier();
            enemyStats.maxHealth *= hpMultiplier;
            enemyStats.currentHealth = enemyStats.maxHealth;

            
        }

    }
}