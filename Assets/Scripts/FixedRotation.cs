using UnityEngine;

public class FixedRotation : MonoBehaviour
{
    private Quaternion initialRotation;

    void Awake()
    {
        // Oyun başladığında metin nasıl duruyorsa o açıyı kaydet
        initialRotation = transform.rotation;
    }

    void LateUpdate()
    {
        // Babası (Reticle) dönse bile, bu objenin açısını başlangıç açısına sabitle
        transform.rotation = initialRotation;
    }
}