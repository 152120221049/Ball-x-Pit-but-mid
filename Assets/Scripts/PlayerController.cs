using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [Header("Ayarlar")]
    public float moveSpeed = 5f;
    public float turnSpeed = 15f;

    public float rotationOffset = 0f; 
    [Header("Nişan Sınırları")]
    public float minAimAngle = 20f;  
    public float maxAimAngle = 160f; 

    [Header("Referanslar")]
    public Transform firePoint;
    public Transform visuals;
    public Joystick movementJoystick;
    public Animator animator;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isAiming = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        float horizontal = movementJoystick.Horizontal;
        float vertical = movementJoystick.Vertical;

        if (horizontal == 0 && vertical == 0)
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
        }

        moveInput = new Vector2(horizontal, vertical);

        if (animator != null)
        {
            bool isMoving = moveInput.magnitude > 0.1f;
            animator.SetBool("isWalking", isMoving);
        }

        isAiming = Input.GetMouseButton(0) && !IsPointerOverUI();

        if (isAiming)
        {
            HandleAimingRotation();
        }
        else if (moveInput.magnitude > 0.1f)
        {
            HandleMovementRotation();
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    void HandleMovementRotation()
    {
   
        if (moveInput.magnitude < 0.15f) return;

        Vector3 direction = new Vector3(moveInput.x, moveInput.y, 0);

        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);

        if (rotationOffset != 0)
        {
            targetRotation *= Quaternion.Euler(0, 0, rotationOffset);
        }

        visuals.rotation = Quaternion.RotateTowards(visuals.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }

    void HandleAimingRotation()
    {
        Vector3 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        touchPos.z = 0;

        Vector3 aimDirection = touchPos - transform.position;
       
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        if (aimDirection.y < 0)
        {
            if (aimDirection.x > 0) angle = minAimAngle;
            else angle = maxAimAngle; 
        }

       
        float clampedAngle = Mathf.Clamp(angle, minAimAngle, maxAimAngle);

      
        firePoint.rotation = Quaternion.Euler(0, 0, clampedAngle);

      
        visuals.rotation = Quaternion.Euler(0, 0, clampedAngle + rotationOffset);
    }

    bool IsPointerOverUI()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                return EventSystem.current.IsPointerOverGameObject(touch.fingerId);
        }
        return EventSystem.current.IsPointerOverGameObject();
    }
}