using System.Windows.Input;

namespace RobotStopApp.RobotApp.Infrastructure;

public sealed class AsyncCommand(Func<CancellationToken, Task> execute, Func<bool>? canExecute = null) : ICommand
{
    private readonly Func<CancellationToken, Task> _execute = execute;
    private readonly Func<bool> _canExecute = canExecute ?? (() => true);

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => _canExecute();

    public async void Execute(object? parameter)
    {
        await _execute(CancellationToken.None);
    }

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
