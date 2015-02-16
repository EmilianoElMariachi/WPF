using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using ElMariachi.WPF.Tools.UndoRedo.RevertibleCommands;

namespace ElMariachi.WPF.Tools.Modelling.ModelRecording.PrivateClasses
{

    internal class CollectionChangedRecordedElement : RecordedElement, IRecordedPropertyInfo
    {

        #region Fields & Properties

        private readonly IRecorderInterface _recorderInterface;
        private readonly IList _objAsIList;
        private readonly INotifyCollectionChanged _listAsNotifyCollectionChanged;
        private readonly List<RecordedElement> _recordedElementItems = new List<RecordedElement>();

        public uint DelayMs
        {
            get { return 0; } //TODO ELM: analyzer le comportement a implémenter
        }


        #endregion

        #region Constructors

        public CollectionChangedRecordedElement(IRecorderInterface recorderInterface, IList objAsIList, INotifyCollectionChanged objAsNotifyCollectionChanged)
            : base(objAsNotifyCollectionChanged)
        {
            if (!ReferenceEquals(objAsIList, objAsNotifyCollectionChanged))
            {
                throw new ArgumentException("Arguments should have the same references");
            }

            _recorderInterface = recorderInterface;
            _objAsIList = objAsIList;
            _listAsNotifyCollectionChanged = objAsNotifyCollectionChanged;
            _listAsNotifyCollectionChanged.CollectionChanged += OnCollectionChanged;

            foreach (var item in _objAsIList)
            {
                _recordedElementItems.Add(RecordedElementFactory.Create(recorderInterface, this, item));
            }

        }

        #endregion

        #region Methods

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (_recorderInterface.CanRecordPropertyChange)
            {
                _recorderInterface.RecordPropertyChange(this, CreateRevertibleCommandFromCollectionChange(notifyCollectionChangedEventArgs));
            }

            SynchronizeWithRecordedItems(notifyCollectionChangedEventArgs);
        }

        private IRevertibleCommand CreateRevertibleCommandFromCollectionChange(NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            switch (notifyCollectionChangedEventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        return new ExternalizedRevertibleCommand(
                            () =>
                            {
                                var insertionIndex = notifyCollectionChangedEventArgs.NewStartingIndex;
                                foreach (var newItem in notifyCollectionChangedEventArgs.NewItems)
                                {
                                    _objAsIList.Insert(insertionIndex++, newItem);
                                }
                            },
                            () =>
                            {
                                foreach (var newItem in notifyCollectionChangedEventArgs.NewItems)
                                {
                                    _objAsIList.RemoveAt(notifyCollectionChangedEventArgs.NewStartingIndex);
                                }
                            });
                    }
                case NotifyCollectionChangedAction.Move:
                    {
                        return new ExternalizedRevertibleCommand(
                            () =>
                            {
                                var oldStartingIndex = notifyCollectionChangedEventArgs.OldStartingIndex;
                                foreach (var item in notifyCollectionChangedEventArgs.OldItems)
                                {
                                    _objAsIList.RemoveAt(oldStartingIndex++);
                                }
                                var newStartingIndex = notifyCollectionChangedEventArgs.NewStartingIndex;
                                foreach (var item in notifyCollectionChangedEventArgs.NewItems)
                                {
                                    _objAsIList.Insert(newStartingIndex++, item);
                                }
                            },
                            () =>
                            {
                                var newStartingIndex = notifyCollectionChangedEventArgs.NewStartingIndex;
                                foreach (var item in notifyCollectionChangedEventArgs.NewItems)
                                {
                                    _objAsIList.RemoveAt(newStartingIndex++);
                                }
                                var oldStartingIndex = notifyCollectionChangedEventArgs.OldStartingIndex;
                                foreach (var item in notifyCollectionChangedEventArgs.OldItems)
                                {
                                    _objAsIList.Insert(oldStartingIndex++, item);
                                }
                            });
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        return new ExternalizedRevertibleCommand(
                            () =>
                            {
                                foreach (var removedItem in notifyCollectionChangedEventArgs.OldItems)
                                {
                                    _objAsIList.RemoveAt(notifyCollectionChangedEventArgs.OldStartingIndex);
                                }
                            },
                            () =>
                            {
                                var insertionIndex = notifyCollectionChangedEventArgs.OldStartingIndex;
                                foreach (var removedItem in notifyCollectionChangedEventArgs.OldItems)
                                {
                                    _objAsIList.Insert(insertionIndex++, removedItem);
                                }
                            });
                    }
                case NotifyCollectionChangedAction.Replace:
                    {
                        return new ExternalizedRevertibleCommand(
                            () =>
                            {
                                var index = notifyCollectionChangedEventArgs.NewStartingIndex;
                                foreach (var replacedItem in notifyCollectionChangedEventArgs.NewItems)
                                {
                                    _objAsIList[index++] = replacedItem;
                                }
                            },
                            () =>
                            {
                                var index = notifyCollectionChangedEventArgs.OldStartingIndex;
                                foreach (var previousItem in notifyCollectionChangedEventArgs.OldItems)
                                {
                                    _objAsIList[index++] = previousItem;
                                }
                            });
                    }
                case NotifyCollectionChangedAction.Reset:
                    {
                        var removedItems = _recordedElementItems.Select((element) => element.OldValue).ToArray();
                        return new ExternalizedRevertibleCommand(
                            () =>
                            {
                                _objAsIList.Clear();
                            },
                            () =>
                            {
                                foreach (var removedItem in removedItems)
                                {
                                    _objAsIList.Add(removedItem);
                                }

                            });
                    }
                default:
                    {
                        throw new Exception("unknown " + typeof(NotifyCollectionChangedAction).Name + " = " + notifyCollectionChangedEventArgs.Action);
                    }
            }

        }

        private void SynchronizeWithRecordedItems(NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            switch (notifyCollectionChangedEventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        var insertionIndex = notifyCollectionChangedEventArgs.NewStartingIndex;
                        foreach (var newItem in notifyCollectionChangedEventArgs.NewItems)
                        {
                            _recordedElementItems.Insert(insertionIndex, RecordedElementFactory.Create(_recorderInterface, this, newItem));
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Move:
                    {
                        var oldStartingIndex = notifyCollectionChangedEventArgs.OldStartingIndex;

                        var itemsToMove = new List<RecordedElement>();
                        foreach (var item in notifyCollectionChangedEventArgs.OldItems)
                        {
                            itemsToMove.Add(_recordedElementItems[oldStartingIndex]);
                            _recordedElementItems.RemoveAt(oldStartingIndex++);
                        }

                        var newStartingIndex = notifyCollectionChangedEventArgs.NewStartingIndex;
                        foreach (var item in itemsToMove)
                        {
                            _recordedElementItems.Insert(newStartingIndex++, item);
                        }

                        break;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (var removedItem in notifyCollectionChangedEventArgs.OldItems)
                        {
                            _recordedElementItems[notifyCollectionChangedEventArgs.OldStartingIndex].Release();
                            _recordedElementItems.RemoveAt(notifyCollectionChangedEventArgs.OldStartingIndex);
                        }

                        break;
                    }
                case NotifyCollectionChangedAction.Replace:
                    {
                        var index = notifyCollectionChangedEventArgs.NewStartingIndex;
                        foreach (var newItem in notifyCollectionChangedEventArgs.NewItems)
                        {
                            _recordedElementItems[index].Release();
                            _recordedElementItems[index] = RecordedElementFactory.Create(_recorderInterface, this, newItem);
                            index++;
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Reset:
                    {
                        foreach (var item in _recordedElementItems)
                        {
                            item.Release();
                        }
                        _recordedElementItems.Clear();
                        break;
                    }
                default:
                    {
                        throw new Exception("unknown " + typeof(NotifyCollectionChangedAction).Name + " = " + notifyCollectionChangedEventArgs.Action);
                    }
            }

        }

        internal override void Release()
        {
            _listAsNotifyCollectionChanged.CollectionChanged -= OnCollectionChanged;
            foreach (var recordedElementItem in _recordedElementItems)
            {
                recordedElementItem.Release();
            }
        }

        #endregion

    }
}
