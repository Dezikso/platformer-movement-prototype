using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerSwinger : MonoBehaviour, IPlayerSwingingHandler
{
    public bool IsActive => _isActive;

    [Header("Variables")]
    [SerializeField] private float _maxAngle = 45f;
    [SerializeField] private float _swingPeriod = 2f;
    [SerializeField] private float _ropeLength = 2f;
    [SerializeField] private float _launchForce = 10f;

    [Header("Animation")]
    [SerializeField] private float _layerFadeDuration = 0.2f;

    private int _swingingLayerIndex;

    private Rigidbody _rb;
    private Animator _animator;
    private Transform _swingTarget;
    private bool _isSwinging = false;
    private float _swingStartTime;

    private bool _isActive = false;
    private Vector3 _swingPlaneDirection;
    private Vector3 _currentTangent;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
        _swingingLayerIndex = _animator.GetLayerIndex("SwingingLayer");
    }

    private void FixedUpdate()
    {
        if (!_isSwinging || _swingTarget == null)
            return;

        float elapsed = Time.time - _swingStartTime;
        float omega = 2 * Mathf.PI / _swingPeriod;
        float theta = (_maxAngle * Mathf.Deg2Rad) * Mathf.Sin(omega * elapsed);

        UpdatePosition(theta);
        UpdateRotation(theta);
    }

    public void Enable() => _isActive = true;

    public void Disable()
    {
        _isActive = false;
        if (_isSwinging)
            EndSwinging();
    }

    public void StartSwinging(Transform swingTarget)
    {
        if (!_isActive)
            return;

        StartCoroutine(FadeAnimationLayer(_swingingLayerIndex, 1));

        _swingTarget = swingTarget;
        _isSwinging = true;
        _swingStartTime = Time.time;

        Vector3 incomingDirection = transform.position - swingTarget.position;
        incomingDirection.y = 0;

        if (incomingDirection == Vector3.zero)
            _swingPlaneDirection = -swingTarget.forward;
        else
        {
            float dot = Vector3.Dot(incomingDirection.normalized, swingTarget.forward);
            _swingPlaneDirection = dot >= 0 ? -swingTarget.forward : swingTarget.forward;
        }

        _rb.isKinematic = true;
    }

    public void Launch()
    {
        if (!_isActive || !_isSwinging)
            return;

        EndSwinging();
        _rb.AddForce(_currentTangent * _launchForce, ForceMode.VelocityChange);
    }

    private void EndSwinging()
    {
        _isSwinging = false;
        _rb.isKinematic = false;

        StartCoroutine(FadeAnimationLayer(_swingingLayerIndex, 0));
    }

    private void UpdatePosition(float theta)
    {
        Vector3 offset = _swingPlaneDirection * (_ropeLength * Mathf.Sin(theta))
                         - Vector3.up * (_ropeLength * Mathf.Cos(theta));
        transform.position = _swingTarget.position + offset;
    }

    private void UpdateRotation(float theta)
    {
        Vector3 derivative = _ropeLength * Mathf.Cos(theta) * _swingPlaneDirection
                             + _ropeLength * Mathf.Sin(theta) * Vector3.up;
        _currentTangent = derivative.normalized;
        transform.rotation = Quaternion.LookRotation(_currentTangent, Vector3.up);
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
