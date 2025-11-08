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
    public class UserQueryFilter: PaginationQueryFilter
    {
        /// <summary>
        /// Fecha de nacimiento del Usuario
        /// </summary>
        [SwaggerSchema("Fecha de nacimiento")]
        public string? DateOfBirth { get; set; }

        /// <summary>
        /// Esta activo?
        /// </summary>
        [SwaggerSchema("Esta activo?")]
        public bool? IsActive { get; set; }

        /// <summary>
        /// Esta activo?
        /// </summary>
        [SwaggerSchema("Nombre del usuario?")]
        public string? FirstName { get; set; }

        /// <summary>
        /// Esta activo?
        /// </summary>
        [SwaggerSchema("Apellido del usuario?")]
        public string? LastName { get; set;}



    }
}
