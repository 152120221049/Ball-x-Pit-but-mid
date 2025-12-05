using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [Header("Grid Ayarları")]
    public int columns = 5;      // Yatayda kaç sütun var? (Örn: 5)
    public float cellSize = 1.5f; // Bir karenin Unity birimi cinsinden boyutu
    public Vector2 startPoint;    // Sol üst köşenin başlangıç pozisyonu (X, Y)

    // Grid durumunu tutan harita (x, y) -> Dolu mu?
    // Key: "x,y" stringi, Value: Dolu mu?
    private Dictionary<string, bool> gridOccupancy = new Dictionary<string, bool>();

    void Awake()
    {
        Instance = this;
    }

    // Bir düşman için belirtilen koordinat uygun mu?
    public bool CanSpawnAt(int startCol, int startRow, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int targetCol = startCol + x;
                int targetRow = startRow + y;

                // 1. Sahne Dışına Taşıyor mu?
                if (targetCol >= columns) return false;

                // 2. Orası Dolu mu?
                if (IsOccupied(targetCol, targetRow)) return false;
            }
        }
        return true;
    }

    // Grid'i doldur (Spawn edildiğinde çağırılır)
    public void OccupyGrid(int startCol, int startRow, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                string key = $"{startCol + x},{startRow + y}";
                gridOccupancy[key] = true;
            }
        }
    }

    // Grid'i boşalt (Düşman öldüğünde veya aşağı kaydığında çağırılır)
    // Şimdilik sadece yeni spawn için kullanacağız, aşağı inenleri WaveSpawner temizleyecek.
    public void ClearGrid()
    {
        gridOccupancy.Clear();
    }

    bool IsOccupied(int col, int row)
    {
        string key = $"{col},{row}";
        return gridOccupancy.ContainsKey(key) && gridOccupancy[key];
    }

    // Grid koordinatını ve düşman boyutunu alıp TAM MERKEZİ döndürür
    public Vector3 GetWorldPosition(int col, int row, int width, int height)
    {
        // 1. Hedefin toplam dünya genişliğini ve yüksekliğini bul
        float totalWorldWidth = width * cellSize;
        float totalWorldHeight = height * cellSize;

        // 2. Başlangıç noktasından grid kadar git + yarım boy kadar daha git (Merkezle)
        float posX = startPoint.x + (col * cellSize) + (totalWorldWidth / 2f);
        float posY = startPoint.y - (row * cellSize) - (totalWorldHeight / 2f); // Aşağı indiği için eksi

        // Not: Sprite'larının Pivot noktası "Center" olmalı.
        return new Vector3(posX, posY, 0);
    }

    // Editörde Grid'i görmek için (Ayar yaparken hayat kurtarır)
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < 10; y++) // Örnek olarak 10 satır çizelim
            {
                Vector3 center = new Vector3(startPoint.x + (x * cellSize), startPoint.y - (y * cellSize), 0);
                Gizmos.DrawWireCube(center, new Vector3(cellSize, cellSize, 0.1f));
            }
        }

        // Başlangıç noktasını göster
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(startPoint, 0.2f);
    }
}