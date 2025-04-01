using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.WSA;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader _input;

    private IPlayerMovementHandler _movementHandler;
    private IPlayerSwingingHandler _swingingHandler;

    private PlayerState _currentState = PlayerState.Walking;

    private void Awake()
    {
        _movementHandler = GetComponent<IPlayerMovementHandler>();
        Assert.IsNotNull(_movementHandler);

        _swingingHandler = GetComponent<IPlayerSwingingHandler>();
        Assert.IsNotNull(_swingingHandler);
    }

    private void OnEnable()
    {
        _input.MoveEvent += HandleMove;
        _input.JumpEvent += HandleJump;
    }
    private void OnDisable()
    {
        _input.MoveEvent -= HandleMove;
        _input.JumpEvent -= HandleJump;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SwingPole"))
        {
            if (_currentState != PlayerState.Swinging)
            {
                StartSwinging(other.transform);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If the player has launched and is waiting for a collision, reenable movement and reset rotation.
        if (_currentState == PlayerState.Launched)
        {
            _movementHandler.Enable();
            _currentState = PlayerState.Walking;
            ResetRotation();
        }
    }

    private void HandleMove(Vector2 direction)
    {
        if (_currentState == PlayerState.Walking)
            _movementHandler.Move(direction);
    }

    private void HandleJump()
    {
        if (_currentState == PlayerState.Walking)
        {
            _movementHandler.Jump();
        }
        else if (_currentState == PlayerState.Swinging)
        {
            _currentState = PlayerState.Launched;
            _swingingHandler.Launch();
        }
    }

    private void StartSwinging(Transform swingTarget)
    {
        _movementHandler.Disable();
        _swingingHandler.StartSwinging(swingTarget);
        _currentState = PlayerState.Swinging;
    }

    private void ResetRotation()
    {
        Vector3 currentEuler = transform.eulerAngles;
        transform.eulerAngles = new Vector3(0, currentEuler.y, 0);
    }

}
