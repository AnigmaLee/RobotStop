using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia.Media;
using RobotStopApp.Client.Ui.Infrastructure;
using RobotStopApp.Client.Ui.Services;

namespace RobotStopApp.Client.Ui.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
    private static readonly IBrush GoodBrush = new SolidColorBrush(Color.Parse("#1C6E5A"));
    private static readonly IBrush WarnBrush = new SolidColorBrush(Color.Parse("#9D6A2A"));
    private static readonly IBrush BadBrush = new SolidColorBrush(Color.Parse("#B63A3A"));
    private static readonly IBrush NeutralBrush = new SolidColorBrush(Color.Parse("#5A544A"));

    private readonly IRobotApiClient _apiClient;
    private readonly AsyncCommand _runCommand;
    private readonly AsyncCommand _stopCommand;
    private readonly AsyncCommand _statusCommand;
    private readonly RelayCommand _cancelCommand;

    private CancellationTokenSource? _activeRequest;
    private string _baseUrl;
    private string _apiKey;
    private IBrush _stateBrush = NeutralBrush;
    private string _stateLabel = "Unknown";
    private string _statusMessage = "Ready.";
    private string _timestampText = "-";
    private bool _isBusy;

    public MainWindowViewModel(IRobotApiClient apiClient, ApiClientSettings settings)
    {
        _apiClient = apiClient;
        _baseUrl = settings.BaseUrl;
        _apiKey = settings.ApiKey;

        _runCommand = new AsyncCommand(_ => ExecuteRunAsync(), () => !IsBusy);
        _stopCommand = new AsyncCommand(_ => ExecuteStopAsync(), () => !IsBusy);
        _statusCommand = new AsyncCommand(_ => ExecuteStatusAsync(), () => !IsBusy);
        _cancelCommand = new RelayCommand(CancelActiveRequest, () => IsBusy);
    }

    public string BaseUrl
    {
        get => _baseUrl;
        set => SetProperty(ref _baseUrl, value);
    }

    public string ApiKey
    {
        get => _apiKey;
        set => SetProperty(ref _apiKey, value);
    }

    public IBrush StateBrush
    {
        get => _stateBrush;
        private set => SetProperty(ref _stateBrush, value);
    }

    public string StateLabel
    {
        get => _stateLabel;
        private set => SetProperty(ref _stateLabel, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    public string TimestampText
    {
        get => _timestampText;
        private set => SetProperty(ref _timestampText, value);
    }

    public ObservableCollection<string> RequestLog { get; } = new();

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (!SetProperty(ref _isBusy, value))
            {
                return;
            }

            _runCommand.RaiseCanExecuteChanged();
            _stopCommand.RaiseCanExecuteChanged();
            _statusCommand.RaiseCanExecuteChanged();
            _cancelCommand.RaiseCanExecuteChanged();
        }
    }

    public ICommand RunCommand => _runCommand;
    public ICommand StopCommand => _stopCommand;
    public ICommand StatusCommand => _statusCommand;
    public ICommand CancelCommand => _cancelCommand;

    private Task ExecuteRunAsync() => ExecuteActionAsync("Run", ct => _apiClient.RunAsync(BaseUrl, ApiKey, ct));

    private Task ExecuteStopAsync() => ExecuteActionAsync("Stop", ct => _apiClient.StopAsync(BaseUrl, ApiKey, ct));

    private Task ExecuteStatusAsync() => ExecuteActionAsync("Status", ct => _apiClient.StatusAsync(BaseUrl, ApiKey, ct));

    private async Task ExecuteActionAsync(string actionName, Func<CancellationToken, Task<RobotApiResult>> operation)
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        _activeRequest = new CancellationTokenSource();

        try
        {
            AppendLog($"{actionName} requested");
            var result = await operation(_activeRequest.Token);
            ApplyResult(actionName, result);
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "Request canceled.";
            StateBrush = WarnBrush;
            AppendLog($"{actionName} canceled");
        }
        finally
        {
            _activeRequest.Dispose();
            _activeRequest = null;
            IsBusy = false;
        }
    }

    private void ApplyResult(string actionName, RobotApiResult result)
    {
        StatusMessage = result.Message;

        if (!string.IsNullOrWhiteSpace(result.State))
        {
            StateLabel = result.State!;
        }

        if (result.Timestamp.HasValue)
        {
            TimestampText = result.Timestamp.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
        }

        StateBrush = ResolveStateBrush(result);
        AppendLog($"{actionName}: {result.Message}");
    }

    private static IBrush ResolveStateBrush(RobotApiResult result)
    {
        if (!result.IsSuccess)
        {
            return BadBrush;
        }

        return result.State?.ToLowerInvariant() switch
        {
            "running" => GoodBrush,
            "stopped" => WarnBrush,
            "idle" => WarnBrush,
            _ => GoodBrush
        };
    }

    private void CancelActiveRequest()
    {
        _activeRequest?.Cancel();
    }

    private void AppendLog(string message)
    {
        RequestLog.Insert(0, $"{DateTimeOffset.Now:HH:mm:ss}  {message}");

        while (RequestLog.Count > 200)
        {
            RequestLog.RemoveAt(RequestLog.Count - 1);
        }
    }
}
