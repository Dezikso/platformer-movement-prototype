using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerSwinger : MonoBehaviour, IPlayerSwingingHandler
{
    private Rigidbody _rb;
    private Transform _swingTarget;
    private bool _isSwinging = false;
    private float _swingStartTime;

    [SerializeField] private float _maxAngle = 45f;
    [SerializeField] private float _swingPeriod = 2f;
    [SerializeField] private float _ropeLength = 2f;
    [SerializeField] private float _launchForce = 10f;

    private Vector3 _swingPlaneDirection;
    private Vector3 _currentTangent;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (_isSwinging && _swingTarget != null)
        {
            float elapsed = Time.time - _swingStartTime;
            float omega = 2 * Mathf.PI / _swingPeriod;
            float theta = (_maxAngle * Mathf.Deg2Rad) * Mathf.Sin(omega * elapsed);

            Vector3 offset = _swingPlaneDirection * (_ropeLength * Mathf.Sin(theta))
                             - Vector3.up * (_ropeLength * Mathf.Cos(theta));

            transform.position = _swingTarget.position + offset;

            float dThetaDt = (_maxAngle * Mathf.Deg2Rad) * Mathf.Cos(omega * elapsed) * omega;
            float signFactor = Mathf.Sign(dThetaDt);

            Vector3 derivative = _ropeLength * Mathf.Cos(theta) * _swingPlaneDirection
                                 + _ropeLength * Mathf.Sin(theta) * Vector3.up;
            _currentTangent = (signFactor * derivative).normalized;

            transform.rotation = Quaternion.LookRotation(_currentTangent, Vector3.up);
        }
    }

    public void StartSwinging(Transform swingTarget)
    {
        _swingTarget = swingTarget;
        _isSwinging = true;
        _swingStartTime = Time.time;

        Vector3 incomingDirection = transform.position - swingTarget.position;
        incomingDirection.y = 0;
        if (incomingDirection == Vector3.zero)
        {
            _swingPlaneDirection = -swingTarget.forward;
        }
        else
        {
            float dot = Vector3.Dot(incomingDirection.normalized, swingTarget.forward);
            _swingPlaneDirection = dot >= 0 ? -swingTarget.forward : swingTarget.forward;
        }

        _rb.isKinematic = true;
    }

    public void Launch()
    {
        _isSwinging = false;
        _rb.isKinematic = false;

        _rb.AddForce(_currentTangent * _launchForce, ForceMode.VelocityChange);
    }
}
