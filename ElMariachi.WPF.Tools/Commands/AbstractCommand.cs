using System;
using System.Windows.Input;

namespace ElMariachi.WPF.Tools.Commands
{
    public abstract class AbstractCommand : ICommand
    {
        public abstract void Execute(object parameter);

        public abstract bool CanExecute(object parameter);

        public event EventHandler CanExecuteChanged;

        protected virtual void NotifyCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
