using System;

namespace ElMariachi.WPF.Tools.UndoRedo.RevertibleCommands
{
    public class RevertibleCommand : AbstractRevertibleCommand
    {
        private readonly Action _doAction;
        private readonly Action _undoAction;

        public RevertibleCommand(Action doAction, Action undoAction)
        {
            _doAction = doAction;
            _undoAction = undoAction;
        }

        public override void Do()
        {
            _doAction();
        }

        public override void Undo()
        {
            _undoAction();
        }
    }
}