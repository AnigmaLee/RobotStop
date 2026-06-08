using RobotStopApp.RobotApp.WinForms.Services;
using Xunit;

namespace RobotStopApp.RobotApp.WinForms.Tests
{
    public class RobotApiResultTests
    {
        [Fact]
        public void Constructor_ShouldAssignValues()
        {
            var result = new RobotApiResult(
                apiConnected: true,
                isRobotRunOk: true,
                message: "API connected and robot state is Running.");

            Assert.True(result.ApiConnected);
            Assert.True(result.IsRobotRunOk);
            Assert.Equal("API connected and robot state is Running.", result.Message);
        }
    }
}
