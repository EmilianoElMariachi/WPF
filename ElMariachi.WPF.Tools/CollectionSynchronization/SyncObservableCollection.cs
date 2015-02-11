using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace ElMariachi.WPF.Tools.CollectionSynchronization
{

    /// <summary>
    /// Base implementation for synchronizing a <see cref="ObservableCollection{T}"/> of TViewModel whith a <see cref="ObservableCollection{T}"/> of TModel
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    public abstract class SyncObservableCollection<TViewModel, TModel> : ObservableCollection<TViewModel>
    {

        #region Fields & Properties

        public delegate TViewModel ViewModelFactoryHandler(TModel tModel);

        private readonly ObservableCollection<TModel> _models;

        #endregion

        #region Constructors

        public SyncObservableCollection(ObservableCollection<TModel> models)
        {
            if (models == null)
            {
                throw new ArgumentNullException("models");
            }

            _models = models;
            _models.CollectionChanged += OnTModelCollectionChanged;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clear this collection and resynchronize with items of the sync collection
        /// </summary>
        public void Synchronize()
        {
            this.Clear();
            foreach (var model in _models)
            {
                this.Add(CreateViewModel(model));
            }
        }

        private void OnTModelCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (var i = 0; i < e.NewItems.Count; i++)
                    {
                        this.Insert(e.NewStartingIndex + i, CreateViewModel((TModel) e.NewItems[i]));
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    if (e.OldItems.Count == 1)
                    {
                        this.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else
                    {
                        var items = this.Skip(e.OldStartingIndex).Take(e.OldItems.Count).ToList();
                        for (var i = 0; i < e.OldItems.Count; i++)
                        {
                            this.RemoveAt(e.OldStartingIndex);
                        }

                        for (var i = 0; i < items.Count; i++)
                        {
                            this.Insert(e.NewStartingIndex + i, items[i]);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    for (var i = 0; i < e.OldItems.Count; i++)
                    {
                        this.RemoveAt(e.OldStartingIndex);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    for (var i = 0; i < e.OldItems.Count; i++)
                    {
                        this.RemoveAt(e.OldStartingIndex);
                    }
                    goto case NotifyCollectionChangedAction.Add;

                case NotifyCollectionChangedAction.Reset:
                    Clear();
                    break;
            }
        }

        protected abstract TViewModel CreateViewModel(TModel tModel);

        #endregion


    }
}
