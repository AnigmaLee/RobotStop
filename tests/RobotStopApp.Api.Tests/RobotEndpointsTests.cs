using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RobotStopApp.Api.Robot;
using RobotStopApp.Service.Models;
using RobotStopApp.Service.Robot;
using Xunit;

namespace RobotStopApp.Api.Tests;

public class RobotEndpointsTests : IClassFixture<RobotEndpointsTests.Factory>
{
    private const string TestApiKey = "test-api-key";
    private readonly Factory _factory;

    public RobotEndpointsTests(Factory factory) => _factory = factory;

    public class Factory : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            Environment.SetEnvironmentVariable("ROBOTSTOPAPP_APIKEY", TestApiKey);
            builder.ConfigureServices(services =>
            {
                // Fresh stub per factory instance.
                var existing = services.Single(s => s.ServiceType == typeof(IRobotController));
                services.Remove(existing);
                services.AddSingleton<IRobotController, StubRobotController>();
            });
            return base.CreateHost(builder);
        }
    }

    private HttpClient AuthedClient()
    {
        var c = _factory.CreateClient();
        c.DefaultRequestHeaders.Add("X-Api-Key", TestApiKey);
        return c;
    }

    [Fact]
    public async Task Run_without_api_key_returns_401()
    {
        var client = _factory.CreateClient();
        var resp = await client.PostAsync("/api/robot/run", content: null);
        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Fact]
    public async Task Run_with_wrong_api_key_returns_401()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Api-Key", "wrong");
        var resp = await client.PostAsync("/api/robot/run", content: null);
        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Fact]
    public async Task Health_is_anonymous()
    {
        var client = _factory.CreateClient();
        var resp = await client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Fact]
    public async Task Full_run_stop_status_cycle()
    {
        var client = AuthedClient();

        var run = await client.PostAsync("/api/robot/run", content: null);
        Assert.Equal(HttpStatusCode.OK, run.StatusCode);
        var runBody = await run.Content.ReadFromJsonAsync<RobotStateResponse>();
        Assert.Equal(RobotState.Running, runBody!.State);

        var status = await client.GetFromJsonAsync<RobotStateResponse>("/api/robot/status");
        Assert.Equal(RobotState.Running, status!.State);

        var stop = await client.PostAsync("/api/robot/stop", content: null);
        Assert.Equal(HttpStatusCode.OK, stop.StatusCode);
        var stopBody = await stop.Content.ReadFromJsonAsync<RobotStateResponse>();
        Assert.Equal(RobotState.Stopped, stopBody!.State);
    }

    [Fact]
    public async Task Run_when_already_running_returns_409()
    {
        var client = AuthedClient();
        await client.PostAsync("/api/robot/stop", content: null); // reset
        var first = await client.PostAsync("/api/robot/run", content: null);
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);

        var second = await client.PostAsync("/api/robot/run", content: null);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }
}
