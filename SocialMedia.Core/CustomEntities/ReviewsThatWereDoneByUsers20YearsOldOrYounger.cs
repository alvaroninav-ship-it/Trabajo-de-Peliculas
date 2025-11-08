using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.CustomEntities
{
    public class ReviewsThatWereDoneByUsers20YearsOldOrYounger
    {
        public string Description { get; set; }
        public float Grade { get; set; }
        public DateTime Date { get; set; }
        public string Titulo_Pelicula { get; set; }
        public string Usuario { get; set; }
    }
}
