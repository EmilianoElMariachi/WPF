using ElMariachi.WPF.Tools.UndoRedo.EventsDefinition;
using ElMariachi.WPF.Tools.UndoRedo.RevertibleCommands;

namespace ElMariachi.WPF.Tools.UndoRedo
{
    public interface IUndoRedoService
    {

        /// <summary>
        /// Event triggered just before a <see cref="IRevertibleCommand"/> is added but not yet executed (<see cref="AddExecutedCommand"/>)
        /// </summary>
        event BeforeUndoRedoCommandExecutedEventHandler BeforeUndoRedoCommandExecuted;

        /// <summary>
        /// Event triggered just after a <see cref="IRevertibleCommand"/> is added and executed (<see cref="AddExecutedCommand"/>)
        /// </summary>
        event UndoRedoCommandExecutedEventHandler UndoRedoCommandExecuted;

        /// <summary>
        /// Event triggered when <see cref="CanUndo"/> is changed
        /// </summary>
        event CanUndoChangedEventHandler CanUndoChanged;

        /// <summary>
        /// Event triggered when <see cref="CanRedo"/> is changed
        /// </summary>
        event CanRedoChangedEventHandler CanRedoChanged;

        /// <summary>
        /// Get an informative array of available stacked revertible commands
        /// </summary>
        UndoRedoStackInfo[] StackInfo { get; }

        /// <summary>
        /// Gets a boolean indicating whether an <see cref="Undo"/> operation is currently executed
        /// </summary>
        bool IsUndoing { get; }  
        
        /// <summary>
        /// Gets a boolean indicating whether a <see cref="Redo"/> operation is currently executed
        /// </summary>
        bool IsRedoing { get; }

        /// <summary>
        /// Gets the number of available <see cref="Undo"/> operations
        /// </summary>
        int NbUndo { get; }

        /// <summary>
        /// Gets the number of available <see cref="Redo"/> operations
        /// </summary>
        int NbRedo { get; }

        /// <summary>
        /// Gets a boolean indicating whether the method <see cref="Undo"/> can be called
        /// </summary>
        bool CanUndo { get; }

        /// <summary>
        /// Gets a boolean indicating whether the method <see cref="Redo"/> can be called
        /// </summary>
        /// <returns></returns>
        bool CanRedo { get; }

        /// <summary>
        /// Executes the <see cref="IRevertibleCommand"/> calling the <see cref="IRevertibleCommand.Do"/> method and
        /// adds the command to the stack
        /// </summary>
        /// <param name="revertibleCommand">The command to be executed</param>
        void Execute(IRevertibleCommand revertibleCommand);

        /// <summary>
        /// Adds the <see cref="IRevertibleCommand"/> to the stack without executing it
        /// </summary>
        /// <param name="revertibleCommand"></param>
        void AddExecutedCommand(IRevertibleCommand revertibleCommand);

        /// <summary>
        /// Calls <see cref="IRevertibleCommand.Do"/> of the focused stacked command
        /// </summary>
        void Redo();

        /// <summary>
        /// Calls <see cref="IRevertibleCommand.Undo"/> of the focused stacked command
        /// </summary>
        void Undo();

    }
}