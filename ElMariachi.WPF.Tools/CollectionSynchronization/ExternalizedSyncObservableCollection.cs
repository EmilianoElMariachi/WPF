using System;
using System.Collections.ObjectModel;

namespace ElMariachi.WPF.Tools.CollectionSynchronization
{

    /// <summary>
    /// Synchronize this <see cref="ObservableCollection{T}"/> of TViewModel whith a <see cref="ObservableCollection{T}"/> of TModel,
    /// simply by providing the TModel to TViewModel converter (see <see cref="ViewModelBuilder"/>).
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    public class ExternalizedSyncObservableCollection<TViewModel, TModel> : SyncObservableCollection<TViewModel, TModel>
    {

        #region Fields & Properties

        private readonly ViewModelBuilder _viewModelBuilder;

        public delegate TViewModel ViewModelBuilder(TModel tModel);

        #endregion

        #region Constructors

        public ExternalizedSyncObservableCollection(ObservableCollection<TModel> models, ViewModelBuilder viewModelBuilder)
            : base(models)
        {
            if (viewModelBuilder == null)
            {
                throw new ArgumentNullException("viewModelBuilder");
            }
            _viewModelBuilder = viewModelBuilder;

            this.Synchronize();
        }

        #endregion

        #region Methods

        protected override TViewModel CreateViewModel(TModel tModel)
        {
            return _viewModelBuilder(tModel);
        }

        #endregion

    }
}