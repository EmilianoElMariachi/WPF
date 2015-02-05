namespace ElMariachi.WPF.Tools.UndoRedo.RevertibleCommands
{
    /// <summary>
    /// Interface représentant une commande pouvant être annulée
    /// </summary>
    public interface IRevertibleCommand
    {

        /// <summary>
        /// Obtient un l'identifiant unique cette instance
        /// </summary>
        long Id { get; }

        /// <summary>
        /// Obtient un indication sur la nature de l'action réalisée par cette commande
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Méthode réalisant la modification souhaitée
        /// </summary>
        void Do();

        /// <summary>
        /// Méthode permettant d'annuler la modification apportée par l'execution de la méthode <see cref="Do"/>
        /// </summary>
        void Undo();

    }
}