using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using RobotStopApp.RobotApp.WinForms.Models;

namespace RobotStopApp.RobotApp.WinForms.Services
{
    public sealed class RobotApiClient : IRobotApiClient
    {
        private readonly HttpClient _httpClient;

        public RobotApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<RobotApiResult> CheckAsync(string baseUrl, string apiKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!Uri.TryCreate((baseUrl ?? string.Empty).Trim(), UriKind.Absolute, out var rootUri))
            {
                return new RobotApiResult(false, false, "Invalid API URL.");
            }

            try
            {
                using (var healthResponse = await _httpClient.GetAsync(new Uri(rootUri, "/health"), cancellationToken).ConfigureAwait(false))
                {
                    if (!healthResponse.IsSuccessStatusCode)
                    {
                        return new RobotApiResult(false, false, string.Format("API health check failed ({0}).", (int)healthResponse.StatusCode));
                    }
                }

                using (var statusRequest = new HttpRequestMessage(HttpMethod.Get, new Uri(rootUri, "/api/robot/status")))
                {
                    statusRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    if (!string.IsNullOrWhiteSpace(apiKey))
                    {
                        statusRequest.Headers.Add("X-Api-Key", apiKey.Trim());
                    }

                    using (var statusResponse = await _httpClient.SendAsync(statusRequest, cancellationToken).ConfigureAwait(false))
                    {
                        if (statusResponse.IsSuccessStatusCode)
                        {
                            var state = await ReadStateAsync(statusResponse, cancellationToken).ConfigureAwait(false);
                            if (state == RobotState.Running)
                            {
                                return new RobotApiResult(true, true, "API connected and robot state is Running.");
                            }

                            return new RobotApiResult(
                                true,
                                false,
                                string.Format("API connected and robot state is {0}.", state?.ToString() ?? "Unknown"));
                        }

                        if (statusResponse.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            return new RobotApiResult(true, false, "API connected but API key is unauthorized.");
                        }

                        return new RobotApiResult(
                            true,
                            false,
                            string.Format("API connected but status request failed ({0}).", (int)statusResponse.StatusCode));
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return new RobotApiResult(false, false, "Request failed: " + ex.Message);
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
                var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                using (var json = JsonDocument.Parse(responseBody))
                {
                    JsonElement stateElement;
                    if (json.RootElement.TryGetProperty("state", out stateElement))
                    {
                        if (stateElement.ValueKind == JsonValueKind.Number &&
                            stateElement.TryGetInt32(out var numericState) &&
                            Enum.IsDefined(typeof(RobotState), numericState))
                        {
                            return (RobotState)numericState;
                        }

                        if (stateElement.ValueKind == JsonValueKind.String)
                        {
                            var text = stateElement.GetString();
                            RobotState parsedState;
                            if (Enum.TryParse(text, true, out parsedState))
                            {
                                return parsedState;
                            }
                        }
                    }
                }
            }
            catch
            {
                // Keep resilient so connectivity still reports even when payload parsing fails.
            }
            return null;
        }
    }
}
