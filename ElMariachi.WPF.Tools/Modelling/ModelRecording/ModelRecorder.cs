using System;
using ElMariachi.WPF.Tools.Modelling.ModelRecording.PrivateClasses;
using ElMariachi.WPF.Tools.UndoRedo;

namespace ElMariachi.WPF.Tools.Modelling.ModelRecording
{
    public class ModelRecorder : IModelRecorder
    {

        #region Fields & Properties

        private RecordedElement _recordedElement;

        public IUndoRedoService UndoRedoService { get; private set; }

        public bool IsRecording
        {
            get { return _recordedElement != null; }
        }

        #endregion

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

            _recordedElement = RecordedElementFactory.Create(undoRedoService, objToRecord);
        }

        public void Stop()
        {
            if (_recordedElement != null)
            {
                _recordedElement.Release();
                _recordedElement = null;
            }
        }

        #endregion

    }
}
