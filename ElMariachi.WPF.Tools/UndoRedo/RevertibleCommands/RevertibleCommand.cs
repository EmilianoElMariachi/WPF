using System.Runtime.Serialization;

namespace ElMariachi.WPF.Tools.UndoRedo.RevertibleCommands
{
    public abstract class RevertibleCommand : IRevertibleCommand
    {

        #region Fields & Properties

        private static readonly ObjectIDGenerator _objectIdGenerator = new ObjectIDGenerator();

        public long Id { get; private set; }

        public string Description { get; protected set; }

        #endregion

        #region Constructors

        public RevertibleCommand()
        {
            bool firstTime;
            this.Id = _objectIdGenerator.GetId(this, out firstTime);
        }

        #endregion

        #region Methods

        public abstract void Do();

        public abstract void Undo();

        public override string ToString()
        {
            return "Revertible Command N°" + this.Id;
        }

        #endregion

    }
}