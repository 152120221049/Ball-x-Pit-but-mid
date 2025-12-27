using UnityEngine;
using UnityEngine.EventSystems;

public enum ControlMode { DualInput, HybridJoystick }

public class PlayerController : MonoBehaviour
{
    [Header("Sistem Ayarları")]
    public ControlMode currentMode = ControlMode.HybridJoystick;
    public bool isAutoFire = true;
    public RectTransform thresholdRing;
    [Header("Hareket Ayarları")]
    public float moveSpeed = 5f;
    public float turnSpeed = 720f;
    [Range(0f, 0.5f)] public float movementSmoothing = 0.1f;

    [Header("Hybrid Joystick Ayarları")]
    [Tooltip("Joystick merkezinden ne kadar uzaklaşınca hareket başlasın?")]
    public float moveThreshold = 0.5f;

    [Header("Aim Ayarları")]
    public float minAimAngle = -10f;
    public float maxAimAngle = 170f;

    [Header("Referanslar")]
    public Transform firePoint;
    public Transform visuals;
    public Joystick movementJoystick;
    public Animator animator;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 lastAimDir = Vector2.up;
    private bool inAimMode = false;
    private Vector2 currentVelocityRef;
    private WeaponSystem weaponSystem;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        weaponSystem = GetComponent<WeaponSystem>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        UpdateControlSettings();
    }
    public void UpdateControlSettings()
    {
        if (PlayerDataManager.Instance == null) return;

        // Ayarı Manager'dan al
        currentMode = PlayerDataManager.Instance.currentControlMode;

        // Joystick görselini ayarla
        RectTransform joystickRect = movementJoystick.GetComponent<RectTransform>();
        if (currentMode == ControlMode.HybridJoystick)
        {
            joystickRect.localScale = Vector3.one * 1.5f;
            moveThreshold = 0.6f;
            isAutoFire = true;
            if (thresholdRing != null)
            {
                thresholdRing.gameObject.SetActive(true);
                thresholdRing.localScale = Vector3.one * moveThreshold;
            }
        }
        else
        {
            joystickRect.localScale = Vector3.one;
            moveThreshold = 0.1f;
            isAutoFire = false; // 
            if (thresholdRing != null)
            {
                thresholdRing.gameObject.SetActive(false);

            }
        }
    }
    void Update()
    {
        HandleInput();

        bool isMoving = moveInput.magnitude > 0.1f;
        if (animator != null) animator.SetBool("isWalking", isMoving);

        HandleBobbing(isMoving);
        HandleFiring();
    }

    void HandleInput()
    {
        float inputMag = movementJoystick.Direction.magnitude;

        if (currentMode == ControlMode.HybridJoystick)
        {
            // --- HYBRID JOYSTICK MANTIĞI ---
            if (inputMag > 0.05f) // Çok küçük bir ölü bölge
            {
                // Her zaman nişan al (İç halka)
                lastAimDir = movementJoystick.Direction.normalized;
                HandleAimingRotation(lastAimDir);

                // Eğer dış halkaya çıktıysa hareket et
                if (inputMag > moveThreshold)
                    moveInput = movementJoystick.Direction.normalized;
                else
                    moveInput = Vector2.zero;
            }
            else
            {
                moveInput = Vector2.zero;
            }
        }
        else
        {
            // --- ESKİ SİSTEM (Dual Input) ---
            ReadMovementInput();
            ReadAimInput(); // Ekrana dokunma ile nişan
        }
    }

    void HandleAimingRotation(Vector2 aimDir)
    {
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        float clampedAngle = Mathf.Clamp(angle, minAimAngle, maxAimAngle);

        firePoint.rotation = Quaternion.Euler(0, 0, clampedAngle);
        visuals.rotation = Quaternion.Euler(0, 0, clampedAngle + -90f);
    }

    void HandleFiring()
    {
        if (currentMode == ControlMode.HybridJoystick)
        {
            // 1. Durum: Auto-Fire AÇIK (Default)
            // Joystick merkezden azıcık bile ayrıldıysa sürekli ateş et
            if (isAutoFire)
            {
                weaponSystem.TryFire();        
            }
            // 2. Durum: Auto-Fire KAPALI
            // Kullanıcı nişan alırken EKSTRA olarak ekrana basarsa ateş et
            else
            {
                if (Input.GetMouseButton(0) && !IsPointerOverUI())
                {
                    weaponSystem.TryFire();
                }
            }
        }
    }
    void FixedUpdate()
    {
        Vector2 targetVelocity = moveInput * moveSpeed;
        rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, targetVelocity, ref currentVelocityRef, movementSmoothing);
    }

    // --- ESKİ METODLARIN ADAPTASYONU ---
    void ReadMovementInput()
    {
        moveInput = movementJoystick.Direction.magnitude > 0.1f ? movementJoystick.Direction.normalized : Vector2.zero;
        if (!inAimMode && moveInput != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;
            visuals.rotation = Quaternion.RotateTowards(visuals.rotation, Quaternion.Euler(0, 0, angle - 90f), turnSpeed * Time.deltaTime);
        }
    }

    void ReadAimInput()
    {
        if (Input.GetMouseButton(0) && !IsPointerOverUI())
        {
            inAimMode = true;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            HandleAimingRotation((mousePos - transform.position).normalized);
        }
        else inAimMode = false;
    }

    void HandleBobbing(bool isMoving)
    {
        if (isMoving)
        {
            float bob = Mathf.Sin(Time.time * 15f) * 0.1f;
            visuals.localScale = new Vector3(1 + bob, 1 - bob, 1);
        }
        else visuals.localScale = Vector3.Lerp(visuals.localScale, Vector3.one, Time.deltaTime * 10f);
    }

    bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();
}