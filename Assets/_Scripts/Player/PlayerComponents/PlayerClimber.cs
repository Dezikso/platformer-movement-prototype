using UnityEngine;

public class PlayerClimber : MonoBehaviour, IPlayerClimbingHandler
{
    public bool IsActive => _isActive;

    [Header("Variables")]
    [SerializeField] private float _speed = 5;
    [SerializeField] private float _jumpForce = 5;
    [SerializeField] private float _alignmentSpeed = 10f;
    [SerializeField] private float _surfaceCheckDistance = 1f;
    [SerializeField] private float _surfaceOffset = 0.55f;

    private Rigidbody _rb;

    private bool _isActive = false;
    private Vector2 _moveInput;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (_isActive)
        {
            ApplyMovement();
            AlignWithSurface();
        }
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
        _moveInput = Vector2.zero;
    }

    public void Move(Vector2 direction) => _moveInput = direction;

    public void Jump()
    {
        if (_isActive)
            _rb.AddForce(-transform.forward * _jumpForce, ForceMode.VelocityChange);
    }

    private void ApplyMovement()
    {
        Vector3 horizontalMove = Vector3.zero;
        if (_moveInput != Vector2.zero)
        {
            horizontalMove = (transform.right * _moveInput.x + transform.up * _moveInput.y) * _speed;
        }
        Vector3 newVelocity = horizontalMove;
        _rb.linearVelocity = newVelocity;
    }

    private void AlignWithSurface()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, _surfaceCheckDistance))
        {
            Vector3 desiredForward = -hit.normal;
            transform.forward = Vector3.Lerp(transform.forward, desiredForward, Time.fixedDeltaTime * _alignmentSpeed);

            Vector3 desiredPosition = hit.point + hit.normal * _surfaceOffset;
            _rb.position = Vector3.Lerp(_rb.position, desiredPosition, Time.fixedDeltaTime * _alignmentSpeed);
        }
    }
}
