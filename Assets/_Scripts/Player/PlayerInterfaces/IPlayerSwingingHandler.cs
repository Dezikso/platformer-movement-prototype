using UnityEngine;

public interface IPlayerSwingingHandler : IPlayerHandler
{
    public void StartSwinging(Transform pivot);
    public void Launch();
}
