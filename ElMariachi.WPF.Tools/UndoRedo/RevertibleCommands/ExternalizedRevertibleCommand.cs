using System;

namespace ElMariachi.WPF.Tools.UndoRedo.RevertibleCommands
{
    public class ExternalizedRevertibleCommand : RevertibleCommand
    {
        private readonly Action _doAction;
        private readonly Action _undoAction;

        public ExternalizedRevertibleCommand(Action doAction, Action undoAction)
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