using UnityEngine;

public interface IPlayerClimbingHandler : IPlayerHandler
{
    public void Move(Vector2 direction);
    public void Jump();
}
