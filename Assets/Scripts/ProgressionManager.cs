using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ProgressionManager : MonoBehaviour
{
    public static ProgressionManager Instance;

    [Header("Tüm Levellar")]
    public List<LevelData> allLevels; // Sırasıyla Level 1, 2, 3...

    [Header("Anlık Durum")]
    public int currentLevelIndex = 0; // Şu an hangisi seçili?
    public int maxUnlockedLvlIndex = 0; // En son açılan level  
    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
        maxUnlockedLvlIndex = PlayerDataManager.Instance.maxLevelReached;
    }

    // Level Seçip Başlatma
    public void SelectLevelAndStart(int index)
    {
        if (index >= 0 && index < allLevels.Count)
        {
            currentLevelIndex = index;

            
            SceneManager.LoadScene("DemoLevel");
        }
        else
        {
            Debug.LogError("Hata: Olmayan bir level indeksi seçildi!");
        }
    }

    // Şu anki levelın verisini döndürür
    public LevelData GetCurrentLevelData()
    {
        if (allLevels.Count == 0) return null;
        return allLevels[currentLevelIndex];
    }

    // Bir sonraki levela geç
    public void LoadNextLevel()
    {
        int nextIndex = currentLevelIndex + 1;
        if (nextIndex < allLevels.Count)
        {
            SelectLevelAndStart(nextIndex);
        }
        else
        {
            Debug.Log("OYUN BİTTİ! Tüm levellar tamamlandı.");
            SceneManager.LoadScene("MainMenu");
        }
    }
    

    // Level kazanılınca bu çağrılacak
    public void UnlockNextLevel(int completedLevelIndex)
    {
        // Eğer bitirdiğim level, şu anki sınırımsa -> Bir sonrakini aç
        if (completedLevelIndex >= maxUnlockedLvlIndex)
        {
            maxUnlockedLvlIndex = completedLevelIndex + 1;
            PlayerDataManager.Instance.maxLevelReached = maxUnlockedLvlIndex;
            // Eğer toplam level sayısını aştıysak sınırla (GameProgressionManager'dan kontrol edebilirsin)
            if (ProgressionManager.Instance != null)
            {
                int totalLevels = ProgressionManager.Instance.allLevels.Count;
                if (maxUnlockedLvlIndex >= totalLevels)
                    maxUnlockedLvlIndex = totalLevels - 1;
            }

            Debug.Log($"İLERLEME KAYDEDİLDİ: Artık Level {maxUnlockedLvlIndex + 1} açık!");
            PlayerDataManager.Instance.SaveGame();
        }
    }
}