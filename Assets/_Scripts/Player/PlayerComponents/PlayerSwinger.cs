using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerSwinger : MonoBehaviour, IPlayerSwingingHandler
{
    public bool IsActive => _isActive;

    private Rigidbody _rb;
    private Transform _swingTarget;
    private bool _isSwinging = false;
    private float _swingStartTime;

    [SerializeField] private float _maxAngle = 45f;
    [SerializeField] private float _swingPeriod = 2f;
    [SerializeField] private float _ropeLength = 2f;
    [SerializeField] private float _launchForce = 10f;

    private bool _isActive = false;
    private Vector3 _swingPlaneDirection;
    private Vector3 _currentTangent;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
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
}
