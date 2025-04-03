using UnityEngine;

public class PlayerClimber : MonoBehaviour, IPlayerClimbingHandler
{
    public bool IsActive => _isActive;

    [Header("Variables")]
    [SerializeField] private float _speed = 5;
    [SerializeField] private float _jumpForce = 5;

    private Rigidbody _rb;

    private bool _isActive = true;
    private Vector2 _moveInput;
    private Transform _climbingTarget;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (_isActive && _climbingTarget != null)
            ApplyMovement();
    }

    public void Enable()
    {
        _rb.useGravity = false;
        _isActive = true;
    }

    public void Disable()
    {
        _rb.useGravity = true;
        _isActive = false;
        _climbingTarget = null;
        _moveInput = Vector2.zero;
    }

    public void SetClimbingTarget(Transform target) => _climbingTarget = target;

    public void Move(Vector2 direction) => _moveInput = direction;

    public void Jump()
    {
        Debug.Log("Jump");
    }

    private void ApplyMovement()
    {
        Vector3 horizontalMove = Vector3.zero;
        if (_moveInput != Vector2.zero)
        {
            Vector3 up = _climbingTarget.up;
            up.z = 0;
            up.Normalize();

            Vector3 right = _climbingTarget.right;
            right.z = 0;
            right.Normalize();

            horizontalMove = (right * _moveInput.x + up * _moveInput.y) * _speed;
        }

        Vector3 newVelocity = horizontalMove;
        newVelocity.z = 0;
        _rb.linearVelocity = newVelocity;
    }
}
