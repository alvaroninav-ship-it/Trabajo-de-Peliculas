using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Movies.Api.Responses;
using Movies.Core.CustomEntities;
using Movies.Core.Entities;
using Movies.Core.Enum;
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
        // Alv
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
        [Authorize(Roles = $"{nameof(RoleType.Administrator)},{nameof(RoleType.Provider)}, {nameof(RoleType.User)}")]
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


        /// <summary>
        /// Recupera un objeto de transferencia de datos segun filtro
        /// </summary>
        /// <remarks>
        /// Este metodo se utiliza para convertir una pelicula en DTO que luego se 
        /// devuelve
        /// </remarks>
        /// <param name="id">El unico filtro de un id para recuperar a una pelicula, 
        /// <param name="idAux">Identificador de la tabla</param>>
        /// <returns>Un objeto actor</returns>
        /// <responsecode="200">Retorna la pelicula correctamente</responsecode>
        /// <responsecode="404">No se encontro el dato</responsecode>
        /// <responsecode="500">Hubo un error al buscar el dato</responsecode>
        /// <responsecode="404">Error por mal ingreso del dato</responsecode>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<MovieDto>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [Authorize(Roles = $"{nameof(RoleType.Administrator)},{nameof(RoleType.Provider)}, {nameof(RoleType.User)}")]
        [HttpGet("dto/mapper/{id}")]
        public async Task<IActionResult> GetMoviesDtoMapperId(int id,int idAux)
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


        /// <summary>
        /// Insertar un objeto enviado en formato json para ser agregado y registrado
        /// </summary>
        /// <remarks>
        /// Este metodo se usa para insertar un objeto enviado en formato json
        /// </remarks>
        /// <param name="movieDto">El objeto pelicula dto que solo permite ingresar datos validos, 
        /// <param name="idAux">Identificador de la tabla</param>>
        /// <returns>El objeto insertado retornado</returns>
        /// <responsecode="200">Retorna el registro insertado</responsecode>
        /// <responsecode="500">Hubo un error al insertar el dato</responsecode>
        /// <responsecode="404">Error por mal ingreso del dato</responsecode>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<Movie>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [Authorize(Roles = nameof(RoleType.Provider))]
        [HttpPost("dto/mapper/")]
        public async Task<IActionResult> InsertMovieDtoMapper([FromBody] MovieDto movieDto,int idAux)
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


        /// <summary>
        /// Actualiza los datos de una pelicula existente por su id
        /// </summary>
        /// <remarks>
        /// Este metodo se utiliza actualizar una pelicula por envio de datos nuevos pero el id no cambia
        /// </remarks>
        /// <param name="movieDto">El MovieDto que se manda para actualizar a una pelicula existente registrado,
        /// <param name="id">El identificador unico del objeto a actualizar</param>
        /// <param name="idAux">Identificador de la tabla</param>>
        /// <returns>Un objeto pelicula actualizado</returns>
        /// <responsecode="200">La pelicula fue actualizado con los datos enviados</responsecode>
        /// <responsecode="404">No se encontro el dato</responsecode>
        /// <responsecode="500">Hubo un error al actualizar el dato</responsecode>
        /// <responsecode="404">Error por mal ingreso del dato</responsecode>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<Movie>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [Authorize(Roles = nameof(RoleType.Provider))]
        [HttpPut("dto/mapper/{id}")]
        public async Task<IActionResult> UpdateMovieDtoMapper(int id,
            [FromBody] MovieDto movieDto, int idAux)
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

        /// <summary>
        /// Elimina a un objeto actor de los registros
        /// </summary>
        /// <remarks>
        /// Este metodo se utiliza para eliminar a un objeto pelicula
        /// </remarks>
        /// <param name="id">El id con el cual se buscara al objeto para eliminarlo, 
        /// <param name="idAux">Identificador de la tabla</param>>
        /// si no se encuentra se manda no encontrado</param>
        /// <returns>No hay contenido de vuelta</returns>
        /// <responsecode="200">El objeto fue eliminado</responsecode>
        /// <responsecode="404">No se encontro el dato</responsecode>
        /// <responsecode="500">Hubo un error al eliminar el dato</responsecode>
        /// <responsecode="404">Error por mal ingreso del dato</responsecode>
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [Authorize(Roles = $"{nameof(RoleType.Administrator)},{nameof(RoleType.Provider)}")]
        [HttpDelete("dto/mapper/{id}")]
        public async Task<IActionResult> DeleteMovieDtoMapper(int id, int idAux)
        {
            var movie = await _movieServices.GetMovieAsync(id);
            if (movie == null)
                return NotFound("Pelicula no encontrada");

            await _movieServices.DeleteMovieAsync(movie);


            return NoContent();
        }

        /// <summary>
        /// Obtiene la pelicula mas famosa por un ano ingresado
        /// </summary>
        /// <remarks>
        /// Este metodo se utiliza para obtener un reporte acerca de peliculas por ano
        /// </remarks>
        /// <param name="year">Ano por el cual se filtrara la pelicula mas famosa del ese ano</param>
        /// <param name="idAux">Identificador de la tabla</param>>
        /// <returns>Una coleccion de reportes de actores de los mas jovenes</returns>
        /// <responsecode="200">Un unico dato</responsecode>
        /// <responsecode="404">No se encontraron los dato</responsecode>
        /// <responsecode="500">Hubo un error al encontrar los dato</responsecode>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<MostFamousMovieForYear>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [Authorize(Roles = $"{nameof(RoleType.Administrator)},{nameof(RoleType.User)}")]
        [HttpGet("dapper/1/{year}")]
        public async Task<IActionResult> GetMostFamousMovieForYear(int year,int idAux)
        {
            var movie = await _movieServices.GetMostFamousMovieForYear(year);

            var response = new ApiResponse<MostFamousMovieForYear>(movie);

            return Ok(response);
        }

        /// <summary>
        /// Obtiene las peliculas que mas actorres tiene en su casting
        /// </summary>
        /// <remarks>
        /// Este metodo se utiliza para obtener un reporte acerca de las peliculas por numero de actores participantes
        /// </remarks>
        /// <param name="idAux">Identificador de la tabla</param>>
        /// <returns>Una coleccion de reportes de las pelculas con mas actores</returns>
        /// <responsecode="200">Coleccion de datos</responsecode>
        /// <responsecode="404">No se encontraron los dato</responsecode>
        /// <responsecode="500">Hubo un error al encontrar los dato</responsecode>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<Top10MoviesThatHasMostActors>>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [Authorize(Roles = $"{nameof(RoleType.Administrator)},{nameof(RoleType.User)}")]
        [HttpGet("dapper/2")]
        public async Task<IActionResult> Gettop10MoviesThatHasMostActors(int idAux)
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