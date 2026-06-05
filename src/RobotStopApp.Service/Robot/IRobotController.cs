namespace RobotStopApp.Service.Robot;

public interface IRobotController
{
    Task<RobotState> StopAsync(CancellationToken cancellationToken = default);
    Task<RobotState> RunAsync(CancellationToken cancellationToken = default);
    RobotState GetStatus();
}

public class InvalidRobotTransitionException : Exception
{
    public RobotState CurrentState { get; }
    public string AttemptedAction { get; }

    public InvalidRobotTransitionException(RobotState current, string action)
        : base($"Cannot {action} robot while in state '{current}'.")
    {
        CurrentState = current;
        AttemptedAction = action;
    }
}