using UnityEngine;

public interface IPlayerMovementHandler : IPlayerHandler
{
    public void Move(Vector2 direction);
    public void Jump();
}