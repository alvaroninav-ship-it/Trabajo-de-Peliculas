using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.Entities
{
    /// <summary>
    /// Representa una [Entidad] en el sistema
    /// </summary>
    /// <remarks>
    /// Esta entidad almacena la información principal de [Entidad] 
    /// y es utilizada para las operaciones de persistencia.
    /// </remarks>

    public class BaseEntity
    {
        /// <summary>
        /// Identificador único de la [Entidad]
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }
    }
}
