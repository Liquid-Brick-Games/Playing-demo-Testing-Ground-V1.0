using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class P_Movement : MonoBehaviour {

    [Header("Speed")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 9f;

    [Header("Force")]
    [SerializeField] private float jumpForce = 6f;

    [Header("Rotation")]
    [SerializeField] private float rotationAngle = 720f;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

    [Header("Spawn")]
    [SerializeField] private Transform startPoint;

    [Header("Ground check")]
    [SerializeField] private Transform groundPoint;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Air control")]
    [SerializeField] float airControlModifier = 0.2f;

    /*---- Переменные ----*/
    private Rigidbody rigidbody;
    private InputActions inputActions;
    private Vector2 playerMovement;

    /*---- Статусы и состояния ----*/
    private bool IsSprinting = false;
    private bool IsGrounded;


    /*---- Методы жизненного цикла ----*/
    private void Awake() {
        TeleportToStar();

        inputActions = new InputActions();
        rigidbody = GetComponent<Rigidbody>();


        if (!cameraTransform && Camera.main != null) cameraTransform = Camera.main.transform;
    }

    private void OnEnable() {
        GameManager.OnGameStateChanged += OnHandleStateChanged;
        inputActions.Player.Jump.performed += OnJumpPerformed;
    }

    private void OnDisable() {
        inputActions.Player.Jump.performed -= OnJumpPerformed;
        GameManager.OnGameStateChanged -= OnHandleStateChanged;

        inputActions.Player.Disable();
    }

    private void Update() {
        playerMovement = inputActions.Player.Move.ReadValue<Vector2>();
        IsSprinting = inputActions.Player.Sprint.ReadValue<float>() > 0.1f;

        if (groundPoint != null) {
            IsGrounded = Physics.CheckSphere(groundPoint.position, groundCheckRadius, groundLayer);
        }
        else { IsGrounded = true; }
    }

    private void FixedUpdate() {
        float speedModifier = IsSprinting ? sprintSpeed : moveSpeed;
        if (!IsGrounded) speedModifier *= airControlModifier;

        Vector3 movementToVector = new Vector3(playerMovement.x, 0f, playerMovement.y);

        if (playerMovement != Vector2.zero && cameraTransform != null) {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;

            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            movementToVector = camForward * playerMovement.y + camRight * playerMovement.x;
        }

        if (movementToVector != Vector3.zero) {
            Quaternion convertToValue = Quaternion.LookRotation(movementToVector);
            Quaternion calculateAngle = Quaternion.RotateTowards(rigidbody.rotation, convertToValue, rotationAngle * Time.deltaTime);

            rigidbody.MoveRotation(calculateAngle);
        }

        if (IsGrounded) {
            rigidbody.linearVelocity = new Vector3(movementToVector.x * speedModifier, rigidbody.linearVelocity.y, movementToVector.z * speedModifier);
        }
        else {
            Vector3 targetAirVelocity = new Vector3(movementToVector.x * speedModifier, rigidbody.linearVelocity.y, movementToVector.z * speedModifier);
            rigidbody.linearVelocity = Vector3.Lerp(rigidbody.linearVelocity, targetAirVelocity, Time.fixedDeltaTime * 5f);
        }

    }

    /*---- Колбек методы событий  ----*/
    private void OnHandleStateChanged(GameManager.GameState gameState) {
        if (gameState == GameManager.GameState.Playing) { inputActions.Player.Enable(); }
        else {
            inputActions.Player.Disable();
            if (rigidbody != null) {
                rigidbody.linearVelocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
            }
        }
    }
    private void OnJumpPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context) {
        if (!IsGrounded) return;

        rigidbody.linearVelocity = new Vector3(rigidbody.linearVelocity.x, 0f, rigidbody.linearVelocity.z);
        rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    /*---- Прочие привтные методы  ----*/
    private void OnDrawGizmosSelected() {
        if (groundPoint != null) {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundPoint.position, groundCheckRadius);
        }
    }

    /*---- Публичные методы ----*/
    public void TeleportToStar() {
        bool isStarPoint = startPoint != null && rigidbody != null;
        if (!isStarPoint) return;

        rigidbody.linearVelocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;

        rigidbody.position = startPoint.position;
        rigidbody.rotation = startPoint.rotation;
    }
}
