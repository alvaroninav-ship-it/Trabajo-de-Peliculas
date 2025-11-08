using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

using Movies.Api.Responses;
using Movies.Core.CustomEntities;
using Movies.Core.Entities;
using Movies.Core.Interfaces;
using Movies.Core.QueryFilters;
using Movies.Core.Services;
using Movies.Infrastructure.DTOs;
using Movies.Infrastructure.Validators;


namespace Movies.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieServices _movieServices;
        private readonly IMapper _mapper;
        private readonly IValidationService _validationService;
        public MovieController(IMovieServices movieServices,
            IMapper mapper,IValidationService validationService)
        {
            _movieServices = movieServices;
            _mapper = mapper;
            _validationService = validationService;
        }



        #region Dto Mapper
        /// <summary>
        /// Recupera una lista paginada de publicaciones como objetos de transferencia de datos segun filtro
        /// </summary>
        /// <remarks>
        /// Este metodo se utiliza para convertir las peliculas recuperadas en DTOs que luego se 
        /// devuelven en registros paginados
        /// </remarks>
        /// <param name="movieQueryFilter">Los filtros de aplican al recuperar las peliculas como paginacion y busqueda, 
        /// <param name="idAux">Identificador de la tabla</param>>
        /// si no se envian los parametros se retornan todos los registros</param>
        /// <returns>Coleccion o lista de movie</returns>
        /// <responsecode="200">Retorna todos lo registros</responsecode>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<MovieDto>>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpGet("dto/mapper")]
        public async Task<IActionResult> GetMovieDtoMapper(
           [FromQuery] MovieQueryFilter movieQueryFilter, int idAux)
        {
            try
            {
                var movies = await _movieServices.GetAllMovieAsync(movieQueryFilter);

                var moviesDto = _mapper.Map<IEnumerable<MovieDto>>(movies.Pagination);

                var pagination = new Pagination
                {
                    TotalCount = movies.Pagination.TotalCount,
                    PageSize = movies.Pagination.PageSize,
                    CurrentPage = movies.Pagination.CurrentPage,
                    TotalPages = movies.Pagination.TotalPages,
                    HasNextPage = movies.Pagination.HasNextPage,
                    HasPreviousPage = movies.Pagination.HasPreviousPage
                };
                var response = new ApiResponse<IEnumerable<MovieDto>>(moviesDto)
                {
                    Pagination = pagination,
                    Messages = movies.Messages
                };

                return StatusCode((int)movies.StatusCode, response);
            }
            catch (Exception err)
            {
                var responsePost = new ResponseData()
                {
                    Messages = new Message[] { new() { Type = "Error", Description = err.Message } },
                };
                return StatusCode(500, responsePost);
            }
        }



        [HttpGet("dto/mapper/{id}")]
        public async Task<IActionResult> GetMoviesDtoMapperId(int id)
        {
            #region Validaciones
            var validationRequest = new GetByIdRequest { Id = id };
            var validationResult = await _validationService.ValidateAsync(validationRequest);

            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Error de validación del ID",
                    Errors = validationResult.Errors
                });
            }
            #endregion

            var movie = await _movieServices.GetMovieAsync(id);
            var movieDto = _mapper.Map<MovieDto>(movie);

            var response = new ApiResponse<MovieDto>(movieDto);

            return Ok(response);
        }

        [HttpPost("dto/mapper/")]
        public async Task<IActionResult> InsertMovieDtoMapper([FromBody] MovieDto movieDto)
        {
            try
            {
                #region Validaciones
                // La validación automática se hace mediante el filtro
                // Esta validación manual es opcional
                var validationResult = await _validationService.ValidateAsync(movieDto);

                if (!validationResult.IsValid)
                {
                    return BadRequest(new { Errors = validationResult.Errors });
                }
                #endregion

                var movie = _mapper.Map<Movie>(movieDto);
                await _movieServices.InsertMovieJustOne(movie);

                var response = new ApiResponse<Movie>(movie);
                return Ok(response);
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
            }
        }

        [HttpPut("dto/mapper/{id}")]
        public async Task<IActionResult> UpdateMovieDtoMapper(int id,
            [FromBody] MovieDto movieDto)
        {
            try
            {
                var validationResult = await _validationService.ValidateAsync(movieDto);
                if(!validationResult.IsValid)
                {
                    return BadRequest(new { Errors = validationResult.Errors });
                }
                if (id != movieDto.Id)
                    return BadRequest("El Id de la pelicula no coincide");

                var movie = await _movieServices.GetMovieAsync(id);
                if (movie == null)
                    return NotFound("Pelicula no encontrado");

                _mapper.Map(movieDto, movie);
                await _movieServices.UpdateMovieAsync(movie);

                var response = new ApiResponse<Movie>(movie);
                return Ok(response);
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
            }
        }

        [HttpDelete("dto/mapper/{id}")]
        public async Task<IActionResult> DeleteMovieDtoMapper(int id)
        {
            var movie = await _movieServices.GetMovieAsync(id);
            if (movie == null)
                return NotFound("Pelicula no encontrada");

            await _movieServices.DeleteMovieAsync(movie);


            return NoContent();
        }


        [HttpGet("dapper/1/{year}")]
        public async Task<IActionResult> GetMostFamousMovieForYear(int year)
        {
            var movie = await _movieServices.GetMostFamousMovieForYear(year);

            var response = new ApiResponse<MostFamousMovieForYear>(movie);

            return Ok(response);
        }

        [HttpGet("dapper/2")]
        public async Task<IActionResult> Gettop10MoviesThatHasMostActors()
        {
            var movies = await _movieServices.Gettop10MoviesThatHasMostActors();
            var totalCount = movies.Count();
            var pagination = new Pagination
            {
                TotalCount = totalCount,
                PageSize = 10,
                CurrentPage = 1,
                TotalPages = 1,
                HasNextPage = false,
                HasPreviousPage = false
            };

            // 3️⃣ Crear respuesta API
            var response = new ApiResponse<IEnumerable<Top10MoviesThatHasMostActors>>(movies)
            {
                Pagination = pagination,
                Messages = movies.Any()
                    ? null
                    : new[] { new Message { Type = "Warning", Description = "No se encontraron peliculas" } }
            };

            return Ok(response);
        }
        #endregion
    }
}