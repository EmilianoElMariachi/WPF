using System;
using ElMariachi.WPF.Tools.Modelling.ModelRecording.PrivateClasses;
using ElMariachi.WPF.Tools.UndoRedo;
using ElMariachi.WPF.Tools.UndoRedo.RevertibleCommands;

namespace ElMariachi.WPF.Tools.Modelling.ModelRecording
{
    public class ModelRecorder : IModelRecorder, IRecorderInterface
    {

        #region Fields & Properties

        private RecordedElement _recordedElement;

        public IUndoRedoService UndoRedoService { get; private set; }

        public bool IsRecording
        {
            get { return _recordedElement != null; }
        }

        #endregion


        public bool CanRecordPropertyChange
        {
            get { return !this.UndoRedoService.IsUndoing && !this.UndoRedoService.IsRedoing; }
        }

        public void RecordPropertyChange(IRecordedPropertyInfo property, IRevertibleCommand revertibleCommand)
        {
            if (property.DelayMs != 0)
            {
                
            }
            this.UndoRedoService.AddExecutedCommand(revertibleCommand);

        }

        #region Methods

        public void Record(IUndoRedoService undoRedoService, object objToRecord)
        {
            if (undoRedoService == null)
            {
                throw new ArgumentNullException("undoRedoService");
            }
            if (objToRecord == null)
            {
                throw new ArgumentNullException("objToRecord");
            }

            this.UndoRedoService = undoRedoService;

            _recordedElement = RecordedElementFactory.Create(this, null, objToRecord);
        }

        public void Stop()
        {
            if (_recordedElement != null)
            {
                _recordedElement.Release();
                _recordedElement = null;
            }
        }

        public IDisposable GroupRecords()
        {
            throw new NotImplementedException();
        }

        #endregion


    }
}
