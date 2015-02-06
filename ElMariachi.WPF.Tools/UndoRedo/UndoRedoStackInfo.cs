using ElMariachi.WPF.Tools.UndoRedo.RevertibleCommands;

namespace ElMariachi.WPF.Tools.UndoRedo
{
    public class UndoRedoStackInfo
    {

        #region Fields & Properties

        /// <summary>
        /// The unique id of the executed <see cref="IRevertibleCommand"/>
        /// </summary>
        public long RevertibleCommandId { get; private set; }

        /// <summary>
        /// Description of the <see cref="IRevertibleCommand"/>
        /// </summary>
        public string Description { get; private set; }

        #endregion

        #region Constructors

        public UndoRedoStackInfo(long revertibleCommandId, string description)
        {
            Description = description;
            RevertibleCommandId = revertibleCommandId;
        }

        #endregion

    }
}