using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Infrastructure.Queries
{
    public static class MovieQueries
    {
        public static string MostFamousMovieForYear= @"                  
                      SELECT TOP 1 m.Title, m.Description, m.ReleaseDate, m.Length, m.Genre, COUNT(r.Id) AS TotalReviews
                      FROM Movie m
                      INNER JOIN Review r ON r.MovieId = m.Id
                      WHERE YEAR(m.ReleaseDate) = @year
                      GROUP BY m.Title, m.Description, m.ReleaseDate, m.Length, m.Genre
                      ORDER BY COUNT(r.Id) DESC;
                        ";
        public static string Top10MoviesThatHasMostActors= @"
                     Select top 10 m.title, m.description, m.ReleaseDate, m.Length, m.Genre, count(a.MovieId) as TotalActores
                     from Movie m inner join Actor a on a.MovieId=m.Id
                     group by m.title, m.description, m.ReleaseDate, m.Length, m.Genre
                     order by count(a.MovieId) desc;
                         ";
    }
}
