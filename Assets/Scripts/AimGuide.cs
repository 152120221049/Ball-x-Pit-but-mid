using UnityEngine;
using UnityEngine.EventSystems;

public class AimGuide : MonoBehaviour
{
    [Header("Ayarlar")]
    public Transform firePoint;
    public float maxDistance = 20f;
    public LayerMask collisionLayer;

    [Range(0f, 2f)]
    public float reticleOffset = 0.5f;

    [Header("Referanslar")]
    public Joystick movementJoystick;
    public Transform targetReticle; // Sembol (Reticle) objesi

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
        if (PlayerDataManager.Instance == null) return;

        ControlMode currentMode = PlayerDataManager.Instance.currentControlMode;

        if (currentMode == ControlMode.HybridJoystick)
        {
            // --- HİBRİT MOD: ÇİZGİ VE RETICLE HER ZAMAN AKTİF ---
            isAiming = true;
        }
        else
        {
            // --- DUAL MOD: SADECE DOKUNUNCA AKTİF ---
            isAiming = false;

            // Joystick ile yürürken çizgi çıkmasın
            if (movementJoystick.Direction.magnitude > 0.1f) return;

            if (Input.touchCount > 0)
            {
                foreach (Touch touch in Input.touches)
                {
                    if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
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
    }

    void DrawAimLine()
    {
        lineRenderer.SetPosition(0, firePoint.position);

        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.right, maxDistance, collisionLayer);
        Vector3 hitPoint = hit.collider != null ? (Vector3)hit.point : firePoint.position + firePoint.right * maxDistance;

        lineRenderer.SetPosition(1, hitPoint);

        if (targetReticle != null)
        {
            if (hit.collider != null)
            {
                // Çarpma noktasından biraz geri çek (Offset)
                targetReticle.position = (Vector3)hit.point - (firePoint.right * reticleOffset);
            }
            else
            {
                targetReticle.position = hitPoint;
            }

            // Sembolü kendi ekseninde döndür
            targetReticle.Rotate(0, 0, 100 * Time.deltaTime);
        }
    }
}