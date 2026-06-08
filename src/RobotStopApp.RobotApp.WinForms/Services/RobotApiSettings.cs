namespace RobotStopApp.RobotApp.WinForms.Services
{
    public sealed class RobotApiSettings
    {
        public string ApiBaseUrl { get; set; } = "http://localhost:5188";

        public string ApiKey { get; set; } = "test-api-key";
    }
}
