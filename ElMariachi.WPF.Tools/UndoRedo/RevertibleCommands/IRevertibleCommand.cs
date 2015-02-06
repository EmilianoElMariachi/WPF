namespace ElMariachi.WPF.Tools.UndoRedo.RevertibleCommands
{
    /// <summary>
    /// Interface representing a revertible action
    /// Implement <see cref="Do"/> to perform a change and implement <see cref="Undo"/> to revert this change
    /// </summary>
    public interface IRevertibleCommand
    {

        /// <summary>
        /// Gets the unique ID of this Revertible command among all instantiated commands
        /// </summary>
        long Id { get; }

        /// <summary>
        /// Gets a description of the action performed by this command when <see cref="Do"/> or <see cref="Undo"/> is called
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Executes the command
        /// </summary>
        void Do();

        /// <summary>
        /// Reverts the action performed by the <see cref="Do"/> method
        /// </summary>
        void Undo();

    }
}