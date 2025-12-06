using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float moveSpeed = 5f;
    public float turnSpeed = 720f;

    [Range(0f, 0.5f)]
    public float movementSmoothing = 0.1f;

    [Header("Görsel Ayarlar (Juice)")]
    public float bobbingSpeed = 15f;
    public float bobbingAmount = 0.1f;
    public float rotationOffset = -90f;

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
    private bool inAimMode = false;

    private Vector2 currentVelocityRef;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        ReadMovementInput();
        ReadAimInput();

        bool isMoving = moveInput.magnitude > 0.1f;

        if (animator != null)
            animator.SetBool("isWalking", isMoving);

        HandleBobbing(isMoving);

        // --- ROTATION ---
        if (inAimMode)
        {
            HandleAimingRotation();
            return;  // Movement rotasyonunu tamamen engelliyoruz
        }

        if (isMoving)
        {
            HandleMovementRotation();
        }
    }

    void FixedUpdate()
    {
        Vector2 targetVelocity = moveInput * moveSpeed;
        rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, targetVelocity, ref currentVelocityRef, movementSmoothing);
    }

    // -----------------------------------------------------
    //                    MOVEMENT INPUT
    // -----------------------------------------------------
    void ReadMovementInput()
    {
        float h = movementJoystick.Horizontal;
        float v = movementJoystick.Vertical;

        if (Mathf.Abs(h) < 0.01f && Mathf.Abs(v) < 0.01f)
        {
            // PC test inputu
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
        }

        moveInput = new Vector2(h, v);

        // deadzone
        if (moveInput.magnitude < 0.1f)
            moveInput = Vector2.zero;
        else
            moveInput = moveInput.normalized;
    }

    // -----------------------------------------------------
    //                     AIM INPUT
    // -----------------------------------------------------
    void ReadAimInput()
    {
        // Mouse veya dokunma başladı → aim mode ON
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
            inAimMode = true;

        // Parmak/mouse bırakıldı → aim mode OFF
        if (Input.GetMouseButtonUp(0))
            inAimMode = false;
    }

    // -----------------------------------------------------
    //                MOVEMENT ROTATION
    // -----------------------------------------------------
    void HandleMovementRotation()
    {
        if (moveInput == Vector2.zero) return;

        float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;

        Quaternion targetRot = Quaternion.Euler(0, 0, angle + rotationOffset);

        visuals.rotation = Quaternion.RotateTowards(
            visuals.rotation,
            targetRot,
            turnSpeed * Time.deltaTime
        );
    }

    // -----------------------------------------------------
    //                    AIM ROTATION
    // -----------------------------------------------------
    void HandleAimingRotation()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        Vector3 aimDir = mousePos - transform.position;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

        float clampedAngle = Mathf.Clamp(angle, minAimAngle, maxAimAngle);

        // Firepoint her zaman aim angle ile döner
        firePoint.rotation = Quaternion.Euler(0, 0, clampedAngle);

        // Aim sırasında visuals sadece firePoint’e göre offsetlenir
        visuals.rotation = Quaternion.Euler(0, 0, clampedAngle + rotationOffset);
    }

    // -----------------------------------------------------
    //                       BOBBING
    // -----------------------------------------------------
    void HandleBobbing(bool isMoving)
    {
        if (isMoving)
        {
            float bob = Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmount;
            visuals.localScale = new Vector3(1 + bob, 1 - bob, 1);
        }
        else
        {
            visuals.localScale = Vector3.Lerp(visuals.localScale, Vector3.one, Time.deltaTime * 10f);
        }
    }

    // -----------------------------------------------------
    //                 UI TOUCH CHECK
    // -----------------------------------------------------
    bool IsPointerOverUI()
    {
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            return EventSystem.current.IsPointerOverGameObject(t.fingerId);
        }
        return EventSystem.current.IsPointerOverGameObject();
    }
}
