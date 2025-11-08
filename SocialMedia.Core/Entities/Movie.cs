using System.Globalization;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Movies.Core.Entities
{
    /// <summary>
    /// Representa una Pelicula en el sistema
    /// </summary>
    /// <remarks>
    /// Esta entidad almacena la información principal de una pelicula
    /// y es utilizada para las relaciones con reviews y actores.
    /// </remarks>

    public partial class Movie : BaseEntity
    {
        /// <summary>
        /// Titulo de la pelicula
        /// </summary>
        /// <example>La nueva ezperanza</example>
        public string? Title { get; set; }
        /// <summary>
        /// Sinopsis y descripcion breve de la pelicula
        /// </summary>
        /// <example>En esta aventura se revelara el secreto mas grande de la humanidad</example>
        public string? Description { get; set; }
        /// <summary>
        /// Fecha de lanzamiento a cines y al publico de la pelicula
        /// </summary>
        /// <example>12/03/2022</example>
        public DateTime ReleaseDate { get; set; }
        /// <summary>
        /// Actores que participan de esta pelicula
        /// </summary>
        /// <example>Actores[]</example>
        public virtual ICollection<Actor> Actors { get; set; } = new List<Actor>();
        /// <summary>
        /// Duracion de tiempo de la pelicula
        /// </summary>
        /// <example>1</example>
        public string Length { get; set; } = null!;
        /// <summary>
        /// Tipo de genero de la pelicula
        /// </summary>
        /// <example>1</example>
        public string Genre { get; set; } = null!;
        /// <summary>
        /// Reviews que recibio la pelicula por parte de consumidores
        /// </summary>
        /// <example>Reviews[]</example>
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    }
}

