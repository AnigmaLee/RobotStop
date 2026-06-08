namespace RobotStopApp.RobotApp.WinForms.Services
{
    public sealed class RobotApiResult
    {
        public RobotApiResult(bool apiConnected, bool isRobotRunOk, string message)
        {
            ApiConnected = apiConnected;
            IsRobotRunOk = isRobotRunOk;
            Message = message;
        }

        public bool ApiConnected { get; }

        public bool IsRobotRunOk { get; }

        public string Message { get; }
    }
}
