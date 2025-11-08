using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.CustomEntities
{
    public class Top10MostCommentedReviews
    {
        public string Description { get; set; }
        public int MovieId { get; set; }
        public float Grade { get; set; }
        public DateTime Date { get; set; }
        public int Total_comentarios { get; set; }
    }
}
