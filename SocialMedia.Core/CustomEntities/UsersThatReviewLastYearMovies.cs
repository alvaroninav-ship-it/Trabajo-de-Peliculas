using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.CustomEntities
{
    public class UsersThatReviewLastYearMovies
    {
        public string FirstName { get; set; }
        public string MovieTitle { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}
