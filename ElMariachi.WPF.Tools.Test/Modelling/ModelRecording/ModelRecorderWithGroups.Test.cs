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
    public class ModelRecorderWithGroups
    {

        private IModelRecorder _modelRecorder;
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
            [IsRecordableWithFilter(1000)]
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
            [IsRecordableWithFilter]
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
            [IsRecordableWithFilter]
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
            [IsRecordableWithFilter]
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
            private string _text;

            [IsRecordableWithFilter]
            internal ObservableCollection<object> Items
            {
                get { return _items; }
                set
                {
                    _items = value;
                    NotifyPropertyChanged("Items");
                }
            }

            [IsRecordableWithFilter]
            internal Point Point
            {
                get { return _point; }
                set
                {
                    _point = value;
                    NotifyPropertyChanged("Point");
                }
            }

            [IsRecordableWithFilter]
            internal string Text
            {
                get { return _text; }
                set
                {
                    _text = value;
                    NotifyPropertyChanged("Text");
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
        public void RecordGroup()
        {
            _modelRecorder.Record(_undoRedoService, _recordedModel);

            var initialBool = _recordedModel.IsTrue;
            var initialNumber = _recordedModel.Number;
            var initialSubClassA = _recordedModel.SubClassA;
            var initialText = _recordedModel.SubClassB.Text;

            using (_modelRecorder.GroupRecords())
            {
                _recordedModel.IsTrue = !_recordedModel.IsTrue;
                _recordedModel.Number++;
                _recordedModel.SubClassA = new ClassA();
                _recordedModel.SubClassB.Text += " new text";
            }

            _undoRedoService.Undo();

            Assert.AreEqual(initialBool, _recordedModel.IsTrue);
            Assert.AreEqual(initialNumber, _recordedModel.Number);
            Assert.AreSame(initialSubClassA, _recordedModel.SubClassA);
            Assert.AreEqual(initialText, _recordedModel.SubClassB.Text);
        }

    }

}

