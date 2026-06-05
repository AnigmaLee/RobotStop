using Microsoft.Extensions.Logging.Abstractions;
using RobotStopApp.Api.Robot;
using RobotStopApp.Service.Robot;
using Xunit;

namespace RobotStopApp.Api.Tests;

public class StubRobotControllerTests
{
    private static StubRobotController NewController() =>
        new(NullLogger<StubRobotController>.Instance);

    [Fact]
    public void Initial_state_is_idle()
    {
        using var sut = NewController();
        Assert.Equal(RobotState.Idle, sut.GetStatus());
    }

    [Fact]
    public async Task Run_from_idle_transitions_to_running()
    {
        using var sut = NewController();
        var state = await sut.RunAsync();
        Assert.Equal(RobotState.Running, state);
        Assert.Equal(RobotState.Running, sut.GetStatus());
    }

    [Fact]
    public async Task Stop_from_running_transitions_to_stopped()
    {
        using var sut = NewController();
        await sut.RunAsync();
        var state = await sut.StopAsync();
        Assert.Equal(RobotState.Stopped, state);
    }

    [Fact]
    public async Task Stop_when_already_stopped_is_idempotent()
    {
        using var sut = NewController();
        await sut.RunAsync();
        await sut.StopAsync();
        var state = await sut.StopAsync();
        Assert.Equal(RobotState.Stopped, state);
    }

    [Fact]
    public async Task Run_when_already_running_throws()
    {
        using var sut = NewController();
        await sut.RunAsync();
        await Assert.ThrowsAsync<InvalidRobotTransitionException>(() => sut.RunAsync());
    }

    [Fact]
    public async Task Run_after_stop_transitions_to_running()
    {
        using var sut = NewController();
        await sut.RunAsync();
        await sut.StopAsync();
        var state = await sut.RunAsync();
        Assert.Equal(RobotState.Running, state);
    }
}
