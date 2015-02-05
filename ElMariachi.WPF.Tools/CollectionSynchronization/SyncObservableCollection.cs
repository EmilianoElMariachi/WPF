using System;
using System.Collections.ObjectModel;

namespace ElMariachi.WPF.Tools.CollectionSynchronization
{
    public class SyncObservableCollection<TViewModel, TModel> : AbstractSyncObservableCollection<TViewModel, TModel>
    {
        private readonly ViewModelBuilder _viewModelBuilder;

        public delegate TViewModel ViewModelBuilder(TModel tModel);

        public SyncObservableCollection(ObservableCollection<TModel> models, ViewModelBuilder viewModelBuilder)
            : base(models)
        {
            if (viewModelBuilder == null)
            {
                throw new ArgumentNullException("viewModelBuilder");
            }
            _viewModelBuilder = viewModelBuilder;

            this.Synchronize();
        }

        protected override TViewModel CreateViewModel(TModel tModel)
        {
            return _viewModelBuilder(tModel);
        }
    }
}