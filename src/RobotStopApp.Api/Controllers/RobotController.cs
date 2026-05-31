using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RobotStopApp.Api.Models;
using RobotStopApp.Api.Robot;

namespace RobotStopApp.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/robot")]
[Produces("application/json")]
public class RobotController : ControllerBase
{
    private readonly IRobotController _robot;
    private readonly ILogger<RobotController> _logger;

    public RobotController(IRobotController robot, ILogger<RobotController> logger)
    {
        _robot = robot;
        _logger = logger;
    }

    [HttpPost("run")]
    [ProducesResponseType(typeof(RobotStateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Run(CancellationToken cancellationToken)
    {
        try
        {
            var state = await _robot.RunAsync(cancellationToken);
            return Ok(RobotStateResponse.From(state));
        }
        catch (InvalidRobotTransitionException ex)
        {
            _logger.LogWarning(ex, "Invalid transition on run");
            return Conflict(new ErrorResponse("InvalidTransition", ex.Message));
        }
    }

    [HttpPost("stop")]
    [ProducesResponseType(typeof(RobotStateResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Stop(CancellationToken cancellationToken)
    {
        var state = await _robot.StopAsync(cancellationToken);
        return Ok(RobotStateResponse.From(state));
    }

    [HttpGet("status")]
    [ProducesResponseType(typeof(RobotStateResponse), StatusCodes.Status200OK)]
    public IActionResult Status()
    {
        return Ok(RobotStateResponse.From(_robot.GetStatus()));
    }
}
