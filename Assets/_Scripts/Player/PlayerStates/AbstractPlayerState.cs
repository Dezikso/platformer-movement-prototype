using UnityEngine;

public abstract class AbstractPlayerState
{
    protected PlayerController _playerController;

    public AbstractPlayerState(PlayerController controller)
    {
        this._playerController = controller;
    }

    public abstract void Enter();
    public abstract void Exit();
    public virtual void HandleMove(Vector2 direction) { }
    public virtual void HandleJump() { }
}
