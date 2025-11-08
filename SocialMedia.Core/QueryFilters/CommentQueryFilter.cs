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
    /// Filtra los parametros de comment
    /// </summary>
    public class CommentQueryFilter: PaginationQueryFilter
    {
        /// <summary>
        /// Id de la review
        /// </summary>
        [SwaggerSchema("Id de la review")]
        public int? ReviewId { get; set; }

        /// <summary>
        /// Id del usuario
        /// </summary>
        [SwaggerSchema("Id del usuario")]
        public int? UserId { get; set; }

        /// <summary>
        /// Fecha de publicacion
        /// </summary>
        [SwaggerSchema("Fecha de publicacion")]
        public string? Date { get; set; }

        /// <summary>
        /// Descripcion del comment
        /// </summary>
        [SwaggerSchema("Descripcion del comment")]
        public string? Description { get; set; }

    }
}
