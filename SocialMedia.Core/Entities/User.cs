using System;
using System.Collections.Generic;

namespace Movies.Core.Entities
{
    /// <summary>
    /// Representa un usuario en el sistema
    /// </summary>
    /// <remarks>
    /// Esta entidad almacena la información principal de un usuario
    /// y es utilizada para las relaciones con peliculas por parte de usuarios.
    /// </remarks>
    public partial class User : BaseEntity
    {
        /// <summary>
        /// Nombre del usuario
        /// </summary>
        /// <example>Mario</example>
        public string FirstName { get; set; } = null!;
        /// <summary>
        /// Apellido
        /// </summary>
        /// <example>Soliz</example>
        public string LastName { get; set; } = null!;
        /// <summary>
        /// Correo electronico
        /// </summary>
        /// <example>1232@.gmail.com</example>
        public string Email { get; set; } = null!;
        /// <summary>
        /// Fecha de nacimiento
        /// </summary>
        /// <example>12/02/2010</example>
        public DateTime DateOfBirth { get; set; }
        /// <summary>
        /// Telefono
        /// </summary>
        /// <example>12343523</example>
        public string? Telephone { get; set; }
        /// <summary>
        /// Usuario activo
        /// </summary>
        /// <example>1=Si</example>
        public bool IsActive { get; set; }
        /// <summary>
        /// Comentarios hecho por el usuario
        /// </summary>
        /// <example>Comments[]</example>
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        /// <summary>
        /// Reviews hechos por el usuario
        /// </summary>
        /// <example>Reviews[]</example>
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}