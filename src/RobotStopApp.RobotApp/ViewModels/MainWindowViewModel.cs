using System.Windows.Input;
using Avalonia.Media;
using RobotStopApp.RobotApp.Infrastructure;
using RobotStopApp.RobotApp.Services;

namespace RobotStopApp.RobotApp.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
    private static readonly IBrush GoodBrush = new SolidColorBrush(Color.Parse("#2B8A3E"));
    private static readonly IBrush BadBrush = new SolidColorBrush(Color.Parse("#B63A3A"));

    private readonly IRobotStatusService _statusService;
    private readonly AsyncCommand _refreshCommand;

    private string _apiBaseUrl;
    private string _apiKey;
    private bool _apiConnected;
    private bool _isRobotRunOk;
    private bool _isBusy;
    private string _message = "Ready.";

    public MainWindowViewModel(IRobotStatusService statusService, RobotAppSettings settings)
    {
        _statusService = statusService;
        _apiBaseUrl = settings.ApiBaseUrl;
        _apiKey = settings.ApiKey;
        _refreshCommand = new AsyncCommand(_ => RefreshAsync(), () => !IsBusy);
    }

    public string ApiBaseUrl
    {
        get => _apiBaseUrl;
        set => SetProperty(ref _apiBaseUrl, value);
    }

    public string ApiKey
    {
        get => _apiKey;
        set => SetProperty(ref _apiKey, value);
    }

    public bool ApiConnected
    {
        get => _apiConnected;
        private set
        {
            if (SetProperty(ref _apiConnected, value))
            {
                OnStatusVisualsChanged();
            }
        }
    }

    public bool IsRobotRunOk
    {
        get => _isRobotRunOk;
        private set
        {
            if (SetProperty(ref _isRobotRunOk, value))
            {
                OnStatusVisualsChanged();
            }
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (SetProperty(ref _isBusy, value))
            {
                _refreshCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string Message
    {
        get => _message;
        private set => SetProperty(ref _message, value);
    }

    public string ApiConnectedText => ApiConnected ? "Connected" : "Not Connected";
    public string IsRobotRunOkText => IsRobotRunOk ? "True" : "False";
    public IBrush ApiConnectedBrush => ApiConnected ? GoodBrush : BadBrush;
    public IBrush IsRobotRunOkBrush => IsRobotRunOk ? GoodBrush : BadBrush;

    public ICommand RefreshCommand => _refreshCommand;

    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        try
        {
            var result = await _statusService.CheckAsync(ApiBaseUrl, ApiKey, cancellationToken);
            ApiConnected = result.ApiConnected;
            IsRobotRunOk = result.IsRobotRunOk;
            Message = result.Message;
        }
        catch (OperationCanceledException)
        {
            Message = "Request canceled.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OnStatusVisualsChanged()
    {
        RaisePropertyChanged(nameof(ApiConnectedText));
        RaisePropertyChanged(nameof(IsRobotRunOkText));
        RaisePropertyChanged(nameof(ApiConnectedBrush));
        RaisePropertyChanged(nameof(IsRobotRunOkBrush));
    }
}
