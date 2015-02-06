namespace ElMariachi.WPF.Tools.UndoRedo.EventsDefinition
{
    public class CanUndoChangedEventHandlerArgs 
    {
        public bool CanUndo { get; private set; }

        public CanUndoChangedEventHandlerArgs(bool canUndo)
        {
            CanUndo = canUndo;
        }
    }
}