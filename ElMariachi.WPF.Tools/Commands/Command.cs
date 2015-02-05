using System;

namespace ElMariachi.WPF.Tools.Commands
{
    public class Command : AbstractCommand
    {
        private readonly Action _executeAction;
        private readonly Action<object> _executeActionWithArg;
        private readonly Func<object, bool> _canExecuteFunctionWithArg;
        private readonly Func<bool> _canExecuteFunction;

        public Command(Action<object> executeActionWithArg)
        {
            _executeActionWithArg = executeActionWithArg;
        }

        public Command(Action<object> executeActionWithArg, Func<object, bool> canExecuteFunctionWithArg)
            : this(executeActionWithArg)
        {
            _canExecuteFunctionWithArg = canExecuteFunctionWithArg;
        }

        public Command(Action executeAction)
        {
            _executeAction = executeAction;
        }

        public Command(Action executeAction, Func<bool> canExecuteFunction) : this(executeAction)
        {
            _canExecuteFunction = canExecuteFunction;
        }

        public override void Execute(object parameter)
        {
            if (_executeActionWithArg != null)
            {
                _executeActionWithArg(parameter);
            }
            else if (_executeAction != null)
            {
                _executeAction();
            }
        }

        public override bool CanExecute(object parameter)
        {
            if (_canExecuteFunctionWithArg != null)
            {
                return _canExecuteFunctionWithArg(parameter);
            }
            else if (_canExecuteFunction != null)
            {
                return _canExecuteFunction();
            }
            else
            {
                return true;
            }
        }

        public void FireCanExecuteChanged()
        {
            this.NotifyCanExecuteChanged();
        }
    }
}