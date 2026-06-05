using RobotStopApp.RobotApp.Services;
using RobotStopApp.RobotApp.ViewModels;

namespace RobotStopApp.RobotApp.Tests;

public class MainWindowViewModelTests
{
    [Fact]
    public async Task RefreshAsync_sets_indicators_true_for_success()
    {
        var fake = new FakeStatusService
        {
            NextResult = new RobotStatusResult(true, true, "OK")
        };
        var vm = NewViewModel(fake);

        await vm.RefreshAsync();

        Assert.True(vm.ApiConnected);
        Assert.True(vm.IsRobotRunOk);
        Assert.Equal("Connected", vm.ApiConnectedText);
        Assert.Equal("True", vm.IsRobotRunOkText);
    }

    [Fact]
    public async Task RefreshAsync_sets_run_false_for_unauthorized()
    {
        var fake = new FakeStatusService
        {
            NextResult = new RobotStatusResult(true, false, "Unauthorized")
        };
        var vm = NewViewModel(fake);

        await vm.RefreshAsync();

        Assert.True(vm.ApiConnected);
        Assert.False(vm.IsRobotRunOk);
        Assert.Equal("False", vm.IsRobotRunOkText);
        Assert.Contains("Unauthorized", vm.Message);
    }

    [Fact]
    public async Task RefreshAsync_sets_both_false_when_api_unreachable()
    {
        var fake = new FakeStatusService
        {
            NextResult = new RobotStatusResult(false, false, "Unreachable")
        };
        var vm = NewViewModel(fake);

        await vm.RefreshAsync();

        Assert.False(vm.ApiConnected);
        Assert.False(vm.IsRobotRunOk);
        Assert.Equal("Not Connected", vm.ApiConnectedText);
    }

    private static MainWindowViewModel NewViewModel(IRobotStatusService statusService)
    {
        return new MainWindowViewModel(statusService, new RobotAppSettings
        {
            ApiBaseUrl = "http://localhost:5188",
            ApiKey = "test-api-key"
        });
    }

    private sealed class FakeStatusService : IRobotStatusService
    {
        public RobotStatusResult NextResult { get; set; } = new(false, false, "Not configured");

        public Task<RobotStatusResult> CheckAsync(string baseUrl, string apiKey, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(NextResult);
        }
    }
}
