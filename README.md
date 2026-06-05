# RobotStopApp

Robot control solution with a reusable service DLL, optional HTTP API host, and a simple Robot App UI consumer.

## Architecture

- `src/RobotStopApp.Service`: reusable DLL with shared robot contracts and models for third-party consumers.
- `src/RobotStopApp.Api`: optional HTTP host (API transport adapter) over the service contracts.
- `src/RobotStopApp.RobotApp`: simple Avalonia Robot App consumer showing `API connected` and `isRobotRunOK`.

## Quick Start

1. Start API host:

```powershell
dotnet run --project src/RobotStopApp.Api
```

2. Start Robot App:

```powershell
dotnet run --project src/RobotStopApp.RobotApp
```

3. In Robot App, click **Check Robot Status**.

4. Verify indicators:
- `API connected`
- `isRobotRunOK`

## Endpoints

| Method | Path                | Auth | Description                        |
| ------ | ------------------- | ---- | ---------------------------------- |
| POST   | `/api/robot/run`    | yes  | Start the robot (409 if running).  |
| POST   | `/api/robot/stop`   | yes  | Stop the robot (idempotent).       |
| GET    | `/api/robot/status` | yes  | Current robot state.               |
| GET    | `/health`           | no   | Health probe.                      |
| GET    | `/swagger`          | no   | Swagger UI (Development only).     |

Authentication: send header `X-Api-Key: <key>`. The key is read from `ROBOTSTOPAPP_APIKEY` env var (preferred) or `ApiKey` in `appsettings.json`.

## Run

```powershell
dotnet run --project src/RobotStopApp.Api
```

## Test

```powershell
dotnet test
```

## Robot App (Simple Status UI)

Run the Robot App consumer:

```powershell
dotnet run --project src/RobotStopApp.RobotApp
```

UI indicators:

- `API connected`: health/API reachability result
- `isRobotRunOK`: run command result

Configuration file: `src/RobotStopApp.RobotApp/appsettings.json`

- `RobotApp:ApiBaseUrl`
- `RobotApp:ApiKey`

## Third-Party DLL Consumption

Reference `src/RobotStopApp.Service` in your app to consume shared contracts directly.

Key contract types:
- `RobotStopApp.Service.Robot.IRobotController`
- `RobotStopApp.Service.Robot.RobotState`
- `RobotStopApp.Service.Robot.InvalidRobotTransitionException`
- `RobotStopApp.Service.Models.RobotStateResponse`
- `RobotStopApp.Service.Models.ErrorResponse`

If you need out-of-process communication instead of direct DLL consumption, use the API host endpoints listed above.

