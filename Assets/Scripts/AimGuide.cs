using UnityEngine;
using UnityEngine.EventSystems;

public class AimGuide : MonoBehaviour
{
    [Header("Ayarlar")]
    public Transform firePoint;
    public float maxDistance = 20f;
    public LayerMask collisionLayer;

    // --- YENİ EKLENEN AYAR ---
    [Range(0f, 2f)]
    public float reticleOffset = 0.5f; // Sembolü ne kadar geri çekeceğiz?
    // -------------------------

    [Header("Referanslar")]
    public Joystick movementJoystick;
    public Transform targetReticle;

    private LineRenderer lineRenderer;
    private bool isAiming = false;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;

        if (targetReticle != null)
            targetReticle.gameObject.SetActive(false);
    }

    void Update()
    {
        CheckAimInput();

        if (isAiming)
        {
            lineRenderer.enabled = true;
            if (targetReticle != null) targetReticle.gameObject.SetActive(true);
            DrawAimLine();
        }
        else
        {
            lineRenderer.enabled = false;
            if (targetReticle != null) targetReticle.gameObject.SetActive(false);
        }
    }

    void CheckAimInput()
    {
        // (Burası aynı kalıyor - Joystick kontrolü vb.)
        isAiming = false;

        if (movementJoystick != null && movementJoystick.Direction.magnitude > 0.1f) return;

        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                int id = touch.fingerId;
                if (!EventSystem.current.IsPointerOverGameObject(id))
                {
                    isAiming = true;
                    break;
                }
            }
        }
        else if (Input.GetMouseButton(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject()) isAiming = true;
        }
    }

    // --- GÜNCELLENEN KISIM ---
    void DrawAimLine()
    {
        lineRenderer.SetPosition(0, firePoint.position);

        // Işın atıyoruz
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.right, maxDistance, collisionLayer);

        Vector3 hitPoint;

        if (hit.collider != null)
        {
            hitPoint = hit.point;
        }
        else
        {
            // Boşa gidiyorsa
            hitPoint = firePoint.position + firePoint.right * maxDistance;
        }

        // 1. Çizgiyi ayarla (Çizgi tam duvara kadar gitsin, onda sorun yok)
        lineRenderer.SetPosition(1, hitPoint);

        // 2. Reticle'ı ayarla (Burası değişti)
        if (targetReticle != null)
        {
            // Eğer bir şeye çarptıysak
            if (hit.collider != null)
            {
                // Çarpma noktasından, merminin geldiği yönün TERSİNE (firePoint.right) doğru
                // 'reticleOffset' kadar geri gel.
                Vector3 offsetPosition = (Vector3)hit.point - (firePoint.right * reticleOffset);
                targetReticle.position = offsetPosition;
            }
            else
            {
                // Boşa gidiyorsa direkt uca koy
                targetReticle.position = hitPoint;
            }

            // Dönme efekti devam etsin
            targetReticle.Rotate(0, 0, 100 * Time.deltaTime);
        }
    }
}