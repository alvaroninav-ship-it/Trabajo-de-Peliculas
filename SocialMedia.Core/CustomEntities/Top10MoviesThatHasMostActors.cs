using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.CustomEntities
{
    public class Top10MoviesThatHasMostActors
    {
        public string title { get; set; }
        public string description { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Length { get; set; }
        public string Genre { get; set; }
        public int TotalActores { get; set; }
    }
}
