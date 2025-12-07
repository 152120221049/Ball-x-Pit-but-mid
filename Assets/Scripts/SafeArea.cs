using UnityEngine;

public class SafeArea : MonoBehaviour
{
    private RectTransform rectTransform;
    private Rect currentSafeArea = new Rect();
    private ScreenOrientation currentOrientation = ScreenOrientation.AutoRotation;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        Refresh();
    }

    void Update()
    {
        // Ekran döndürülürse veya güvenli alan değişirse güncelle
        if ((currentOrientation != Screen.orientation) || (currentSafeArea != Screen.safeArea))
        {
            Refresh();
        }
    }

    void Refresh()
    {
        currentSafeArea = Screen.safeArea;
        currentOrientation = Screen.orientation;
        ApplySafeArea();
    }

    void ApplySafeArea()
    {
        if (rectTransform == null) return;

        Rect safeArea = Screen.safeArea;

        // Ekranın tam boyutuna göre oranla
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
    }
}