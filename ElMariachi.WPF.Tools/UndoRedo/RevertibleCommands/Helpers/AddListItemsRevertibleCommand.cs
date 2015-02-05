using System.Collections.Generic;
using System.Linq;

namespace ElMariachi.WPF.Tools.UndoRedo.RevertibleCommands.Helpers
{
    public class AddListItemsRevertibleCommand<T> : AbstractRevertibleCommand
    {

        #region Fields & Properties

        private readonly IList<T> _collection;
        private readonly IEnumerable<T> _itemsToAdd;

        #endregion

        #region Constructor(s)

        public AddListItemsRevertibleCommand(IList<T> collection, IEnumerable<T> itemsToAdd, string description = null)
        {
            _collection = collection;
            _itemsToAdd = itemsToAdd.ToArray();
            this.Description = description;
        }

        #endregion

        #region Methods

        public override void Do()
        {
            foreach (var item in _itemsToAdd)
            {
                _collection.Add(item);
            }
        }

        public override void Undo()
        {
            foreach (var item in _itemsToAdd.Reverse())
            {
                _collection.Remove(item);
            }
        }

        #endregion

    }
}