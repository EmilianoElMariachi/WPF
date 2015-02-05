using ElMariachi.WPF.Tools.UndoRedo;

namespace ElMariachi.WPF.Tools.Modelling.ModelRecording
{
    internal interface IModelRecorder
    {
        IUndoRedoService UndoRedoService { get; }

        bool IsRecording { get; }

        void Record(IUndoRedoService undoRedoService, object objToRecord);

        void Stop();
    }
}