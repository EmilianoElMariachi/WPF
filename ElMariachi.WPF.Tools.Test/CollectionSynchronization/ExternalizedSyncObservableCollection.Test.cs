using System.Collections.ObjectModel;
using ElMariachi.WPF.Tools.CollectionSynchronization;
using NUnit.Framework;

namespace ElMariachi.WPF.Tools.Test.CollectionSynchronization
{
    [TestFixture]
    internal class ExternalizedSyncObservableCollectionTest
    {
        private SyncObservableCollection<ViewModel, Model> _viewModels;
        private ObservableCollection<Model> _models;

        private class Model
        {
        }

        private class ViewModel
        {
            private readonly Model _linkedModel;

            public ViewModel(Model linkedModel)
            {
                _linkedModel = linkedModel;
            }

            public Model LinkedModel
            {
                get { return _linkedModel; }
            }
        }


        [SetUp]
        public void Setup()
        {
            _models = new ObservableCollection<Model>();
            _viewModels = new ExternalizedSyncObservableCollection<ViewModel, Model>(_models, ViewModelBuilder);
        }

        private ViewModel ViewModelBuilder(Model tModel)
        {
            return new ViewModel(tModel);
        }

        [Test]
        public void ModelsCollectionAddition()
        {
            Assert.AreEqual(_models.Count, _viewModels.Count);

            var model = new Model();
            _models.Add(model);

            Assert.AreEqual(1, _viewModels.Count);
            Assert.AreSame(_viewModels[0].LinkedModel, model);
        }

        [Test]
        public void ModelsCollectionRemoval()
        {
            var model1 = new Model();
            _models.Add(model1);

            var model2 = new Model();
            _models.Add(model2);

            _models.Remove(model1);

            Assert.AreEqual(1, _viewModels.Count);
            Assert.AreSame(_viewModels[0].LinkedModel, model2);
        }

        [Test]
        public void ModelsCollectionInsertion()
        {
            Assert.AreEqual(0, _viewModels.Count);
            Assert.AreEqual(0, _models.Count);

            var model1 = new Model();
            _models.Insert(0, model1);

            var model2 = new Model();
            _models.Insert(0, model2);

            var model3 = new Model();
            _models.Insert(1, model3);

            //0 => model2
            //1 => model3
            //2 => model1

            Assert.AreEqual(3, _viewModels.Count);
            Assert.AreSame(model2, _viewModels[0].LinkedModel);
            Assert.AreSame(model3, _viewModels[1].LinkedModel);
            Assert.AreSame(model1, _viewModels[2].LinkedModel);
        }

        [Test]
        public void ModelsCollectionClear()
        {
            var model1 = new Model();
            _models.Add(model1);

            var model2 = new Model();
            _models.Add(model2);

            Assert.AreEqual(2, _viewModels.Count);

            _models.Clear();

            Assert.AreEqual(0, _viewModels.Count);
        }

        [Test]
        public void ModelsCollectionReplacement()
        {
            var model0 = new Model();
            _models.Add(model0);

            var viewModel0 = _viewModels[0];
            Assert.AreSame(model0, viewModel0.LinkedModel);

            var newModel0 = new Model();
            _models[0] = newModel0;
            var newViewModel0 = _viewModels[0];
            Assert.AreSame(newModel0, newViewModel0.LinkedModel);

            Assert.AreNotSame(viewModel0, newViewModel0);

            _models[0] = null;
            var newNewViewModel0 = _viewModels[0];
            Assert.IsNull(newNewViewModel0.LinkedModel);
        }

        [Test]
        public void ModelsCollectionMove()
        {
            var model0 = new Model();
            _models.Add(model0);

            var model1 = new Model();
            _models.Add(model1);

            var model2 = new Model();
            _models.Add(model2);

            _models.Move(2, 0);

            Assert.AreSame(model0, _viewModels[1].LinkedModel);
            Assert.AreSame(model1, _viewModels[2].LinkedModel);
            Assert.AreSame(model2, _viewModels[0].LinkedModel);
        }

    }


}
