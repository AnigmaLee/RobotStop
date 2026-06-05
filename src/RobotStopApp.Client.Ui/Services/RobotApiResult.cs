namespace RobotStopApp.Client.Ui.Services;

public sealed record RobotApiResult(
    bool IsSuccess,
    string Message,
    string? State = null,
    DateTimeOffset? Timestamp = null);
