using System;
using System.Collections.ObjectModel;
using System.Windows;
using ElMariachi.WPF.Tools.Modelling;
using ElMariachi.WPF.Tools.Modelling.ModelRecording;
using ElMariachi.WPF.Tools.Modelling.ModelRecording.Attributes;
using ElMariachi.WPF.Tools.UndoRedo;
using NUnit.Framework;

namespace ElMariachi.WPF.Tools.Test.Modelling.ModelRecording
{

    [TestFixture]
    public class ModelRecorderTest
    {
        private ModelRecorder _modelRecorder;
        private UndoRedoService _undoRedoService;
        private ClassA _recordedModel;

        private class ClassA : NotifyPropertyChangedImpl
        {
            private static int _intClassInstanceCounter = 0;

            public ClassA()
            {
                InstanceNumber = ++_intClassInstanceCounter;
            }

            public int InstanceNumber { get; private set; }

            private string _nonRecordedString;
            public string NonRecordedString
            {
                get { return _nonRecordedString; }
                set
                {
                    _nonRecordedString = value;
                    NotifyPropertyChanged("NonRecordedString");
                }
            }

            private bool _isTrue;
            [IsRecordable("Is True")]
            public bool IsTrue
            {
                get { return _isTrue; }
                set
                {
                    _isTrue = value;
                    NotifyPropertyChanged("IsTrue");
                }
            }

            private int _number;
            [IsRecordable("Number of I don't know what")]
            public int Number
            {
                get { return _number; }
                set
                {
                    _number = value;
                    NotifyPropertyChanged("Number");
                }
            }

            private ClassA _subClassA;
            [IsRecordable("SubClass A")]
            public ClassA SubClassA
            {
                get { return _subClassA; }
                set
                {
                    _subClassA = value;
                    NotifyPropertyChanged("SubClassA");
                }
            }

            private ClassB _subClassB;
            [IsRecordable("SubClass B")]
            public ClassB SubClassB
            {
                get { return _subClassB; }
                set
                {
                    _subClassB = value;
                    NotifyPropertyChanged("SubClassB");
                }
            }

            public override string ToString()
            {
                return this.GetType().Name + " : " + InstanceNumber;
            }
        }

        internal class ClassB : NotifyPropertyChangedImpl
        {
            private static int _intClassInstanceCounter = 0;

            public int InstanceNumber { get; private set; }

            public ClassB()
            {
                InstanceNumber = ++_intClassInstanceCounter;
            }

            private ObservableCollection<object> _items = new ObservableCollection<object>();
            private Point _point;

            [IsRecordable("Collection of Items of Class B")]
            internal ObservableCollection<object> Items
            {
                get { return _items; }
                set
                {
                    _items = value;
                    NotifyPropertyChanged("Items");
                }
            }

            [IsRecordable("Position of the message on the screen")]
            internal Point Point
            {
                get { return _point; }
                set
                {
                    _point = value;
                    NotifyPropertyChanged("Point");
                }
            }

            public override string ToString()
            {
                return this.GetType().Name + " : " + InstanceNumber;
            }

        }

        [SetUp]
        public void SetUp()
        {
            _modelRecorder = new ModelRecorder();
            _undoRedoService = new UndoRedoService();
            _recordedModel = new ClassA
            {
                SubClassA = new ClassA(),
                SubClassB = new ClassB(),
            };
        }

        [Test]
        public void UndoRedoServicePropertyIsTheOneGivenAtRecord()
        {
            Assert.IsNull(_modelRecorder.UndoRedoService);
            _modelRecorder.Record(_undoRedoService, new object());

            Assert.IsNotNull(_modelRecorder.UndoRedoService);
            Assert.AreSame(_undoRedoService, _modelRecorder.UndoRedoService);
        }

        [Test]
        public void RecordThrowsIfUndoRedoServiceIsNullOrModelToRecordIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _modelRecorder.Record(_undoRedoService, null));
            Assert.Throws<ArgumentNullException>(() => _modelRecorder.Record(null, _recordedModel));
        }

        [Test]
        public void IsRecordingPropertyTest()
        {
            Assert.IsFalse(_modelRecorder.IsRecording);
            _modelRecorder.Record(_undoRedoService, new object());
            Assert.IsTrue(_modelRecorder.IsRecording);
            _modelRecorder.Stop();
            Assert.IsFalse(_modelRecorder.IsRecording);
        }

        [Test]
        public void WhenStopIsCalledRecordingIsStopped()
        {
            Assert.IsFalse(_modelRecorder.IsRecording);
            _modelRecorder.Record(_undoRedoService, _recordedModel);

            Assert.IsTrue(_modelRecorder.IsRecording);
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(0, _undoRedoService.NbUndo);

            _recordedModel.Number++;
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(1, _undoRedoService.NbUndo);

            _recordedModel.SubClassA = new ClassA();
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(2, _undoRedoService.NbUndo);

            _modelRecorder.Stop();
            _recordedModel.Number++;
            _recordedModel.SubClassA = new ClassA();
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(2, _undoRedoService.NbUndo);
            Assert.IsFalse(_modelRecorder.IsRecording);
        }

        [Test]
        public void RecorderIgnoreNonRecordedAttributes()
        {
            _modelRecorder.Record(_undoRedoService, _recordedModel);

            _recordedModel.NonRecordedString += "Hello!";

            Assert.IsFalse(_undoRedoService.CanRedo);
            Assert.IsFalse(_undoRedoService.CanUndo);
        }

        [Test]
        public void BasicPropertyChangeIsRecorded()
        {
            _modelRecorder.Record(_undoRedoService, _recordedModel);

            var initialValue = _recordedModel.IsTrue;
            Assert.IsFalse(_undoRedoService.CanUndo);
            Assert.IsFalse(_undoRedoService.CanRedo);

            _recordedModel.IsTrue = !initialValue;
            Assert.IsTrue(_undoRedoService.CanUndo);
            Assert.IsFalse(_undoRedoService.CanRedo);

            _undoRedoService.Undo();
            Assert.AreEqual(_recordedModel.IsTrue, initialValue);
            Assert.IsFalse(_undoRedoService.CanUndo);
            Assert.IsTrue(_undoRedoService.CanRedo);

            _undoRedoService.Redo();
            Assert.AreEqual(_recordedModel.IsTrue, !initialValue);
            Assert.IsTrue(_undoRedoService.CanUndo);
            Assert.IsFalse(_undoRedoService.CanRedo);
        }

        [Test]
        public void SubmodelChange()
        {
            var initialClassA = _recordedModel.SubClassA;
            _modelRecorder.Record(_undoRedoService, _recordedModel);
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(0, _undoRedoService.NbUndo);

            var newSubClassA = new ClassA();
            _recordedModel.SubClassA = newSubClassA;
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(1, _undoRedoService.NbUndo);

            var newSubClassA2 = new ClassA();
            _recordedModel.SubClassA = newSubClassA2;
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(2, _undoRedoService.NbUndo);

            newSubClassA.Number++; //NOTE : instance removed from recorded model
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(2, _undoRedoService.NbUndo);

            var newNumber = ++newSubClassA2.Number;
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(3, _undoRedoService.NbUndo);

            _undoRedoService.Undo();
            Assert.AreEqual(newNumber - 1, newSubClassA2.Number);
            Assert.AreEqual(1, _undoRedoService.NbRedo);
            Assert.AreEqual(2, _undoRedoService.NbUndo);

            _undoRedoService.Undo();
            Assert.AreSame(newSubClassA, _recordedModel.SubClassA);
            Assert.AreEqual(2, _undoRedoService.NbRedo);
            Assert.AreEqual(1, _undoRedoService.NbUndo);

            _undoRedoService.Undo();
            Assert.AreSame(initialClassA, _recordedModel.SubClassA);
            Assert.AreEqual(3, _undoRedoService.NbRedo);
            Assert.AreEqual(0, _undoRedoService.NbUndo);
        }

        [Test]
        public void UndoSubModelChangeWithNewModifications()
        {
            var initialClassA = _recordedModel.SubClassA;
            _modelRecorder.Record(_undoRedoService, _recordedModel);
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(0, _undoRedoService.NbUndo);


            var subClassA = new ClassA();
            _recordedModel.SubClassA = subClassA;
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(1, _undoRedoService.NbUndo);

            _undoRedoService.Undo();
            Assert.AreEqual(1, _undoRedoService.NbRedo);
            Assert.AreEqual(0, _undoRedoService.NbUndo);
            Assert.AreSame(initialClassA, _recordedModel.SubClassA);

            initialClassA.Number++;
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(1, _undoRedoService.NbUndo);

            subClassA.Number++; //NOTE l'item est supprimé et ne doit plus engendrer de modification
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(1, _undoRedoService.NbUndo);
        }

        [Test]
        public void AddItemsToObservableCollection()
        {
            _modelRecorder.Record(_undoRedoService, _recordedModel);
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(0, _undoRedoService.NbUndo);

            var item1 = 5;
            _recordedModel.SubClassB.Items.Add(item1);
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(1, _undoRedoService.NbUndo);

            _undoRedoService.Undo();
            Assert.AreEqual(1, _undoRedoService.NbRedo);
            Assert.AreEqual(0, _undoRedoService.NbUndo);
            Assert.AreEqual(0, _recordedModel.SubClassB.Items.Count);

            _undoRedoService.Redo();
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(1, _undoRedoService.NbUndo);
            Assert.AreEqual(1, _recordedModel.SubClassB.Items.Count);
            Assert.AreEqual(item1, _recordedModel.SubClassB.Items[0]);
        }

        [Test]
        public void RemoveItemsFromObservableCollection()
        {
            _recordedModel.SubClassB.Items.Clear();
            var classAItem0 = new ClassA();
            _recordedModel.SubClassB.Items.Add(classAItem0);
            var classAItem1 = new ClassA();
            _recordedModel.SubClassB.Items.Add(classAItem1);
            var numberItem2 = 5;
            _recordedModel.SubClassB.Items.Add(numberItem2);

            _modelRecorder.Record(_undoRedoService, _recordedModel);
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(0, _undoRedoService.NbUndo);

            Assert.IsTrue(_recordedModel.SubClassB.Items.Remove(classAItem0));
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(1, _undoRedoService.NbUndo);

            Assert.IsTrue(_recordedModel.SubClassB.Items.Remove(classAItem1));
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(2, _undoRedoService.NbUndo);

            Assert.IsTrue(_recordedModel.SubClassB.Items.Remove(numberItem2));
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(3, _undoRedoService.NbUndo);

            _undoRedoService.Undo();
            Assert.AreEqual(1, _undoRedoService.NbRedo);
            Assert.AreEqual(2, _undoRedoService.NbUndo);
            Assert.AreEqual(1, _recordedModel.SubClassB.Items.Count);
            Assert.AreEqual(numberItem2, _recordedModel.SubClassB.Items[0]);

            _undoRedoService.Undo();
            Assert.AreEqual(2, _undoRedoService.NbRedo);
            Assert.AreEqual(1, _undoRedoService.NbUndo);
            Assert.AreEqual(2, _recordedModel.SubClassB.Items.Count);
            Assert.AreEqual(classAItem1, _recordedModel.SubClassB.Items[0]);
            Assert.AreEqual(numberItem2, _recordedModel.SubClassB.Items[1]);

            _undoRedoService.Undo();
            Assert.AreEqual(3, _undoRedoService.NbRedo);
            Assert.AreEqual(0, _undoRedoService.NbUndo);
            Assert.AreEqual(3, _recordedModel.SubClassB.Items.Count);
            Assert.AreEqual(classAItem0, _recordedModel.SubClassB.Items[0]);
            Assert.AreEqual(classAItem1, _recordedModel.SubClassB.Items[1]);
            Assert.AreEqual(numberItem2, _recordedModel.SubClassB.Items[2]);
        }

        [Test]
        public void ChangingPropertyOfARemovedCollectionItem()
        {
            _recordedModel.SubClassB.Items.Clear();
            var classAItem0 = new ClassA();
            _recordedModel.SubClassB.Items.Add(classAItem0);
            var classAItem1 = new ClassA();
            _recordedModel.SubClassB.Items.Add(classAItem1);

            _modelRecorder.Record(_undoRedoService, _recordedModel);
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(0, _undoRedoService.NbUndo);

            _recordedModel.SubClassB.Items.Remove(classAItem0);
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(1, _undoRedoService.NbUndo);

            classAItem0.Number++; //NOTE : l'item n'est plus dans la collection
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(1, _undoRedoService.NbUndo);
            Assert.AreEqual(1, _recordedModel.SubClassB.Items.Count);

            _undoRedoService.Undo();
            Assert.AreEqual(1, _undoRedoService.NbRedo);
            Assert.AreEqual(0, _undoRedoService.NbUndo);
            Assert.AreEqual(2, _recordedModel.SubClassB.Items.Count);

            classAItem0.Number++; //NOTE : l'item est de nouveau dans la collection
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(1, _undoRedoService.NbUndo);
            Assert.AreEqual(2, _recordedModel.SubClassB.Items.Count);
        }

        [Test]
        public void ChangePropertiesOfCollectionItem()
        {
            var classAItem0 = new ClassA();
            _recordedModel.SubClassB.Items.Clear();
            _recordedModel.SubClassB.Items.Add(classAItem0);

            _modelRecorder.Record(_undoRedoService, _recordedModel);
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(0, _undoRedoService.NbUndo);

            var classAItem1 = new ClassA();
            _recordedModel.SubClassB.Items.Add(classAItem1);
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(1, _undoRedoService.NbUndo);

            classAItem0.Number++;
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(2, _undoRedoService.NbUndo);

            classAItem1.Number++;
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(3, _undoRedoService.NbUndo);
        }

        [Test]
        public void ReplaceCollectionItem()
        {
            _recordedModel.SubClassB.Items.Clear();
            var item0 = new ClassA();
            _recordedModel.SubClassB.Items.Add(item0);
            var item1 = "Hello";
            _recordedModel.SubClassB.Items.Add(item1);

            _modelRecorder.Record(_undoRedoService, _recordedModel);
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(0, _undoRedoService.NbUndo);

            //NOTE :
            _recordedModel.SubClassB.Items[0] = _recordedModel.SubClassB.Items[0];
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(1, _undoRedoService.NbUndo);

            var obj = new object();
            _recordedModel.SubClassB.Items[0] = obj;
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(2, _undoRedoService.NbUndo);

            var classB = new ClassB();
            _recordedModel.SubClassB.Items[1] = classB;
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(3, _undoRedoService.NbUndo);

            classB.Point = new Point(0, 5);
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(4, _undoRedoService.NbUndo);

            _recordedModel.SubClassB.Items[1] = new ClassA();
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(5, _undoRedoService.NbUndo);

            _undoRedoService.Undo(); //NOTE : Restaure classB à l'index 1
            Assert.AreEqual(1, _undoRedoService.NbRedo);
            Assert.AreEqual(4, _undoRedoService.NbUndo);
            Assert.AreSame(classB, _recordedModel.SubClassB.Items[1]);

            classB.Point = new Point();
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(5, _undoRedoService.NbUndo);
        }

        [Test]
        public void MoveCollectionItems()
        {
            _recordedModel.SubClassB.Items.Clear();
            var item0 = "Hello1";
            _recordedModel.SubClassB.Items.Add(item0);
            var item1 = "Hello2";
            _recordedModel.SubClassB.Items.Add(item1);
            var item2 = "Hello3";
            _recordedModel.SubClassB.Items.Add(item2);

            _modelRecorder.Record(_undoRedoService, _recordedModel);
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(0, _undoRedoService.NbUndo);

            _recordedModel.SubClassB.Items.Move(2, 0);
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(1, _undoRedoService.NbUndo);
            Assert.AreEqual(item2, _recordedModel.SubClassB.Items[0]);
            Assert.AreEqual(item0, _recordedModel.SubClassB.Items[1]);
            Assert.AreEqual(item1, _recordedModel.SubClassB.Items[2]);

            _undoRedoService.Undo();
            Assert.AreEqual(1, _undoRedoService.NbRedo);
            Assert.AreEqual(0, _undoRedoService.NbUndo);
            Assert.AreEqual(item0, _recordedModel.SubClassB.Items[0]);
            Assert.AreEqual(item1, _recordedModel.SubClassB.Items[1]);
            Assert.AreEqual(item2, _recordedModel.SubClassB.Items[2]);

            _undoRedoService.Redo();
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(1, _undoRedoService.NbUndo);
            Assert.AreEqual(item2, _recordedModel.SubClassB.Items[0]);
            Assert.AreEqual(item0, _recordedModel.SubClassB.Items[1]);
            Assert.AreEqual(item1, _recordedModel.SubClassB.Items[2]);
        }

        [Test]
        public void ChangingPropertyOfAMovedCollectionItem()
        {
            _recordedModel.SubClassB.Items.Clear();
            var item0 = new ClassA();
            _recordedModel.SubClassB.Items.Add(item0);
            var item1 = new ClassB();
            _recordedModel.SubClassB.Items.Add(item1);

            _modelRecorder.Record(_undoRedoService, _recordedModel);
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(0, _undoRedoService.NbUndo);

            _recordedModel.SubClassB.Items.Move(1, 0);
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(1, _undoRedoService.NbUndo);

            _undoRedoService.Undo();
            Assert.AreEqual(1, _undoRedoService.NbRedo);
            Assert.AreEqual(0, _undoRedoService.NbUndo);

            item1.Point = new Point(100, 200);
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(1, _undoRedoService.NbUndo);

            _recordedModel.SubClassB.Items.Move(1, 0);
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(2, _undoRedoService.NbUndo);

            item1.Point = new Point(100, 200);
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(3, _undoRedoService.NbUndo);
        }

        [Test]
        public void ClearCollection()
        {
            _recordedModel.SubClassB.Items.Clear();
            var item0 = new ClassA();
            _recordedModel.SubClassB.Items.Add(item0);
            var item1 = new ClassB();
            _recordedModel.SubClassB.Items.Add(item1);
            var item2 = "Hello";
            _recordedModel.SubClassB.Items.Add(item2);

            _modelRecorder.Record(_undoRedoService, _recordedModel);
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(0, _undoRedoService.NbUndo);

            _recordedModel.SubClassB.Items.Clear();
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(1, _undoRedoService.NbUndo);
            Assert.AreEqual(0, _recordedModel.SubClassB.Items.Count);

            item0.IsTrue = !item0.IsTrue;
            item1.Point = new Point();
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(1, _undoRedoService.NbUndo);
            Assert.AreEqual(0, _recordedModel.SubClassB.Items.Count);


            _modelRecorder.UndoRedoService.Undo();
            Assert.AreEqual(1, _undoRedoService.NbRedo);
            Assert.AreEqual(0, _undoRedoService.NbUndo);
            Assert.AreEqual(3, _recordedModel.SubClassB.Items.Count);
        }

        [Test]
        public void ChangingCollection()
        {
            var initialCollection = _recordedModel.SubClassB.Items;
            _recordedModel.SubClassB.Items.Clear();
            var item0 = new ClassA();
            _recordedModel.SubClassB.Items.Add(item0);
            var item1 = new ClassB();
            _recordedModel.SubClassB.Items.Add(item1);

            _modelRecorder.Record(_undoRedoService, _recordedModel);
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(0, _undoRedoService.NbUndo);

            var newCollection = new ObservableCollection<object> { new ClassA(), new object(), 25, new Point() };
            _recordedModel.SubClassB.Items = newCollection;
            Assert.AreEqual(0, _undoRedoService.NbRedo);
            Assert.AreEqual(1, _undoRedoService.NbUndo);
            Assert.AreEqual(4, _recordedModel.SubClassB.Items.Count);

            _modelRecorder.UndoRedoService.Undo();
            Assert.AreEqual(1, _undoRedoService.NbRedo);
            Assert.AreEqual(0, _undoRedoService.NbUndo);
            Assert.AreSame(initialCollection, _recordedModel.SubClassB.Items);
            Assert.AreEqual(2, _recordedModel.SubClassB.Items.Count);

            _modelRecorder.UndoRedoService.Redo();
            Assert.AreSame(newCollection, _recordedModel.SubClassB.Items);
            Assert.AreEqual(4, _recordedModel.SubClassB.Items.Count);
        }

    }


}

