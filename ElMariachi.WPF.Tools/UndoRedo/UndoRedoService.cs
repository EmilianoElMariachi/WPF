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

        public UndoRedoStackInfo[] StackInfo
        {
            get
            {
                return _revertibleCommandsStack.Select((revertibleCommand) => new UndoRedoStackInfo(revertibleCommand.Id, revertibleCommand.Description)).ToArray();
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

        private bool _canUndo;

        public bool CanUndo
        {
            get
            {
                lock (_lock)
                {
                    return _canUndo;
                }
            }
            private set
            {
                if (value != _canUndo)
                {
                    _canUndo = value;
                    NotifyCanUndoChanged(new CanUndoChangedEventHandlerArgs(_canUndo));
                }
            }
        }

        private bool _canRedo;

        public bool CanRedo
        {
            get
            {
                lock (_lock)
                {
                    return _canRedo;
                }
            }
            private set
            {
                if (value != _canRedo)
                {
                    _canRedo = value;
                    NotifyCanRedoChanged(new CanRedoChangedEventHandlerArgs(_canRedo));
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

        public event CanUndoChangedEventHandler CanUndoChanged;

        protected virtual void NotifyCanUndoChanged(CanUndoChangedEventHandlerArgs args)
        {
            var handler = CanUndoChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        public event CanRedoChangedEventHandler CanRedoChanged;

        protected virtual void NotifyCanRedoChanged(CanRedoChangedEventHandlerArgs args)
        {
            var handler = CanRedoChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion

        #region Public Methods

        public void Execute(IRevertibleCommand revertibleCommand)
        {
            lock (_lock)
            {
                NotifyBeforeUndoRedoCommandExecuted(new BeforeUndoRedoCommandExecutedEventHandlerArgs(UndoRedoExecutionCategory.NEW));

                revertibleCommand.Do();

                AddExecutedCommand(revertibleCommand);

                NotifyUndoRedoCommandExecuted(new UndoRedoCommandExecutedEventHandlerArgs(UndoRedoExecutionCategory.NEW));

                UpdateCanUndoCanRedo();
            }
        }

        public void AddExecutedCommand(IRevertibleCommand revertibleCommand)
        {
            lock (_lock)
            {
                PruneCommands();
                _revertibleCommandsStack.Add(revertibleCommand);
                _currentDoneCommandStackIndex = _revertibleCommandsStack.Count - 1;
                UpdateCanUndoCanRedo();
            }
        }

        public void Redo()
        {
            lock (_lock)
            {
                if (!this.CanRedo)
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

                UpdateCanUndoCanRedo();
            }
        }

        public void Undo()
        {
            lock (_lock)
            {
                if (!this.CanUndo)
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

                UpdateCanUndoCanRedo();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Removes all commands following the currently focused command
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

        private void UpdateCanUndoCanRedo()
        {
            lock (_lock)
            {
                this.CanUndo = _currentDoneCommandStackIndex >= 0;
                this.CanRedo = _currentDoneCommandStackIndex < (_revertibleCommandsStack.Count - 1);
            }
        }

        #endregion

    }
}