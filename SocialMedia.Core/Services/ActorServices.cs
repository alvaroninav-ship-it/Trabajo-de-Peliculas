using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using Movies.Core.CustomEntities;
using Movies.Core.Entities;
using Movies.Core.Exceptions;
using Movies.Core.Interfaces;
using Movies.Core.QueryFilters;

namespace Movies.Core.Services
{

    public class ActorServices : IActorServices
    {
        public readonly IUnitOfWork _unitOfWork;
        public ActorServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseData> GetAllActorAsync(
            ActorQueryFilter actorQueryFilter)
        {
            var actors = await _unitOfWork.ActorRepository.GetAll();

            if (actorQueryFilter.MovieId != null)
            {
                actors = actors.Where(a => a.MovieId == actorQueryFilter.MovieId);
            }
            if (actorQueryFilter.DateOfBirth != null)
            {
                actors = actors.Where(a => a.DateOfBirth.ToShortDateString() ==
                actorQueryFilter.DateOfBirth);
            }
            if (actorQueryFilter.IsActive != null)
            {
                actors = actors.Where(a =>
                a.IsActive == actorQueryFilter.IsActive);
            }

            if (actorQueryFilter.FirstName != null)
            {
                actors = actors.Where(a => a.FirstName == actorQueryFilter.FirstName);
            }
            if (actorQueryFilter.LastName != null)
            {
                actors = actors.Where(a => a.LastName == actorQueryFilter.LastName);
            }
            
            var totalCount = actors.Count();

          
            var pageActors = PageList<object>.Create(actors, actorQueryFilter.PageNumber, actorQueryFilter.PageSize);

            if (pageActors.Any())
            {
                return new ResponseData()
                {
                    Messages = new Message[] { new() { Type = "Information", Description = "Registros de actores recuperados correctamente" } },
                    Pagination = pageActors,
                    StatusCode = HttpStatusCode.OK,
                    totalcount= totalCount
                };
            }
            else
            {
                return new ResponseData()
                {
                    Messages = new Message[] { new() { Type = "Warning", Description = "No fue posible recuperar la cantidad de registros" } },
                    Pagination = pageActors,
                    StatusCode = HttpStatusCode.OK
                };
            }
        }
        public async Task DeleteActorAsync(Actor actor)
        {
            await _unitOfWork.ActorRepository.Delete(actor.Id);
        }

        public async Task<IEnumerable<Actor>> GetAllActorDapperAsync()
        {
            return await _unitOfWork.ActorRepository.GetAll();
        }

        public Task<Actor> GetActorAsync(int id)
        {
            return _unitOfWork.ActorRepository.GetById(id);
        }

        public Task InsertActorAsync(Actor actor)
        {
            return _unitOfWork.ActorRepository.Add(actor);
        }

        public async Task UpdateActorAsync(Actor actor)
        {
            var movie = await _unitOfWork.MovieRepository.GetById(actor.Id);
            if (movie == null)
            {
                throw new BusinessException("La pelicula no existe");
            }
            await _unitOfWork.ActorRepository.Update(actor);
        }

        public async Task<IEnumerable<Top10TheYoungestActors>> GetTop10TheYoungestActors()
        {
            var actors = await _unitOfWork.ActorRepository.GetTop10TheYoungestActors();
            return actors;
        }
    }
}
