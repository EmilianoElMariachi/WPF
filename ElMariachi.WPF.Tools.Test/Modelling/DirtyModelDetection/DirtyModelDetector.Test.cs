using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using ElMariachi.WPF.Tools.Modelling.DirtyModelDetection;
using ElMariachi.WPF.Tools.Modelling.DirtyModelDetection.Attributes;
using ElMariachi.WPF.Tools.Modelling.DirtyModelDetection.EventsDefinition;
using NUnit.Framework;

namespace ElMariachi.WPF.Tools.Test.Modelling.DirtyModelDetection
{

    [TestFixture]
    public class DirtyModelDetectorTest
    {

        private DirtyModelDetector _modelDirtyDetector;
        private ClassA _testingClassA;

        private class NotifyPropertyChangedImpl : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            protected void NotifyPropertyChanged(string propertyName)
            {
                var handler = PropertyChanged;
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private class ClassA : NotifyPropertyChangedImpl
        {

            public ClassA this[ClassA classA]
            {
                get
                {
                    return null;
                }
            }

            private bool _bool;

            [IsSerialized]
            public bool Bool
            {
                get { return _bool; }
                set
                {
                    _bool = value;
                    NotifyPropertyChanged("Bool");
                }
            }

            private int _number;

            [IsSerialized]
            public int Number
            {
                get { return _number; }
                set
                {
                    _number = value;
                    NotifyPropertyChanged("Number");
                }
            }

            private string _text = "COOL TEXT";

            [IsSerialized]
            public string Text
            {
                get { return _text; }
                set
                {
                    _text = value;
                    NotifyPropertyChanged("Text");
                }
            }

            private ClassB _classB;

            [IsSerialized]
            public ClassB ClassB
            {
                get { return _classB; }
                set
                {
                    _classB = value;
                    NotifyPropertyChanged("ClassB");
                }
            }

            private ObservableCollection<object> _items;

            [IsSerialized]
            public ObservableCollection<object> Items
            {
                get { return _items; }
                set
                {
                    _items = value;
                    NotifyPropertyChanged("Items");
                }
            }
        }

        private class ClassB : NotifyPropertyChangedImpl
        {
            private string _text;

            [IsSerialized]
            public string Text
            {
                get { return _text; }
                set
                {
                    _text = value;
                    NotifyPropertyChanged("Text");
                }
            }

            private ClassA _classA;
            [IsSerialized]
            public ClassA ClassA
            {
                get { return _classA; }
                set
                {
                    _classA = value;
                    NotifyPropertyChanged("ClassA");
                }
            }

            private ObservableCollection<ClassA> _classesA;
            [IsSerialized]
            public ObservableCollection<ClassA> ClassesA
            {
                get { return _classesA; }
                set
                {
                    _classesA = value;
                    NotifyPropertyChanged("ClassesA");
                }
            }

        }

        private ClassA CreateNewClassA()
        {
            var classA = new ClassA
            {
                ClassB = CreateNewClassB(),
                Items = new ObservableCollection<object>(),
            };

            return classA;
        }

        private ClassB CreateNewClassB()
        {
            var classB = new ClassB
            {
                ClassesA = new ObservableCollection<ClassA>(),
                Text = "I'm a the Text of \"" + typeof(ClassB).Name + "\""
            };

            return classB;
        }

        [SetUp]
        public void SetUp()
        {
            _modelDirtyDetector = new DirtyModelDetector();
            _testingClassA = CreateNewClassA();
        }

        [TestCase(null)]
        [TestCase("OK")]
        public void RootModelStringValueChange(string initialText)
        {
            _testingClassA.Text = initialText;

            _modelDirtyDetector.Start(_testingClassA);
            Assert.IsFalse(_modelDirtyDetector.IsDirty);

            _testingClassA.Text += "ok";
            Assert.IsTrue(_modelDirtyDetector.IsDirty);

            _testingClassA.Text = initialText;
            Assert.IsFalse(_modelDirtyDetector.IsDirty);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void RootModelBoolValueChange(bool initialBool)
        {
            _testingClassA.Bool = initialBool;

            _modelDirtyDetector.Start(_testingClassA);
            Assert.IsFalse(_modelDirtyDetector.IsDirty);

            _testingClassA.Bool = !_testingClassA.Bool;
            Assert.IsTrue(_modelDirtyDetector.IsDirty);

            _testingClassA.Bool = initialBool;
            Assert.IsFalse(_modelDirtyDetector.IsDirty);
        }

        [Test]
        public void RootModelClassPropertyFromNullToSameInstance()
        {

            _modelDirtyDetector.Start(_testingClassA);
            Assert.IsFalse(_modelDirtyDetector.IsDirty);

            var originalBInstance = _testingClassA.ClassB;
            Assert.IsNotNull(originalBInstance);

            _testingClassA.ClassB = null;
            Assert.IsTrue(_modelDirtyDetector.IsDirty);

            _testingClassA.ClassB = originalBInstance;
            Assert.IsFalse(_modelDirtyDetector.IsDirty);
        }

        [Test]
        public void ChangingPropertyOfAReplacedInstance()
        {
            var originalClassB = _testingClassA.ClassB;
            var originalClassBText = _testingClassA.ClassB.Text;
            Assert.IsNotNull(originalClassB);

            _modelDirtyDetector.Start(_testingClassA);
            Assert.IsFalse(_modelDirtyDetector.IsDirty);

            _testingClassA.ClassB = CreateNewClassB();
            Assert.IsFalse(_modelDirtyDetector.IsDirty);

            originalClassB.Text += "OK";
            Assert.IsFalse(_modelDirtyDetector.IsDirty);

            _testingClassA.ClassB.Text += "OK";
            Assert.IsTrue(_modelDirtyDetector.IsDirty);

            _testingClassA.ClassB = originalClassB;
            Assert.IsTrue(_modelDirtyDetector.IsDirty);

            _testingClassA.ClassB.Text = originalClassBText;
            Assert.IsFalse(_modelDirtyDetector.IsDirty);
        }

        [Test]
        public void ChangingPropertyOfAnInstanceReplacedWithNull()
        {
            var originalClassB = _testingClassA.ClassB;
            Assert.IsNotNull(originalClassB);

            _modelDirtyDetector.Start(_testingClassA);
            Assert.IsFalse(_modelDirtyDetector.IsDirty);

            _testingClassA.ClassB = null;
            Assert.IsTrue(_modelDirtyDetector.IsDirty);
            originalClassB.Text += "OK";

            _testingClassA.ClassB = CreateNewClassB();
            Assert.IsFalse(_modelDirtyDetector.IsDirty);
        }

        [Test]
        public void TwoLevelIntricatePropertyChange()
        {
            var secondLevelClassA = new ClassA();
            _testingClassA.ClassB.ClassA = secondLevelClassA;

            _modelDirtyDetector.Start(_testingClassA);
            Assert.IsFalse(_modelDirtyDetector.IsDirty);

            secondLevelClassA.Number++;
            Assert.IsTrue(_modelDirtyDetector.IsDirty);

            _testingClassA.ClassB = CreateNewClassB();
            _testingClassA.ClassB.ClassA = new ClassA();
            Assert.IsFalse(_modelDirtyDetector.IsDirty);

            secondLevelClassA.Number++;
            Assert.IsFalse(_modelDirtyDetector.IsDirty);
        }

        [Test]
        public void RootModelClassPropertyFromNullToAnotherInstanceWithSameProperties()
        {

            _modelDirtyDetector.Start(_testingClassA);
            Assert.IsFalse(_modelDirtyDetector.IsDirty);

            var originalBInstance = _testingClassA.ClassB;
            Assert.IsNotNull(originalBInstance);
            var originalBText = originalBInstance.Text;

            var newClassB = CreateNewClassB();
            _testingClassA.ClassB = newClassB;
            Assert.IsFalse(_modelDirtyDetector.IsDirty);

            _testingClassA.ClassB = originalBInstance;
            Assert.IsFalse(_modelDirtyDetector.IsDirty);

            newClassB.Text = originalBText + "ok";
            _testingClassA.ClassB = newClassB;
            Assert.IsTrue(_modelDirtyDetector.IsDirty);

            newClassB.Text = originalBText;
            Assert.IsFalse(_modelDirtyDetector.IsDirty);
        }

        [Test]
        public void ObservableCollectionSetFromNullToNewInstance()
        {
            _testingClassA.ClassB.ClassesA = null;
            Assert.IsNull(_testingClassA.ClassB.ClassesA);

            _modelDirtyDetector.Start(_testingClassA);
            Assert.IsFalse(_modelDirtyDetector.IsDirty);

            _testingClassA.ClassB.ClassesA = new ObservableCollection<ClassA>();
            Assert.IsTrue(_modelDirtyDetector.IsDirty);

            _testingClassA.ClassB.ClassesA = null;
            Assert.IsFalse(_modelDirtyDetector.IsDirty);
        }

        [Test]
        public void RemovingAndReAddingItemFromObservableCollection()
        {
            var item = new ClassA();
            _testingClassA.ClassB.ClassesA = new ObservableCollection<ClassA> { item };

            _modelDirtyDetector.Start(_testingClassA);
            Assert.IsFalse(_modelDirtyDetector.IsDirty);

            Assert.IsTrue(_testingClassA.ClassB.ClassesA.Remove(item));
            Assert.IsTrue(_modelDirtyDetector.IsDirty);

            _testingClassA.ClassB.ClassesA.Add(item);
            Assert.IsFalse(_modelDirtyDetector.IsDirty);
        }

        [Test]
        public void ChangingPropertyOfAReplacedObservableCollectionItem()
        {
            var item = new ClassA();
            _testingClassA.ClassB.ClassesA = new ObservableCollection<ClassA> { item };

            _modelDirtyDetector.Start(_testingClassA);
            Assert.IsFalse(_modelDirtyDetector.IsDirty);

            Assert.IsTrue(_testingClassA.ClassB.ClassesA.Remove(item));
            Assert.IsTrue(_modelDirtyDetector.IsDirty);

            _testingClassA.ClassB.ClassesA.Add(item);
            Assert.IsFalse(_modelDirtyDetector.IsDirty);
        }

        [Test]
        public void ReplacingObservableCollectionValueTypeItem()
        {
            _testingClassA.Items = new ObservableCollection<object> { 5, 6 };

            _modelDirtyDetector.Start(_testingClassA);
            Assert.IsFalse(_modelDirtyDetector.IsDirty);

            _testingClassA.Items[0] = 5;
            Assert.IsFalse(_modelDirtyDetector.IsDirty);

            _testingClassA.Items[1] = 6;
            Assert.IsFalse(_modelDirtyDetector.IsDirty);

            _testingClassA.Items[1] = 8;
            Assert.IsTrue(_modelDirtyDetector.IsDirty);

            _testingClassA.Items[1] = 6;
            Assert.IsFalse(_modelDirtyDetector.IsDirty);
        }

        [Test]
        public void ReplaceComplexObservableCollectionItem()
        {
            var classAitem0 = CreateNewClassA();
            var classAitem0ClassBText = classAitem0.ClassB.Text;
            _testingClassA.Items = new ObservableCollection<object>
            {
                classAitem0,
                CreateNewClassA(),
                null
            };

            _modelDirtyDetector.Start(_testingClassA);
            Assert.IsFalse(_modelDirtyDetector.IsDirty);

            _testingClassA.Items[0] = null;
            Assert.IsTrue(_modelDirtyDetector.IsDirty);

            _testingClassA.Items[0] = CreateNewClassA();
            Assert.IsFalse(_modelDirtyDetector.IsDirty);

            _testingClassA.Items[0] = new Point();
            Assert.IsTrue(_modelDirtyDetector.IsDirty);

            _testingClassA.Items[0] = CreateNewClassA();
            Assert.IsFalse(_modelDirtyDetector.IsDirty);

            classAitem0.Number = 15;
            classAitem0.ClassB.Text += "Dirty";
            Assert.IsFalse(_modelDirtyDetector.IsDirty);

            var newClassA = CreateNewClassA();
            newClassA.ClassB.Text += "Changed";
            _testingClassA.Items[0] = newClassA;
            Assert.IsTrue(_modelDirtyDetector.IsDirty);

            newClassA.ClassB.Text = classAitem0ClassBText;
            Assert.IsFalse(_modelDirtyDetector.IsDirty);
        }

        [Test]
        public void RemoveAddComplexObservableCollectionItem()
        {
            _testingClassA.Items = new ObservableCollection<object>();
            var classA = CreateNewClassA();
            _testingClassA.Items.Add(classA);
            var classB = CreateNewClassB();
            _testingClassA.Items.Add(classB);

            _modelDirtyDetector.Start(_testingClassA);
            Assert.IsFalse(_modelDirtyDetector.IsDirty);

            _testingClassA.Items.Remove(classA);
            Assert.IsTrue(_modelDirtyDetector.IsDirty);

            _testingClassA.Items.Insert(0, classA);
            Assert.IsFalse(_modelDirtyDetector.IsDirty);

            _testingClassA.Items.Insert(2, classA);
            Assert.IsTrue(_modelDirtyDetector.IsDirty);

            classA.Bool = !classA.Bool;

            _testingClassA.Items.RemoveAt(2);
            Assert.IsTrue(_modelDirtyDetector.IsDirty);

            classA.Bool = !classA.Bool;
            Assert.IsFalse(_modelDirtyDetector.IsDirty);
        }

        [Test]
        public void ReplacingObservableCollection()
        {
            _testingClassA.Items = new ObservableCollection<object>
            {
                CreateNewClassA(),
                false
            };

            _modelDirtyDetector.Start(_testingClassA);
            Assert.IsFalse(_modelDirtyDetector.IsDirty);

            _testingClassA.Items = new ObservableCollection<object>();
            Assert.IsTrue(_modelDirtyDetector.IsDirty);

            var newClassA = CreateNewClassA();
            newClassA.Number++;
            _testingClassA.Items.Add(newClassA);
            _testingClassA.Items.Add(CreateNewClassA());
            Assert.IsTrue(_modelDirtyDetector.IsDirty);

            _testingClassA.Items[1] = false;
            Assert.IsTrue(_modelDirtyDetector.IsDirty);

            newClassA.Number--;
            Assert.IsFalse(_modelDirtyDetector.IsDirty);
        }

        [Test]
        public void StartThrowsIfCalledWithNull()
        {
            Assert.Throws<ArgumentNullException>(() => _modelDirtyDetector.Start(null));
        }

        [Test]
        public void RestartThrowsIfNeverStartedBefore()
        {
            Assert.Throws<InvalidOperationException>(() => _modelDirtyDetector.Restart());
        }

        [Test]
        public void RestartIgnoreModifiedProperties()
        {
            _modelDirtyDetector.Start(_testingClassA);

            var originalNumber = _testingClassA.Number;
            _testingClassA.Number++;
            Assert.IsTrue(_modelDirtyDetector.IsDirty);

            _modelDirtyDetector.Restart();
            Assert.IsFalse(_modelDirtyDetector.IsDirty);

            _testingClassA.Number = originalNumber;
            Assert.IsTrue(_modelDirtyDetector.IsDirty);

            _testingClassA.Number++;
            Assert.IsFalse(_modelDirtyDetector.IsDirty);
        }

        [Test]
        public void DirtyStateEventIsRaised()
        {
            object eventSender = null;
            DirtyStateChangedEventHandlerArgs dirtyStateChangedEventHandlerArgs = null;
            var nbCalls = 0;
            _modelDirtyDetector.DirtyStateChanged += (sender, args) =>
            {
                eventSender = sender;
                dirtyStateChangedEventHandlerArgs = args;
                nbCalls++;
            };

            _modelDirtyDetector.Start(_testingClassA);
            Assert.AreEqual(nbCalls, 0);
            Assert.IsNull(eventSender);
            Assert.IsNull(dirtyStateChangedEventHandlerArgs);

            _testingClassA.Bool = !_testingClassA.Bool;
            Assert.AreEqual(nbCalls, 1);
            Assert.AreSame(_modelDirtyDetector, eventSender);
            Assert.IsNotNull(dirtyStateChangedEventHandlerArgs);
            Assert.IsTrue(dirtyStateChangedEventHandlerArgs.IsDirty);
            Assert.AreSame(dirtyStateChangedEventHandlerArgs.ObservedModel, _testingClassA);

            _testingClassA.Bool = !_testingClassA.Bool;
            Assert.AreEqual(nbCalls, 2);
            Assert.AreSame(_modelDirtyDetector, eventSender);
            Assert.IsNotNull(dirtyStateChangedEventHandlerArgs);
            Assert.AreSame(dirtyStateChangedEventHandlerArgs.ObservedModel, _testingClassA);
            Assert.IsFalse(dirtyStateChangedEventHandlerArgs.IsDirty);
        }

        [Test]
        public void StopCancelDetectionChangesAndResetDirtyStateToFalse()
        {
            _modelDirtyDetector.Start(_testingClassA);
            Assert.IsFalse(_modelDirtyDetector.IsDirty);

            _modelDirtyDetector.Stop();
            Assert.IsFalse(_modelDirtyDetector.IsDirty);

            _testingClassA.Bool = !_testingClassA.Bool;
            Assert.IsFalse(_modelDirtyDetector.IsDirty);

            _modelDirtyDetector.Start(_testingClassA);
            Assert.IsFalse(_modelDirtyDetector.IsDirty);

            _testingClassA.Bool = !_testingClassA.Bool;
            Assert.IsTrue(_modelDirtyDetector.IsDirty);

            _modelDirtyDetector.Stop();
            Assert.IsFalse(_modelDirtyDetector.IsDirty);
        }

        [Test]
        public void NoMemoryLeaksAfterStop()
        {
            var weakReference = new WeakReference(_testingClassA);

            _modelDirtyDetector.Start(_testingClassA);

            _testingClassA.ClassB = new ClassB { ClassA = new ClassA() };
            var classA = new ClassA();
            _testingClassA.Items.Add(classA);
            classA.ClassB = new ClassB();

            _testingClassA = null;

            GC.Collect();
            Assert.True(weakReference.IsAlive);

            _modelDirtyDetector.Stop();

            GC.Collect();
            Assert.IsFalse(weakReference.IsAlive);
        }
    }
}
