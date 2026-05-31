# RobotStopApp

ASP.NET Core Web API exposing endpoints to stop and run a robot.

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
