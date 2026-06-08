using System.Threading;
using System.Threading.Tasks;

namespace RobotStopApp.RobotApp.WinForms.Services
{
    public interface IRobotApiClient
    {
        Task<RobotApiResult> CheckAsync(string baseUrl, string apiKey, CancellationToken cancellationToken = default(CancellationToken));
    }
}
