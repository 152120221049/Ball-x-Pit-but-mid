using UnityEngine;

public class SafeArea : MonoBehaviour
{
    RectTransform myRect;

    void Awake()
    {
        myRect = GetComponent<RectTransform>();
        Refresh();
    }

    void Refresh()
    {
        Rect safeArea = Screen.safeArea;
        Rect pixelRect = new Rect(0, 0, Screen.width, Screen.height); // Tam ekran

        // Safe Area'yı Anchor pozisyonuna çevir
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= pixelRect.width;
        anchorMin.y /= pixelRect.height;
        anchorMax.x /= pixelRect.width;
        anchorMax.y /= pixelRect.height;

        // Uygula
        myRect.anchorMin = anchorMin;
        myRect.anchorMax = anchorMax;

        // Debug.Log($"Safe Area Uygulandı: {anchorMin} - {anchorMax}");
    }
}