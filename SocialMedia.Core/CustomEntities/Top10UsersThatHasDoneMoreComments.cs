using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.CustomEntities
{
    public class Top10UsersThatHasDoneMoreComments
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int TotalComments { get; set; }
    }
}
