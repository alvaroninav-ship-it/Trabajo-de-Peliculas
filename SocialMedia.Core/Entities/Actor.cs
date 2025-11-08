namespace Movies.Core.Entities
{
    /// <summary>
    /// Representa un Actor de pelicula en el sistema
    /// </summary>
    /// <remarks>
    /// Esta entidad almacena la información principal de un Actor de una pelicula
    /// y es utilizada para las relaciones con peliculas.
    /// </remarks>
    public partial class Actor : BaseEntity
    {
        /// <summary>
        /// Id que hace referencia a una Entidad Movie
        /// </summary>
        /// <example>1</example>
        public int? MovieId { get; set; }

        /// <summary>
        /// Nombre de la entidad
        /// </summary>
        /// <example>Marco</example>
        public string FirstName { get; set; } = null!;

        /// <summary>
        /// Apellido de la entidad
        /// </summary>
        /// <example>Fernandez</example>
        public string LastName { get; set; } = null!;

        /// <summary>
        /// Direccion de correo electronico de la entidad
        /// </summary>
        /// <example>aldf@.com</example>
        public string Email { get; set; } = null!;
        /// <summary>
        /// Fecha de cumpleanos 
        /// </summary>
        /// <example>1</example>
        public DateTime DateOfBirth { get; set; }
        /// <summary>
        /// Id que hace referencia a una Entidad Review
        /// </summary>
        /// <example>1</example>
        public bool IsActive { get; set; }
        /// <summary>
        /// Id que hace referencia a una Entidad Review
        /// </summary>
        /// <example>1</example>
        public virtual Movie? Movie { get; set; }

    }
}