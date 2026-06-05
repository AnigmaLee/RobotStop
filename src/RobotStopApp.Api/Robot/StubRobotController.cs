using Microsoft.Extensions.Logging;
using RobotStopApp.Service.Robot;

namespace RobotStopApp.Api.Robot;

public sealed class StubRobotController : IRobotController, IDisposable
{
    private readonly ILogger<StubRobotController> _logger;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private RobotState _state = RobotState.Idle;

    public StubRobotController(ILogger<StubRobotController> logger)
    {
        _logger = logger;
    }

    public RobotState GetStatus() => _state;

    public async Task<RobotState> RunAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_state == RobotState.Running)
            {
                throw new InvalidRobotTransitionException(_state, "run");
            }

            _logger.LogInformation("Robot transitioning from {From} to Running", _state);
            await Task.Delay(50, cancellationToken).ConfigureAwait(false);
            _state = RobotState.Running;
            _logger.LogInformation("Robot is now Running");
            return _state;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<RobotState> StopAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            // Stop is idempotent: stopping an already-stopped robot is a no-op success.
            if (_state == RobotState.Stopped)
            {
                _logger.LogInformation("Stop requested while already Stopped (no-op)");
                return _state;
            }

            _logger.LogInformation("Robot transitioning from {From} to Stopped", _state);
            await Task.Delay(50, cancellationToken).ConfigureAwait(false);
            _state = RobotState.Stopped;
            _logger.LogInformation("Robot is now Stopped");
            return _state;
        }
        finally
        {
            _gate.Release();
        }
    }

    public void Dispose() => _gate.Dispose();
}
