namespace ElMariachi.WPF.Tools.UndoRedo.EventsDefinition
{
    public class CanRedoChangedEventHandlerArgs
    {
        public bool CanRedo { get; private set; }

        public CanRedoChangedEventHandlerArgs(bool canRedo)
        {
            CanRedo = canRedo;
        }
    }
}