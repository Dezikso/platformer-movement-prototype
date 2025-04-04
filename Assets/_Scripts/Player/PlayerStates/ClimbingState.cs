using UnityEngine;

public class ClimbingState : AbstractPlayerState
{
    public ClimbingState(PlayerController controller) : base(controller) { }

    public override void Enter() => _playerController.ClimbingHandler.Enable();
    public override void Exit() => _playerController.ClimbingHandler.Disable();

    public override void HandleMove(Vector2 direction) => _playerController.ClimbingHandler.Move(direction);
    public override void HandleJump()
    {
        _playerController.ClimbingHandler.Jump();
        _playerController.ChangeState(new LaunchedState(_playerController));
    }
}
