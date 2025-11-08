using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Infrastructure.Queries
{
    public static class ActorQueries
    {
        public static string Top10TheYoungestActors= @"
                        select Top 10 a.FirstName, a.LastName, a.IsActive, DATEDIFF(Year,a.DateOfBirth,GetDate()) as Edad
                        from Actor a
                        order by DATEDIFF(Year,a.DateOfBirth,GetDate())
                        ";

    }
}
