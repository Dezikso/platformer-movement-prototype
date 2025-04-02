using UnityEngine;

public class SwingingState : AbstractPlayerState
{
    private readonly Transform  _swingTarget;

    public SwingingState(PlayerController controller, Transform swingTarget) : base(controller) 
    {
        _swingTarget = swingTarget;
    }

    public override void Enter()
    {
        _playerController.SwingingHandler.Enable();
        _playerController.SwingingHandler.StartSwinging(_swingTarget);
    }

    public override void Exit()
    {
        _playerController.SwingingHandler.Launch();
        _playerController.SwingingHandler.Disable();
    }

    public override void HandleJump() => _playerController.ChangeState(new LaunchedState(_playerController));
}
