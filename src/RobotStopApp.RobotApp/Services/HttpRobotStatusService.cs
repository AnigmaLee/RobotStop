using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using RobotStopApp.Service.Robot;

namespace RobotStopApp.RobotApp.Services;

public sealed class HttpRobotStatusService(HttpClient httpClient) : IRobotStatusService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<RobotStatusResult> CheckAsync(string baseUrl, string apiKey, CancellationToken cancellationToken = default)
    {
        if (!Uri.TryCreate(baseUrl?.Trim(), UriKind.Absolute, out var rootUri))
        {
            return new RobotStatusResult(false, false, "Invalid API URL.");
        }

        try
        {
            using var healthResponse = await _httpClient.GetAsync(new Uri(rootUri, "/health"), cancellationToken);
            if (!healthResponse.IsSuccessStatusCode)
            {
                return new RobotStatusResult(false, false, $"API health check failed ({(int)healthResponse.StatusCode}).");
            }

            using var statusRequest = new HttpRequestMessage(HttpMethod.Get, new Uri(rootUri, "/api/robot/status"));
            statusRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                statusRequest.Headers.Add("X-Api-Key", apiKey.Trim());
            }

            using var statusResponse = await _httpClient.SendAsync(statusRequest, cancellationToken);
            if (statusResponse.IsSuccessStatusCode)
            {
                var state = await ReadStateAsync(statusResponse, cancellationToken);
                if (state == RobotState.Running)
                {
                    return new RobotStatusResult(true, true, "API connected and robot state is Running.");
                }

                return new RobotStatusResult(true, false, $"API connected and robot state is {state?.ToString() ?? "Unknown"}.");
            }

            if (statusResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                return new RobotStatusResult(true, false, "API connected but API key is unauthorized.");
            }

            return new RobotStatusResult(true, false, $"API connected but status request failed ({(int)statusResponse.StatusCode}).");
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return new RobotStatusResult(false, false, $"Request failed: {ex.Message}");
        }
    }

    private static async Task<RobotState?> ReadStateAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.Content.Headers.ContentLength == 0)
        {
            return null;
        }

        try
        {
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var json = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
            if (json.RootElement.TryGetProperty("state", out var stateElement) &&
                stateElement.ValueKind == JsonValueKind.Number &&
                stateElement.TryGetInt32(out var numericState) &&
                Enum.IsDefined(typeof(RobotState), numericState))
            {
                return (RobotState)numericState;
            }

            if (json.RootElement.TryGetProperty("state", out stateElement) &&
                stateElement.ValueKind == JsonValueKind.String)
            {
                var text = stateElement.GetString();
                if (Enum.TryParse<RobotState>(text, ignoreCase: true, out var parsedState))
                {
                    return parsedState;
                }
            }
        }
        catch
        {
            // Keep this resilient: if payload parsing fails, caller still gets connectivity status.
        }

        return null;
    }
}
