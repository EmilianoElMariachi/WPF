using System.Collections.Generic;
using System.Linq;
using ElMariachi.WPF.Tools.UndoRedo.EventsDefinition;
using ElMariachi.WPF.Tools.UndoRedo.RevertibleCommands;

namespace ElMariachi.WPF.Tools.UndoRedo
{
    public class UndoRedoService : IUndoRedoService
    {

        #region Fields and Properties

        private readonly object _lock = new object();
        private readonly List<IRevertibleCommand> _revertibleCommandsStack = new List<IRevertibleCommand>();

        public UndoRedoStackData[] UndoRedoStack
        {
            get
            {
                return _revertibleCommandsStack.Select((revertibleCommand) => new UndoRedoStackData(revertibleCommand.Id, revertibleCommand.Description)).ToArray();
            }
        }

        private int _currentDoneCommandStackIndex = -1;

        public bool IsUndoing { get; private set; }

        public bool IsRedoing { get; private set; }

        public int NbUndo
        {
            get
            {
                lock (_lock)
                {
                    return _currentDoneCommandStackIndex + 1;
                }
            }
        }

        public int NbRedo
        {
            get
            {
                lock (_lock)
                {
                    return _revertibleCommandsStack.Count - (_currentDoneCommandStackIndex + 1);
                }
            }
        }

        #endregion

        #region Events

        public event BeforeUndoRedoCommandExecutedEventHandler BeforeUndoRedoCommandExecuted;

        protected virtual void NotifyBeforeUndoRedoCommandExecuted(BeforeUndoRedoCommandExecutedEventHandlerArgs args)
        {
            var handler = BeforeUndoRedoCommandExecuted;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        public event UndoRedoCommandExecutedEventHandler UndoRedoCommandExecuted;

        protected virtual void NotifyUndoRedoCommandExecuted(UndoRedoCommandExecutedEventHandlerArgs args)
        {
            var handler = UndoRedoCommandExecuted;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion

        #region Public Methods

        public bool Execute(IRevertibleCommand revertibleCommand)
        {
            lock (_lock)
            {
                NotifyBeforeUndoRedoCommandExecuted(new BeforeUndoRedoCommandExecutedEventHandlerArgs(UndoRedoExecutionCategory.NEW));

                revertibleCommand.Do();

                AddExecutedCommand(revertibleCommand);

                NotifyUndoRedoCommandExecuted(new UndoRedoCommandExecutedEventHandlerArgs(UndoRedoExecutionCategory.NEW));

                return true;
            }
        }

        public void AddExecutedCommand(IRevertibleCommand revertibleCommand)
        {
            lock (_lock)
            {
                PruneCommands();
                _revertibleCommandsStack.Add(revertibleCommand);
                _currentDoneCommandStackIndex = _revertibleCommandsStack.Count - 1;
            }
        }

        public bool CanUndo()
        {
            lock (_lock)
            {
                return _currentDoneCommandStackIndex >= 0;
            }
        }

        public bool CanRedo()
        {
            lock (_lock)
            {
                return _currentDoneCommandStackIndex < (_revertibleCommandsStack.Count - 1);
            }
        }

        public void Redo()
        {
            lock (_lock)
            {
                if (!this.CanRedo())
                {
                    return;
                }

                NotifyBeforeUndoRedoCommandExecuted(new BeforeUndoRedoCommandExecutedEventHandlerArgs(UndoRedoExecutionCategory.REDO));
                try
                {
                    IsRedoing = true;
                    _revertibleCommandsStack[_currentDoneCommandStackIndex + 1].Do();
                    _currentDoneCommandStackIndex++;
                }
                finally
                {
                    IsRedoing = false;
                }
                NotifyUndoRedoCommandExecuted(new UndoRedoCommandExecutedEventHandlerArgs(UndoRedoExecutionCategory.REDO));
            }
        }

        public void Undo()
        {
            lock (_lock)
            {
                if (!this.CanUndo())
                {
                    return;
                }

                NotifyBeforeUndoRedoCommandExecuted(new BeforeUndoRedoCommandExecutedEventHandlerArgs(UndoRedoExecutionCategory.UNDO));
                try
                {
                    IsUndoing = true;
                    _revertibleCommandsStack[_currentDoneCommandStackIndex].Undo();
                    _currentDoneCommandStackIndex--;
                }
                finally
                {
                    IsUndoing = false;
                }
                NotifyUndoRedoCommandExecuted(new UndoRedoCommandExecutedEventHandlerArgs(UndoRedoExecutionCategory.UNDO));
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Supprime toutes les commandes qui suivent la commande à l'index courant
        /// </summary>
        private void PruneCommands()
        {
            var firstDeletionIndex = _currentDoneCommandStackIndex + 1;
            var numberOfElementsToDelete = _revertibleCommandsStack.Count - firstDeletionIndex;
            if (numberOfElementsToDelete > 0)
            {
                _revertibleCommandsStack.RemoveRange(firstDeletionIndex, numberOfElementsToDelete);
            }
        }

        #endregion

    }
}