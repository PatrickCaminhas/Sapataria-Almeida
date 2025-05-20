using System;
using System.Windows.Input;

namespace Sapataria_Almeida.Commands
{
    /// <summary>
    /// Implementação simples de ICommand para ligar XAML a métodos.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
            => _canExecute?.Invoke(parameter) ?? true;

        public void Execute(object? parameter)
            => _execute(parameter);

        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// Chame este método para forçar a reavaliação de CanExecute.
        /// </summary>
        public void RaiseCanExecuteChanged()
            => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
