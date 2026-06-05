namespace RobotStopApp.RobotApp.Services;

public record RobotStatusResult(bool ApiConnected, bool IsRobotRunOk, string Message);
