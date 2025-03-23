using UnityEngine;

public interface IPlayerMovementHandler
{
    public void Enable();
    public void Disable();
    public void Move(Vector2 direction);
    public void Jump();
}