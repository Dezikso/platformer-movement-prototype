using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class PlayerMover : MonoBehaviour, IPlayerMovementHandler
{
    public bool IsActive => _isActive;

    [Header("Variables")]
    [SerializeField] private float _speed = 5;
    [SerializeField] private float _jumpForce = 5;
    [SerializeField] private float _rotationSpeed = 10f;

    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private static readonly int JumpTriggerHash = Animator.StringToHash("JumpTrigger");

    private Rigidbody _rb;
    private Transform _cameraTransform;
    private Animator _animator;

    private bool _isActive = false;
    private Vector2 _moveInput;

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        if (!_isActive)
            return;

        ApplyMovement();
        ApplyAnimation();
    }

    public void Move(Vector2 direction) => _moveInput = direction;

    public void Enable() => _isActive = true;

    public void Disable()
    {
        _isActive = false;
        _moveInput = Vector2.zero;
        _animator.SetBool(IsMovingHash, false);
    }

    public void Jump()
    {
        if (!IsGrounded())
            return;

        _rb.AddForce(Vector3.up * _jumpForce, ForceMode.VelocityChange);
        _animator.SetTrigger(JumpTriggerHash);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    private void ApplyMovement()
    {
        Vector3 horizontalMove = Vector3.zero;
        if (_moveInput != Vector2.zero)
        {
            Vector3 forward = _cameraTransform.forward;
            forward.y = 0;
            forward.Normalize();

            Vector3 right = _cameraTransform.right;
            right.y = 0;
            right.Normalize();

            horizontalMove = (forward * _moveInput.y + right * _moveInput.x) * _speed;

            Quaternion targetRotation = Quaternion.LookRotation(horizontalMove);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.fixedDeltaTime * _rotationSpeed);
        }

        Vector3 newVelocity = horizontalMove;
        newVelocity.y = _rb.linearVelocity.y;
        _rb.linearVelocity = newVelocity;
    }
    private void ApplyAnimation()
    {
        bool moving = _moveInput.sqrMagnitude > 0.01f;
        _animator.SetBool(IsMovingHash, moving);
    }
}
