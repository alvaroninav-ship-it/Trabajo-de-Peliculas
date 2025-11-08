namespace Movies.Core.Entities
{
    /// <summary>
    /// Representa un comentario de una review en el sistema
    /// </summary>
    /// <remarks>
    /// Esta entidad almacena la información principal de un comentario 
    /// y es utilizada para la relacion con criticas y usuarios.
    /// </remarks>
    public partial class Comment : BaseEntity
    {
        /// <summary>
        /// Id que hace referencia a una Entidad Review
        /// </summary>
        /// <example>1</example>
        public int? ReviewId { get; set; }

        /// <summary>
        /// Id que hace referencia a una Entidad Usuario
        /// </summary>
        /// <example>1</example>
        public int? UserId { get; set; }

        /// <summary>
        /// Descripcion del comentario
        /// </summary>
        /// <example>Estoy en total desacuerdo con tu critica</example>

        public string Description { get; set; } = null!;

        /// <summary>
        /// Fecha de publicacion del comentario
        /// </summary>
        /// <example>12/10/2005</example>

        public DateTime Date { get; set; }

        /// <summary>
        /// El comentario esta activo
        /// </summary>
        /// <example>1 = Si</example>

        public bool? IsActive { get; set; }

        /// <summary>
        /// Entidad Review a la que esta conectada
        /// </summary>
        /// <example>Review{}</example>

        public virtual Review? Review { get; set; }

        /// <summary>
        /// Entidad User a la que esta conectada
        /// </summary>
        /// <example>User{}</example>

        public virtual User? User { get; set; }
    }
}

