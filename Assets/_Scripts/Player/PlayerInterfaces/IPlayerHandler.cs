public interface IPlayerHandler
{
    public bool IsActive { get; }
    public void Enable();
    public void Disable();
}
