namespace RobotStopApp.Client.Ui.Services;

public interface IRobotApiClient
{
    Task<RobotApiResult> RunAsync(string baseUrl, string apiKey, CancellationToken cancellationToken = default);
    Task<RobotApiResult> StopAsync(string baseUrl, string apiKey, CancellationToken cancellationToken = default);
    Task<RobotApiResult> StatusAsync(string baseUrl, string apiKey, CancellationToken cancellationToken = default);
}
