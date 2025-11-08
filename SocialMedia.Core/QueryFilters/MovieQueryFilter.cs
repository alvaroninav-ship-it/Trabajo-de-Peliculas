using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Movies.Core.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace Movies.Core.QueryFilters
{
    /// <summary>
    /// Filtra los parametros de movie
    /// </summary>
    public class MovieQueryFilter: PaginationQueryFilter
    {
        public string? Title { get; set; }
        /// <summary>
        /// Fecha de lanzamiento
        /// </summary>
        [SwaggerSchema("Fecha de lanzamiento")]
        public string? ReleaseDate { get; set; }

        /// <summary>
        /// Duracion
        /// </summary>
        [SwaggerSchema("Duracion")]
        public string? Length { get; set; } = null!;

        /// <summary>
        /// Genero
        /// </summary>
        [SwaggerSchema("Genero")]
        public string? Genre { get; set; } = null!;

        /// <summary>
        /// Descripcion de la pelicula
        /// </summary>
        [SwaggerSchema("Descripcion de la pelicula")]
        public string? Description { get; set; }

    }
}
