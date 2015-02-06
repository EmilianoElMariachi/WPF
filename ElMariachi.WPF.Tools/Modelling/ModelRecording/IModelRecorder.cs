using ElMariachi.WPF.Tools.UndoRedo;

namespace ElMariachi.WPF.Tools.Modelling.ModelRecording
{
    internal interface IModelRecorder
    {

        /// <summary>
        /// The Undo/Redo service given at <see cref="Record"/> or null if recorder is stopped
        /// </summary>
        IUndoRedoService UndoRedoService { get; }

        /// <summary>
        /// True if recorder is recording, false otherwise
        /// </summary>
        bool IsRecording { get; }

        /// <summary>
        /// Starts recording changes of object to record
        /// </summary>
        /// <param name="undoRedoService"></param>
        /// <param name="objToRecord"></param>
        void Record(IUndoRedoService undoRedoService, object objToRecord);

        /// <summary>
        /// Stop recording of object changes
        /// </summary>
        void Stop();

    }
}