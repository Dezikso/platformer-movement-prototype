using UnityEngine;

public class WalkingState : AbstractPlayerState
{
    public WalkingState(PlayerController controller) : base(controller) { }

    public override void Enter() => _playerController.MovementHandler.Enable();
    public override void Exit() => _playerController.MovementHandler.Disable();

    public override void HandleMove(Vector2 direction) => _playerController.MovementHandler.Move(direction);
    public override void HandleJump() => _playerController.MovementHandler.Jump();
}
