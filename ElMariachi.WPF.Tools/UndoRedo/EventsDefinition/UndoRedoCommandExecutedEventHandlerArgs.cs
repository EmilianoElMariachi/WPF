namespace ElMariachi.WPF.Tools.UndoRedo.EventsDefinition
{
    public class UndoRedoCommandExecutedEventHandlerArgs
    {
        internal UndoRedoCommandExecutedEventHandlerArgs(UndoRedoExecutionCategory undoRedoExecutionCategory)
        {
            UndoRedoCommandExecutedState = undoRedoExecutionCategory;
        }

        public UndoRedoExecutionCategory UndoRedoCommandExecutedState { get; private set; }
    }
}