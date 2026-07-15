namespace Movement
{
    public interface IMovementProvider
    {
        MovementCommand GetMovementCommand();
    }
}