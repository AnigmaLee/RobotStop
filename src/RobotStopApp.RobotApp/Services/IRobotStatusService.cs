namespace RobotStopApp.RobotApp.Services;

public interface IRobotStatusService
{
    Task<RobotStatusResult> CheckAsync(string baseUrl, string apiKey, CancellationToken cancellationToken = default);
}
