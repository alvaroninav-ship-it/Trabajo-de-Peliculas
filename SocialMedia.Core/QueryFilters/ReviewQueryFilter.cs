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
    /// Filtra los parametros de review
    /// </summary>
    public class ReviewQueryFilter: PaginationQueryFilter
    {
        /// <summary>
        /// Id del Usuario
        /// </summary>
        [SwaggerSchema("Id del usuario")]
        public int? UserId { get; set; }

        /// <summary>
        /// Id de la pelicula
        /// </summary>
        [SwaggerSchema("Id de la pelicula")]
        public int? MovieId { get; set; }

        /// <summary>
        /// Fecha de publicacion
        /// </summary>
        [SwaggerSchema("Fecha de publicacion")]
        public string? Date { get; set; }
        /// <summary>
        /// Puntaje
        /// </summary>
        [SwaggerSchema("Puntaje dado")]
        public float? Grade { get; set; }

        /// <summary>
        /// Descripcion de la review
        /// </summary>
        [SwaggerSchema("Descripcion de la review")]
        public string? Description { get; set; }
    }
}
