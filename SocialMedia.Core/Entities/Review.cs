using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.Entities
{
    /// <summary>
    /// Representa una critica a una pelicula en el sistema
    /// </summary>
    /// <remarks>
    /// Esta entidad almacena la información principal de una critica
    /// y es utilizada para las relaciones con peliculas por parte de usuarios.
    /// </remarks>
    public partial class Review : BaseEntity
    {
        /// <summary>
        /// Id referente a una entidad user
        /// </summary>
        /// <example>1</example>
        public int? UserId { get; set; }
        /// <summary>
        /// Id referente a una entidad movie
        /// </summary>
        /// <example>1</example>
        public int? MovieId { get; set; }
        /// <summary>
        /// Descripcion de la review acerca de la pelicula
        /// </summary>
        /// <example>La nueva ezperanza me parece aburrida</example>
        public string? Description { get; set; }
        /// <summary>
        /// Fecha de publicacion de la pelicula
        /// </summary>
        /// <example>12/04/2023</example>
        public DateTime Date { get; set; }
        /// <summary>
        /// Commentarios en respuestas a la critica
        /// </summary>
        /// <example>Comments[]</example>
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        /// <summary>
        /// Usuario relacionado quien publico la review
        /// </summary>
        /// <example>User{}</example>
        public virtual User? User { get; set; }
        /// <summary>
        /// Pelicula que fue criticada
        /// </summary>
        /// <example>Movie{}</example>
        public virtual Movie? Movie { get; set; }
        /// <summary>
        /// Calificacion de la pelicula
        /// </summary>
        /// <example>6.77</example>
        public float Grade { get; set; }

    }
}

