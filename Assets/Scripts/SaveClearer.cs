#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class SaveClearer
{
    [MenuItem("Tools/Clear Save File")]
    public static void ClearSave()
    {
        string path = Path.Combine(Application.persistentDataPath, "savefile.json");

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"<color=green>Save Dosyası Silindi:</color> {path}");

            // PlayerPrefs'i de temizlemek istersen:
            // PlayerPrefs.DeleteAll();
            // Debug.Log("PlayerPrefs Silindi.");
        }
        else
        {
            Debug.LogWarning("Silinecek Save dosyası bulunamadı.");
        }
    }
}
#endif