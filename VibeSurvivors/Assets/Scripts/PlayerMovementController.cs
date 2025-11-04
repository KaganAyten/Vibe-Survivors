using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float fallGravityMultiplier = 2f; // Düşerken yer çekimi çarpanı
    [SerializeField] private float coyoteTime = 0.1f; // Yere değdikten sonra zıplama toleransı

    [Header("Bunny Hop Settings")]
    [SerializeField] private float momentumGainPerJump = 0.5f; // Her zıplamada kazanılan hız
    [SerializeField] private float maxMomentumSpeed = 12f; // Maksimum momentum hızı
    [SerializeField] private float momentumDecayRate = 5f; // Durduğunda momentum azalma hızı

    [Header("Input Actions")]
    [SerializeField] private InputActionAsset playerActions;

    private CharacterController characterController;
    private PlayerAnimatorController animatorController;
    private InputAction moveAction;
    private InputAction jumpAction;

    private Vector3 velocity;
    private Vector2 moveInput;
    private bool isGrounded;
    private bool jumpButtonHeld = false; // Space tuşu basılı mı?
    private float coyoteTimeCounter; // Yere değme zamanı sayacı
    
    private float currentMomentum = 0f; // Şu anki momentum
    private int consecutiveJumps = 0; // Ard arda zıplama sayısı
    private bool wasGroundedLastFrame = false; // Önceki frame'de yerde miydi?

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animatorController = GetComponent<PlayerAnimatorController>();

        // Input Actions'ı ayarla
        if (playerActions != null)
        {
            var actionMap = playerActions.FindActionMap("PlayerMap");
            if (actionMap != null)
            {
                moveAction = actionMap.FindAction("Movement");
                jumpAction = actionMap.FindAction("Jump");
            }
        }
    }

    private void OnEnable()
    {
        if (moveAction != null)
        {
            moveAction.Enable();
            moveAction.performed += OnMove;
            moveAction.canceled += OnMove;
        }

        if (jumpAction != null)
        {
            jumpAction.Enable();
            jumpAction.performed += OnJumpPressed;
            jumpAction.canceled += OnJumpReleased;
        }
    }

    private void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.performed -= OnMove;
            moveAction.canceled -= OnMove;
            moveAction.Disable();
        }

        if (jumpAction != null)
        {
            jumpAction.performed -= OnJumpPressed;
            jumpAction.canceled -= OnJumpReleased;
            jumpAction.Disable();
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnJumpPressed(InputAction.CallbackContext context)
    {
        jumpButtonHeld = true;
    }

    private void OnJumpReleased(InputAction.CallbackContext context)
    {
        jumpButtonHeld = false;
    }

    private void PerformJump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // Momentum kazanımı - Her zıplamada hız artar
        consecutiveJumps++;
        currentMomentum = Mathf.Min(currentMomentum + momentumGainPerJump, maxMomentumSpeed - moveSpeed);

        // Zıplama animasyonunu tetikle
        if (animatorController != null)
        {
            animatorController.TriggerJump();
        }
    }

    private void Update()
    {
        // Yer kontrolü
        isGrounded = characterController.isGrounded;

        // Yere yeni değdiyse momentum zincirini kontrol et
        if (isGrounded && !wasGroundedLastFrame)
        {
            // Eğer jump butonu basılı değilse momentum sıfırlanır
            if (!jumpButtonHeld)
            {
                consecutiveJumps = 0;
                currentMomentum = 0f; // Momentum da sıfırla
            }
        }

        wasGroundedLastFrame = isGrounded;

        // Coyote time - Yere değdikten kısa bir süre sonra zıplama hakkı
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Yerde sabit kalması için küçük bir negatif değer
        }

        // Bunny Hop - Space basılıyken ve yerdeyken otomatik zıpla
        if (jumpButtonHeld && coyoteTimeCounter > 0f)
        {
            PerformJump();
            coyoteTimeCounter = 0f; // Coyote time'ı sıfırla
        }

        // Hareket
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);

        // Kamera yönüne göre hareket (TPS için)
        move = Camera.main.transform.TransformDirection(move);
        move.y = 0f; // Y eksenini sıfırla

        // Momentum sistemini uygula
        bool isMoving = moveInput.magnitude > 0.1f;
        float currentSpeed = moveSpeed;

        if (isMoving && consecutiveJumps > 0)
        {
            // Momentum varsa hıza ekle
            currentSpeed += currentMomentum;
        }
        else if (!isMoving)
        {
            // Hareket etmiyorsa momentumu azalt ve sıfırla
            currentMomentum = Mathf.Max(0f, currentMomentum - momentumDecayRate * Time.deltaTime);
            if (currentMomentum <= 0.01f)
            {
                currentMomentum = 0f;
                consecutiveJumps = 0;
            }
        }

        // Yerdeyken ve hareket etmiyorken momentum sıfırla (duruş)
        if (isGrounded && !isMoving && !jumpButtonHeld)
        {
            currentMomentum = 0f;
            consecutiveJumps = 0;
        }

        characterController.Move(move * currentSpeed * Time.deltaTime);

        // Animasyon kontrolü - Hareket var mı?
        if (animatorController != null)
        {
            animatorController.SetRunning(isMoving);
        }

        // Karakter rotasyonu (hareket yönüne doğru)
        if (move != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Yer çekimi - Düşerken daha hızlı
        float appliedGravity = gravity;
        
        // Eğer aşağı düşüyorsa (velocity.y negatifse) yer çekimini artır
        if (velocity.y < 0)
        {
            appliedGravity *= fallGravityMultiplier;
        }
        
        velocity.y += appliedGravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
}
