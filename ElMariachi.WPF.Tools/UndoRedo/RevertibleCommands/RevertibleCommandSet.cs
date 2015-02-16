using System.Collections.Generic;
using System.Linq;

namespace ElMariachi.WPF.Tools.UndoRedo.RevertibleCommands
{
    /// <summary>
    /// Set of revertible commands in a single <see cref="IRevertibleCommand"/>
    /// Use this class if you have several commands to execute in one Undo/Redo operation.
    /// </summary>
    public class RevertibleCommandSet : RevertibleCommand
    {

        #region Fields & Properties

        private readonly HashSet<IRevertibleCommand> _revertibleCommands = new HashSet<IRevertibleCommand>();

        /// <summary>
        /// Gets the set of <see cref="IRevertibleCommand"/> to be executed in one Undo/Redo operation
        /// </summary>
        public HashSet<IRevertibleCommand> CommandsSet
        {
            get { return _revertibleCommands; }
        }

        #endregion

        #region Constructors

        public RevertibleCommandSet()
        {
        }

        public RevertibleCommandSet(IEnumerable<IRevertibleCommand> revertibleCommands)
        {
            foreach (var revertibleCommand in revertibleCommands)
            {
                _revertibleCommands.Add(revertibleCommand);
            }
        }

        public RevertibleCommandSet(params IRevertibleCommand[] revertibleCommands) 
        {
            foreach (var revertibleCommand in revertibleCommands)
            {
                _revertibleCommands.Add(revertibleCommand);
            }
        }

        #endregion

        #region Methods

        public override void Do()
        {
            foreach (var revertibleCommand in CommandsSet)
            {
                revertibleCommand.Do();
            }
        }

        public override void Undo()
        {
            foreach (var revertibleCommand in CommandsSet.Reverse())
            {
                revertibleCommand.Undo();
            }
        }

        #endregion

    }
}