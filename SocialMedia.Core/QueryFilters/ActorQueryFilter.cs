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
    /// Filtra los parametros de actor
    /// </summary>
    public class ActorQueryFilter: PaginationQueryFilter
    {
        /// <summary>
        /// Nombre del actor
        /// </summary>
        [SwaggerSchema("Nombre del actor")]
        public string? FirstName { get; set; }
        /// <summary>
        /// Apellido del actor
        /// </summary>
        [SwaggerSchema("Apellido del actor")]
        public string? LastName { get; set; }

        /// <summary>
        /// Id de la pelicula
        /// </summary>
        [SwaggerSchema("Id de la pelicula")]
        public int? MovieId { get; set; }

        /// <summary>
        /// Fecha de nacimiento del actor
        /// </summary>
        [SwaggerSchema("Fecha de nacimiento")]
        public string? DateOfBirth { get; set; }

        /// <summary>
        /// Esta activo?
        /// </summary>
        [SwaggerSchema("Sigue activo?")]
        public bool? IsActive { get; set; }

    }
}
