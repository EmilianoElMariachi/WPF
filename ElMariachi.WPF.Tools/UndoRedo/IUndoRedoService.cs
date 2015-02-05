using ElMariachi.WPF.Tools.UndoRedo.EventsDefinition;
using ElMariachi.WPF.Tools.UndoRedo.RevertibleCommands;

namespace ElMariachi.WPF.Tools.UndoRedo
{
    public interface IUndoRedoService
    {

        /// <summary>
        /// Evènement déclenché lorsqu'une commande est sur le point d'être executée
        /// </summary>
        event BeforeUndoRedoCommandExecutedEventHandler BeforeUndoRedoCommandExecuted;

        /// <summary>
        /// Evènement déclenché lorsqu'une commande vient d'être executée
        /// </summary>
        event UndoRedoCommandExecutedEventHandler UndoRedoCommandExecuted;

        /// <summary>
        /// Obtient un booléen qui indique si une opération d'undo est en cours
        /// </summary>
        bool IsUndoing { get; }  
        
        /// <summary>
        /// Obtient un booléen qui indique si une opération de redo est en cours
        /// </summary>
        bool IsRedoing { get; }

        /// <summary>
        /// Obtient le nombre d'Undo disponibles
        /// </summary>
        int NbUndo { get; }

        /// <summary>
        /// Obtient le nombre de redo disponibles
        /// </summary>
        int NbRedo { get; }

        /// <summary>
        /// Ajoute et execute la nouvelle commande reversible passée.
        /// </summary>
        /// <param name="revertibleCommand"></param>
        /// <returns></returns>
        bool Execute(IRevertibleCommand revertibleCommand);

        /// <summary>
        /// Ajoute la commande reversible déjà executée à la liste des commandes
        /// </summary>
        /// <param name="revertibleCommand"></param>
        void AddExecutedCommand(IRevertibleCommand revertibleCommand);

        /// <summary>
        /// Obtient un booléen indiquant s'il est possible d'executer la fonction <see cref="Undo"/>
        /// </summary>
        /// <returns></returns>
        bool CanUndo();

        /// <summary>
        /// Obtient un booléen indiquant s'il est possible d'executer la fonction <see cref="Redo"/>
        /// </summary>
        /// <returns></returns>
        bool CanRedo();

        /// <summary>
        /// Re-execute l'action
        /// </summary>
        void Redo();

        /// <summary>
        /// Restaure l'action
        /// </summary>
        void Undo();


        UndoRedoStackData[] UndoRedoStack { get; }

    }

    public class UndoRedoStackData
    {
        public UndoRedoStackData(long revertibleCommandId, string description)
        {
            Description = description;
            RevertibleCommandId = revertibleCommandId;
        }

        public long RevertibleCommandId { get; private set; }

        public string Description { get; private set; }

    }
}