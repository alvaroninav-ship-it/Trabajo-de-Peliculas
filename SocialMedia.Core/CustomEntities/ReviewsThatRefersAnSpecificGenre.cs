using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.CustomEntities
{
    public class ReviewsThatRefersAnSpecificGenre
    {
        public string Description { get; set; }
        public float Grade { get; set; }
        public DateTime Date { get; set; }
        public string Movie_Title{get; set ; }
        public string Genre { get; set; }
    }
}
