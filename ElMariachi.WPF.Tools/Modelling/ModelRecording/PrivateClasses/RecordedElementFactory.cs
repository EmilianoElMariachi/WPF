using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using ElMariachi.WPF.Tools.UndoRedo;

namespace ElMariachi.WPF.Tools.Modelling.ModelRecording.PrivateClasses
{
    internal static class RecordedElementFactory
    {

        #region Methods

        internal static RecordedElement Create(IUndoRedoService undoRedoService, object obj)
        {
            var objAsIList = obj as IList;
            var objAsNotifyCollectionChanged = obj as INotifyCollectionChanged;
            if (objAsNotifyCollectionChanged != null && objAsIList != null)
            {
                return new CollectionChangedRecordedElement(undoRedoService, objAsIList, objAsNotifyCollectionChanged);
            }
            else
            {
                var objAsNotifyPropertyChanged = obj as INotifyPropertyChanged;
                if (objAsNotifyPropertyChanged != null)
                {
                    return new NotifyPropertyChangedRecordedElement(undoRedoService, objAsNotifyPropertyChanged);
                }
                else
                {
                    return new DefaultRecordedElement(obj);
                }
            }
        }

        #endregion

    }
}