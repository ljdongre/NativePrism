// Native replacement shim for Prism Library MVVM core types.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Input;

namespace Prism.Mvvm
{
    /// <summary>
    /// Replacement for Prism.Mvvm.BindableBase.
    /// Provides INotifyPropertyChanged support with SetProperty helper.
    /// </summary>
    public abstract class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">Name of the property that changed.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Sets the property value and raises PropertyChanged if the value changed.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to the backing field.</param>
        /// <param name="value">New value.</param>
        /// <param name="propertyName">Name of the property (auto-filled by compiler).</param>
        /// <returns>True if the value was changed; otherwise false.</returns>
        protected virtual bool SetProperty<T>(ref T storage, T value,
            [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Raises PropertyChanged for the given property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }

        /// <summary>
        /// Raises PropertyChanged using a lambda expression to identify the property.
        /// </summary>
        /// <typeparam name="T">The property type.</typeparam>
        /// <param name="propertyExpression">A lambda expression selecting the property (e.g. () =&gt; PropertyName).</param>
        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression != null)
            {
                OnPropertyChanged(memberExpression.Member.Name);
            }
        }
    }
}

namespace Prism.Commands
{
    // --- Non-Generic Implementation ---
    public class DelegateCommand : ICommand
    {
        private readonly Func<Task> _executeMethod;
        private readonly Func<bool> _canExecuteMethod;
        
        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod = null)
            : this(() => { executeMethod?.Invoke(); return Task.CompletedTask; }, canExecuteMethod) { }
        
        public DelegateCommand(Func<Task> executeMethod, Func<bool> canExecuteMethod = null)
        {
            _executeMethod = executeMethod ?? throw new ArgumentNullException(nameof(executeMethod));
            _canExecuteMethod = canExecuteMethod;
        }
        
        public bool CanExecute(object parameter) => _canExecuteMethod == null || _canExecuteMethod();
        
        public async void Execute(object parameter) => await ExecuteAsync();
        
        public virtual async Task ExecuteAsync()
        {
            if (CanExecute(null)) await _executeMethod();
        }
        
        public event EventHandler CanExecuteChanged;
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    // --- Generic Implementation ---
    public class DelegateCommand<T> : ICommand
    {
        private readonly Func<T, Task> _executeMethod;
        private readonly Func<T, bool> _canExecuteMethod;
    
        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod = null)
            : this((t) => { executeMethod?.Invoke(t); return Task.CompletedTask; }, canExecuteMethod) { }
    
        public DelegateCommand(Func<T, Task> executeMethod, Func<T, bool> canExecuteMethod = null)
        {
            _executeMethod = executeMethod ?? throw new ArgumentNullException(nameof(executeMethod));
            _canExecuteMethod = canExecuteMethod;
        }
    
        public bool CanExecute(object parameter)
        {
            if (_canExecuteMethod == null) return true;
            return _canExecuteMethod(ConvertParameter(parameter));
        }
    
        public async void Execute(object parameter) => await ExecuteAsync(ConvertParameter(parameter));
    
        public virtual async Task ExecuteAsync(T parameter)
        {
            if (CanExecute(parameter)) await _executeMethod(parameter);
        }
    
        protected virtual T ConvertParameter(object parameter)
        {
            if (parameter == null && typeof(T).IsValueType) return default;
            return (T)parameter;
        }
    
        public event EventHandler CanExecuteChanged;
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
  


    /// <summary>
    /// Replacement for Prism.Commands.CompositeCommand.
    /// Aggregates multiple child commands into one command.
    /// </summary>
    public class CompositeCommand : ICommand
    {
        private readonly List<ICommand> _registeredCommands = new List<ICommand>();
        private readonly bool _monitorCommandActivity;
        private readonly object _lock = new object();

        /// <summary>
        /// Creates a new CompositeCommand.
        /// </summary>
        public CompositeCommand() : this(false) { }

        /// <summary>
        /// Creates a new CompositeCommand.
        /// </summary>
        /// <param name="monitorCommandActivity">When true, only active commands are evaluated.</param>
        public CompositeCommand(bool monitorCommandActivity)
        {
            _monitorCommandActivity = monitorCommandActivity;
        }

        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Registers a child command.
        /// </summary>
        /// <param name="command">The command to register.</param>
        public void RegisterCommand(ICommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            lock (_lock)
            {
                if (!_registeredCommands.Contains(command))
                {
                    _registeredCommands.Add(command);
                    command.CanExecuteChanged += OnChildCanExecuteChanged;
                }
            }
            RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Unregisters a child command.
        /// </summary>
        /// <param name="command">The command to unregister.</param>
        public void UnregisterCommand(ICommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            lock (_lock)
            {
                if (_registeredCommands.Remove(command))
                {
                    command.CanExecuteChanged -= OnChildCanExecuteChanged;
                }
            }
            RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Gets the list of registered commands.
        /// </summary>
        public IList<ICommand> RegisteredCommands
        {
            get
            {
                lock (_lock)
                {
                    return _registeredCommands.ToList();
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            lock (_lock)
            {
                if (_registeredCommands.Count == 0)
                    return false;
                return _registeredCommands.All(cmd => cmd.CanExecute(parameter));
            }
        }

        public void Execute(object parameter)
        {
            IList<ICommand> commands;
            lock (_lock)
            {
                commands = _registeredCommands.ToList();
            }
            foreach (var command in commands)
            {
                if (command.CanExecute(parameter))
                    command.Execute(parameter);
            }
        }

        /// <summary>
        /// Raises CanExecuteChanged.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnChildCanExecuteChanged(object sender, EventArgs e)
        {
            RaiseCanExecuteChanged();
        }
    }
}
