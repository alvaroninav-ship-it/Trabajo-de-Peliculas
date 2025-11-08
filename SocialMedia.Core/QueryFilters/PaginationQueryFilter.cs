using Swashbuckle.AspNetCore.Annotations;


namespace Movies.Core.QueryFilters
{
    public abstract class PaginationQueryFilter
    {
        /// <summary>
        /// Cantidad de registros por pagina
        /// </summary>
        [SwaggerSchema("Cantidad de registros por pagina")]
        public int PageSize { get; set; } = 10;


        /// <summary>
        /// Numero de pagina a mostrar
        /// </summary>
        [SwaggerSchema("Numero de pagina a mostrar")]
        public int PageNumber { get; set; } = 1;
    }
}
