namespace ElMariachi.WPF.Tools.UndoRedo.EventsDefinition
{
    public class BeforeUndoRedoCommandExecutedEventHandlerArgs
    {
        private readonly UndoRedoExecutionCategory _undoRedoExecutionCategory;

        public BeforeUndoRedoCommandExecutedEventHandlerArgs(UndoRedoExecutionCategory undoRedoExecutionCategory)
        {
            _undoRedoExecutionCategory = undoRedoExecutionCategory;
        }

        public UndoRedoExecutionCategory UndoRedoExecutionCategory
        {
            get { return _undoRedoExecutionCategory; }
        }
    }
}