using UnityEngine;
using UnityEngine.Assertions;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader _input;

    private IPlayerMovementHandler _movementHandler;
    private IPlayerSwingingHandler _swingingHandler;

    private bool _isSwinging = false;
    private bool _launched = false;

    private void Awake()
    {
        _movementHandler = GetComponent<IPlayerMovementHandler>();
        Assert.IsNotNull(_movementHandler);

        _swingingHandler = GetComponent<IPlayerSwingingHandler>();
        Assert.IsNotNull(_swingingHandler);
    }

    private void OnEnable()
    {
        _input.MoveEvent += OnMove;
        _input.JumpEvent += OnJump;
    }
    private void OnDisable()
    {
        _input.MoveEvent -= OnMove;
        _input.JumpEvent -= OnJump;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If the player has launched and is waiting for a collision, reenable movement and reset rotation.
        if (_launched)
        {
            _movementHandler.Enable();
            _launched = false;
            Vector3 currentEuler = transform.eulerAngles;
            transform.eulerAngles = new Vector3(0, currentEuler.y, 0);
        }
    }

    // Called by the SwingingPole class on collision
    public void StartSwinging(Transform swingTarget)
    {
        if (_isSwinging)
            return;

        _movementHandler.Disable();
        _swingingHandler.StartSwinging(swingTarget);
        _isSwinging = true;
    }

    private void OnMove(Vector2 direction)
    {
        if (!_isSwinging)
            _movementHandler.Move(direction);
    }

    private void OnJump()
    {
        if (_isSwinging)
        {
            _isSwinging = false;
            _swingingHandler.Launch();
            _launched = true;
        }
        else
            _movementHandler.Jump();
    }
}
