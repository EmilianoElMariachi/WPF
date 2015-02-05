using System.Collections.Generic;
using System.Linq;

namespace ElMariachi.WPF.Tools.UndoRedo.RevertibleCommands.Helpers
{
    /// <summary>
    /// Classe permettant de gérer la suppression d'éléments d'une liste, de façon révertible.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RemoveListItemsRevertibleCommand<T> : AbstractRevertibleCommand
    {

        #region Fields & Properties

        private readonly IList<T> _collection;
        private readonly IEnumerable<T> _itemsToRemove;
        private readonly Dictionary<T, int> _deletedItemsPositions = new Dictionary<T, int>();

        #endregion

        #region Constructor(s)

        public RemoveListItemsRevertibleCommand(IList<T> collection, IEnumerable<T> itemsToRemove, string description = null)
        {
            _collection = collection;
            _itemsToRemove = itemsToRemove.ToArray();
            this.Description = description;
        }

        #endregion

        #region Methods

        public override void Do()
        {
            _deletedItemsPositions.Clear();
            foreach (var item in _itemsToRemove)
            {
                var index = _collection.IndexOf(item);
                _deletedItemsPositions.Add(item, index);
                _collection.RemoveAt(index);
            }
        }

        public override void Undo()
        {
            foreach (var keyValuePair in _deletedItemsPositions.Reverse())
            {
                _collection.Insert(keyValuePair.Value, keyValuePair.Key);
            }
        }

        #endregion

    }
}