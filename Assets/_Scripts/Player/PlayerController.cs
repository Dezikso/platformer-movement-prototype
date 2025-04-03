using UnityEngine;
using UnityEngine.Assertions;

public class PlayerController : MonoBehaviour
{
    public IPlayerMovementHandler MovementHandler => _movementHandler;
    public IPlayerSwingingHandler SwingingHandler => _swingingHandler;
    public IPlayerClimbingHandler ClimbingHandler => _climbingHandler;

    [Header("References")]
    [SerializeField] private InputReader _input;

    private IPlayerMovementHandler _movementHandler;
    private IPlayerSwingingHandler _swingingHandler;
    private IPlayerClimbingHandler _climbingHandler;

    private AbstractPlayerState _currentState;

    private void Awake()
    {
        _movementHandler = GetComponent<IPlayerMovementHandler>();
        Assert.IsNotNull(_movementHandler);

        _swingingHandler = GetComponent<IPlayerSwingingHandler>();
        Assert.IsNotNull(_swingingHandler);

        _climbingHandler = GetComponent<IPlayerClimbingHandler>();
        Assert.IsNotNull(_climbingHandler);

        _currentState = new WalkingState(this);
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
        if (other.CompareTag("SwingPole") && _currentState is not SwingingState)
            ChangeState(new SwingingState(this, other.transform));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_currentState is LaunchedState)
        {
            if (collision.gameObject.CompareTag("ClimbingSurface"))
                ChangeState(new ClimbingState(this, collision.transform));

            ResetRotation();
        }
        if (_currentState is WalkingState)
        {
            if (collision.gameObject.CompareTag("ClimbingSurface"))
                ChangeState(new ClimbingState(this, collision.transform));
        }
    }

    public void ChangeState(AbstractPlayerState newState)
    {
        _currentState.Exit();
        _currentState = newState;
        _currentState.Enter();
    }

    private void HandleMove(Vector2 direction) => _currentState.HandleMove(direction);

    private void HandleJump() => _currentState.HandleJump();

    private void ResetRotation()
    {
        Vector3 currentEuler = transform.eulerAngles;
        transform.eulerAngles = new Vector3(0, currentEuler.y, 0);
    }
}
