using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Movies.Core.CustomEntities;
using Movies.Core.Entities;

namespace Movies.Core.Interfaces
{
    public interface IMovieRepository:IBaseRepository<Movie>
    {
        Task<Movie> GetMovieByTittle(string tittle);
        Task<MostFamousMovieForYear> GetMostFamousMovieForYear(int year);

        Task<IEnumerable<Top10MoviesThatHasMostActors>> Gettop10MoviesThatHasMostActors();
    }
}
