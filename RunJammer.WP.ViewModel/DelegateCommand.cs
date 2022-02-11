using System;
using System.Windows.Input;

public class DelegateCommand<T> : ICommand where T : class
{
    private Action<T> executeDelegate;
    private Func<T, bool> canExecuteDelegate;

    public DelegateCommand(Action<T> executeDelegate, Func<T, bool> canExecuteDelegate)
    {
        this.canExecuteDelegate = canExecuteDelegate;
        this.executeDelegate = executeDelegate;
    }

    public DelegateCommand(Action<T> executeDelegate)
        : this(executeDelegate, null)
    {
    }

    public bool CanExecute(object parameter)
    {
        if (canExecuteDelegate == null)
            return true;

        return canExecuteDelegate.Invoke(parameter as T);
    }

    public event EventHandler CanExecuteChanged;

    public void RaiseCanExecuteChanged()
    {
        if (CanExecuteChanged != null)
            CanExecuteChanged(this, EventArgs.Empty);
    }

    public void Execute(object parameter)
    {
        executeDelegate.Invoke(parameter as T);
    }
}

public class DelegateCommand : ICommand
{
    private Action executeDelegate;
    private Func<bool> canExecuteDelegate;

    public DelegateCommand(Action executeDelegate, Func<bool> canExecuteDelegate)
    {
        this.canExecuteDelegate = canExecuteDelegate;
        this.executeDelegate = executeDelegate;
    }

    public DelegateCommand(Action executeDelegate)
        : this(executeDelegate, null)
    {
    }

    public bool CanExecute(object parameter)
    {
        if (canExecuteDelegate == null)
            return true;

        return canExecuteDelegate.Invoke();
    }

    public event EventHandler CanExecuteChanged;

    public void RaiseCanExecuteChanged()
    {
        if (CanExecuteChanged != null)
            CanExecuteChanged(this, EventArgs.Empty);
    }

    public void Execute(object parameter)
    {
        executeDelegate.Invoke();
    }
}

