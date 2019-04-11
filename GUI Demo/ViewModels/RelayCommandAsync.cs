using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GUI_Demo.ViewModels
{
    class RelayCommandAsync : ICommand
    {
        // Derived from several sources.
        private readonly Func<object, Task> _execute;
        private readonly Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommandAsync(Func<object, Task> execute) : this(execute, null) { }

        public RelayCommandAsync(Func<object, Task> execute, Func<object, bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException("execute");
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);
        public async void Execute(object parameter) => await _execute(parameter);
    }
}
