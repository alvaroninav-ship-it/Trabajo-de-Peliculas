using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.CustomEntities
{
    public class MostFamousMovieForYear
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime ReleaseDate { get; set; } 
        public string Length { get; set; }
        public string Genre { get; set; }
        public int TotalReviews { get; set; }
    }
}
