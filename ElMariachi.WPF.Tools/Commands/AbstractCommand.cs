using System;
using System.Windows.Input;

namespace ElMariachi.WPF.Tools.Commands
{
    /// <summary>
    /// Minimalist <see cref="ICommand"/> implementation
    /// </summary>
    public abstract class AbstractCommand : ICommand
    {

        #region Events

        public event EventHandler CanExecuteChanged;

        protected virtual void NotifyCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Methods

        public abstract void Execute(object parameter);

        public abstract bool CanExecute(object parameter);

        #endregion

    }
}
