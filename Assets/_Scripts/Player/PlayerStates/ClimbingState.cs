using UnityEngine;

public class ClimbingState : AbstractPlayerState
{
    private readonly Transform _climbingTarget;

    public ClimbingState(PlayerController controller, Transform climbingTarget) : base(controller)
    {
        _climbingTarget = climbingTarget;
    }

    public override void Enter()
    {
        _playerController.ClimbingHandler.SetClimbingTarget(_climbingTarget);
        _playerController.ClimbingHandler.Enable();
    }
    public override void Exit() => _playerController.ClimbingHandler.Disable();

    public override void HandleMove(Vector2 direction) => _playerController.ClimbingHandler.Move(direction);
    public override void HandleJump() => _playerController.ClimbingHandler.Jump();
}
