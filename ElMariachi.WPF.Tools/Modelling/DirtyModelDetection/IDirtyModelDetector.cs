using ElMariachi.WPF.Tools.Modelling.DirtyModelDetection.EventsDefinition;

namespace ElMariachi.WPF.Tools.Modelling.DirtyModelDetection
{
    public interface IDirtyModelDetector
    {

        /// <summary>
        /// When true, the observed object is dirty.
        /// </summary>
        bool IsDirty { get; }

        /// <summary>
        /// True when an object is actually observed, False otherwise
        /// </summary>
        bool IsStarted { get; }

        /// <summary>
        /// Event raised when model is becoming dirty or cleaned
        /// </summary>
        event DirtyStateChangedEventHandler DirtyStateChanged;

        /// <summary>
        /// Start auto-detection of changes of given object
        /// Throws if given object is null
        /// </summary>
        /// <param name="obj"></param>
        void Start(object obj);

        /// <summary>
        /// Restart auto-detection with current state of the last started object.
        /// <see cref="Start"/>
        /// Throws if no object was previously started.
        /// </summary>
        void Restart();

        /// <summary>
        /// Stop auto-detection
        /// </summary>
        void Stop();

    }
}