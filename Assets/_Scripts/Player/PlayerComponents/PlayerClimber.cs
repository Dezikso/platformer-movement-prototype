using System.Collections;
using UnityEngine;

public class PlayerClimber : MonoBehaviour, IPlayerClimbingHandler
{
    public bool IsActive => _isActive;

    [Header("Variables")]
    [SerializeField] private LayerMask _climbingLayer;
    [SerializeField] private float _speed = 5;
    [SerializeField] private float _jumpForce = 5;
    [SerializeField] private float _alignmentSpeed = 10f;
    [SerializeField] private float _surfaceCheckDistance = 1f;
    [SerializeField] private float _surfaceDistance = 0.55f;
    [SerializeField] private float _movementCheckOffset = 0.2f;

    [Header("Animation")]
    [SerializeField] private float _animationAcceleration = 1f;
    [SerializeField] private float _layerFadeDuration = 0.2f;

    private static readonly int VelocityXHash = Animator.StringToHash("VelocityX");
    private static readonly int VelocityZHash = Animator.StringToHash("VelocityZ");
    private int _climbingLayerIndex;

    private Rigidbody _rb;
    private Animator _animator;

    private bool _isActive = false;
    private Vector2 _moveInput;
    private Vector2 _animationVelocity;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
        _climbingLayerIndex = _animator.GetLayerIndex("ClimbingLayer");
    }

    private void FixedUpdate()
    {
        if (!_isActive)
            return;

        if (IsDirectionClimbable())
        {
            ApplyMovement();
            ApplyAnimation();
        }
        else
            _moveInput = Vector2.zero;

        AlignWithSurface();
    }

    public void Enable()
    {
        _rb.useGravity = false;
        _isActive = true;
        StartCoroutine(FadeAnimationLayer(_climbingLayerIndex, 1f));
    }

    public void Disable()
    {
        _rb.useGravity = true;
        _isActive = false;
        _moveInput = Vector2.zero;
        _animationVelocity = Vector2.zero;
        StartCoroutine(FadeAnimationLayer(_climbingLayerIndex, 0f));
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
            horizontalMove = (transform.right * _moveInput.x + transform.up * _moveInput.y) * _speed;

        _rb.linearVelocity = horizontalMove;
    }

    private void AlignWithSurface()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, _surfaceCheckDistance))
        {
            Vector3 desiredForward = -hit.normal;
            transform.forward = Vector3.Lerp(transform.forward, desiredForward, Time.fixedDeltaTime * _alignmentSpeed);

            Vector3 desiredPosition = hit.point + hit.normal * _surfaceDistance;
            _rb.position = Vector3.Lerp(_rb.position, desiredPosition, Time.fixedDeltaTime * _alignmentSpeed);
        }
    }

    private bool IsDirectionClimbable()
    {
        if (_moveInput == Vector2.zero)
            return true;

        Vector3 offset = (transform.right * Mathf.Round(_moveInput.x) + transform.up * Mathf.Round(_moveInput.y)) * _movementCheckOffset;
        Vector3 rayOrigin = transform.position + offset - transform.forward;

        Debug.DrawRay(rayOrigin, transform.forward * _surfaceCheckDistance, Color.red);

        return Physics.Raycast(rayOrigin, transform.forward, _surfaceCheckDistance, _climbingLayer);
    }

    private void ApplyAnimation()
    {
        _animationVelocity = Vector2.MoveTowards(_animationVelocity, _moveInput, _animationAcceleration * Time.fixedDeltaTime);

        _animator.SetFloat(VelocityXHash, _animationVelocity.x);
        _animator.SetFloat(VelocityZHash, _animationVelocity.y);
    }

    private IEnumerator FadeAnimationLayer(int layerIndex, float targetWeight)
    {
        float startWeight = _animator.GetLayerWeight(layerIndex);

        for (float t = 0f; t < 1f; t += Time.deltaTime / _layerFadeDuration)
        {
            float weight = Mathf.Lerp(startWeight, targetWeight, t);
            _animator.SetLayerWeight(layerIndex, weight);
            yield return null;
        }

        _animator.SetLayerWeight(layerIndex, targetWeight);
    }
}
