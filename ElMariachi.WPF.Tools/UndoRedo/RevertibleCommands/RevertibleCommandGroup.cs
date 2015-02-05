namespace ElMariachi.WPF.Tools.UndoRedo.RevertibleCommands
{
    /// <summary>
    /// Permet de regrouper un ensemble de commandes au sein d'une action Do / Redo
    /// </summary>
    public class RevertibleCommandGroup : AbstractRevertibleCommand
    {
        private readonly AbstractRevertibleCommand[] _revertibleCommands;

        public RevertibleCommandGroup(params AbstractRevertibleCommand[] revertibleCommands)
        {
            _revertibleCommands = revertibleCommands;
        }

        public override void Do()
        {
            foreach (var revertibleCommand in _revertibleCommands)
            {
                revertibleCommand.Do();
            }
        }

        public override void Undo()
        {
            foreach (var revertibleCommand in _revertibleCommands)
            {
                revertibleCommand.Undo();
            }
        }
    }
}