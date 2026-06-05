using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace RobotStopApp.Client.Ui.Services;

public sealed class RobotApiClient(HttpClient httpClient) : IRobotApiClient
{
    private readonly HttpClient _httpClient = httpClient;

    public Task<RobotApiResult> RunAsync(string baseUrl, string apiKey, CancellationToken cancellationToken = default)
        => SendRobotRequestAsync(HttpMethod.Post, "/api/robot/run", baseUrl, apiKey, cancellationToken);

    public Task<RobotApiResult> StopAsync(string baseUrl, string apiKey, CancellationToken cancellationToken = default)
        => SendRobotRequestAsync(HttpMethod.Post, "/api/robot/stop", baseUrl, apiKey, cancellationToken);

    public Task<RobotApiResult> StatusAsync(string baseUrl, string apiKey, CancellationToken cancellationToken = default)
        => SendRobotRequestAsync(HttpMethod.Get, "/api/robot/status", baseUrl, apiKey, cancellationToken);

    private async Task<RobotApiResult> SendRobotRequestAsync(
        HttpMethod method,
        string relativePath,
        string baseUrl,
        string apiKey,
        CancellationToken cancellationToken)
    {
        if (!Uri.TryCreate(baseUrl?.Trim(), UriKind.Absolute, out var rootUri))
        {
            return new RobotApiResult(false, "Invalid API URL.");
        }

        try
        {
            using var request = new HttpRequestMessage(method, new Uri(rootUri, relativePath));
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                request.Headers.Add("X-Api-Key", apiKey.Trim());
            }

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            var (state, timestamp) = await TryReadStateAsync(response, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return new RobotApiResult(true, "Request succeeded.", state, timestamp);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return new RobotApiResult(false, "Unauthorized (401). Check API key.", state, timestamp);
            }

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                return new RobotApiResult(true, "Request completed with conflict (robot already in requested state).", state, timestamp);
            }

            return new RobotApiResult(false, $"Request failed ({(int)response.StatusCode} {response.ReasonPhrase}).", state, timestamp);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return new RobotApiResult(false, $"Request failed: {ex.Message}");
        }
    }

    private static async Task<(string? State, DateTimeOffset? Timestamp)> TryReadStateAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.Content.Headers.ContentLength == 0)
        {
            return (null, null);
        }

        try
        {
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var json = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            string? state = null;
            DateTimeOffset? timestamp = null;

            if (json.RootElement.TryGetProperty("state", out var stateElement) &&
                stateElement.ValueKind == JsonValueKind.Number &&
                stateElement.TryGetInt32(out var numericState))
            {
                state = numericState switch
                {
                    0 => "Idle",
                    1 => "Running",
                    2 => "Stopped",
                    3 => "Faulted",
                    _ => null
                };
            }

            if (json.RootElement.TryGetProperty("state", out stateElement) &&
                stateElement.ValueKind == JsonValueKind.String)
            {
                state = stateElement.GetString();
            }

            if (json.RootElement.TryGetProperty("timestamp", out var timeElement) &&
                timeElement.ValueKind == JsonValueKind.String &&
                DateTimeOffset.TryParse(timeElement.GetString(), out var parsed))
            {
                timestamp = parsed;
            }

            return (state, timestamp);
        }
        catch
        {
            return (null, null);
        }
    }
}
