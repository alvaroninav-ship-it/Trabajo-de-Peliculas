using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Infrastructure.Queries
{
    public static class ReviewQueries
    {
        public static string Top10MostCommentedReviews= @"                            
                           select top 10 r.Description, r.MovieId, r.Grade, r.Date, count(c.ReviewId) as total_comentarios
                           from Review r inner join Comment c on c.ReviewId=r.Id
                           group by r.Description, r.MovieId, r.Grade, r.Date
                           order by count(c.ReviewId) desc;
                              ";
        public static string ReviewsThatWereDoneByUsers20YearsOldOrYounger= @"
                           SELECT r.Description, r.Grade, r.Date, m.Title AS MovieTitle, u.FirstName AS UserName
                           FROM Review r
                           INNER JOIN [User] u ON u.Id = r.UserId
                           INNER JOIN Movie m ON m.Id = r.MovieId
                           WHERE DATEDIFF(YEAR, u.DateOfBirth, GETDATE()) <= 20;
                             ";
        public static string ReviewsThatRefersAnSpecificGenre= @"
                            SELECT  r.Description, r.Grade,  r.Date,  m.Title,   m.Genre
                            FROM Review r
                            INNER JOIN Movie m ON m.Id = r.MovieId
                            WHERE m.Genre = @genre;
                             ";
    }
}
