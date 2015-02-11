namespace ElMariachi.WPF.Tools.Logging.LoggedItems
{
    /// <summary>
    /// Represents a logged item
    /// </summary>
    public interface ILoggedItem
    {
        /// <summary>
        /// A text representation of the logged item
        /// </summary>
        string Text { get; }

        /// <summary>
        /// A text representation of this type of logged item.
        /// Generally "Warning", "Error", ...
        /// </summary>
        string TypeText { get; }
    }
}