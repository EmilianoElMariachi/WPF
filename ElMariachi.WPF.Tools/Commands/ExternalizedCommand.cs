using System;
using System.Windows.Input;

namespace ElMariachi.WPF.Tools.Commands
{

    /// <summary>
    /// An <see cref="ExternalizedCommand"/> is a <see cref="ICommand"/> for which the logic of the methods <see cref="Execute"/> and <see cref="CanExecute"/>
    /// is defined outside of the command itself.
    /// <remarks>The class name is called "Externalized" because the responsability of creating the command logic is up to the class user.</remarks>
    /// </summary>
    public class ExternalizedCommand : AbstractCommand
    {

        #region Fields & Properties

        private readonly Action _executeAction;
        private readonly Action<object> _executeActionWithArg;
        private readonly Func<object, bool> _canExecuteFunctionWithArg;
        private readonly Func<bool> _canExecuteFunction;

        #endregion

        #region Constructors

        public ExternalizedCommand(Action<object> executeActionWithArg)
        {
            _executeActionWithArg = executeActionWithArg;
        }

        public ExternalizedCommand(Action<object> executeActionWithArg, Func<object, bool> canExecuteFunctionWithArg)
            : this(executeActionWithArg)
        {
            _canExecuteFunctionWithArg = canExecuteFunctionWithArg;
        }

        public ExternalizedCommand(Action executeAction)
        {
            _executeAction = executeAction;
        }

        public ExternalizedCommand(Action executeAction, Func<bool> canExecuteFunction) : this(executeAction)
        {
            _canExecuteFunction = canExecuteFunction;
        }

        #endregion

        #region Methods

        /// <summary>
        /// <see cref="ICommand.Execute"/>
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
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

        /// <summary>
        /// <see cref="ICommand.CanExecute"/>
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Triggers <see cref="ICommand.CanExecuteChanged"/> event
        /// </summary>
        public void FireCanExecuteChanged()
        {
            this.NotifyCanExecuteChanged();
        }

        #endregion

    }
}