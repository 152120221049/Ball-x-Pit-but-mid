using UnityEngine;
using System.IO;

public static class SaveSystem
{
    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true); // true = okunabilir format
        string path = GetPath();

        File.WriteAllText(path, json);
        Debug.Log("Oyun Kaydedildi: " + path);
    }

    public static SaveData Load()
    {
        string path = GetPath();

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            return data;
        }
        else
        {
            Debug.LogWarning("Kayıt dosyası bulunamadı, yeni oluşturuluyor.");
            return new SaveData(); // Varsayılan boş veri döndür
        }
    }

    private static string GetPath()
    {
        // PC'de: C:/Users/Ad/AppData/LocalLow/Sirket/OyunAdi/savefile.json
        // Mobilde: Uygulama veri klasörü
        return Path.Combine(Application.persistentDataPath, "savefile.json");
    }

    // Test amaçlı dosyayı silmek istersen
    public static void DeleteSave()
    {
        string path = GetPath();
        if (File.Exists(path)) File.Delete(path);
    }
}