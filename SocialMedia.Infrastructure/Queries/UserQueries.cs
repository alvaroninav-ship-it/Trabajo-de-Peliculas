using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Infrastructure.Queries
{
    public static class UserQueries
    {
        public static string Top10UsersMostCommentedInTheirReview= @"
                   SELECT TOP 10 u.FirstName, u.LastName, COUNT(c.Id) AS Cantidad_Comentarios_Recibidos
                   FROM [User] u
                   INNER JOIN Review r ON r.UserId = u.Id
                   INNER JOIN Comment c ON c.ReviewId = r.Id
                   GROUP BY u.FirstName, u.LastName
                   ORDER BY COUNT(c.Id) DESC;
                   ";
        public static string UsersThatReviewLastYearMovies= @"
                   SELECT DISTINCT u.FirstName, m.Title AS MovieTitle, m.ReleaseDate
                   FROM [User] u
                   INNER JOIN Review r ON r.UserId = u.Id
                   INNER JOIN Movie m ON r.MovieId = m.Id
                   WHERE m.ReleaseDate >= DATEADD(YEAR, -1, GETDATE())
                   ORDER BY m.ReleaseDate DESC;
                   ";
        public static string Top10UsersThatHasDoneMoreComments= @"          
                    SELECT TOP 10 u.FirstName,u.LastName, COUNT(c.Id) AS TotalComments
                   FROM [User] u
                   INNER JOIN Comment c ON c.UserId = u.Id
                  GROUP BY u.FirstName, u.LastName
                  ORDER BY COUNT(c.Id) DESC;
                   ";
    }
}
