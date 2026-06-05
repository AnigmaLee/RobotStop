using RobotStopApp.Service.Robot;

namespace RobotStopApp.Service.Models;

public record RobotStateResponse(RobotState State, DateTimeOffset Timestamp)
{
    public static RobotStateResponse From(RobotState state) => new(state, DateTimeOffset.UtcNow);
}

public record ErrorResponse(string Error, string? Detail = null);