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
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userRepository;
        private readonly IMapper _mapper;
        private readonly IValidationService _validationService;
        public UserController(IUserServices userRepository,
            IMapper mapper, IValidationService validationService)
        {
            _userRepository = userRepository;
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
        /// <param name="userQueryFilter">Los filtros de aplican al recuperar las peliculas como paginacion y busqueda, 
        /// <param name="idAux">Identificador de la tabla</param>>
        /// si no se envian los parametros se retornan todos los registros</param>
        /// <returns>Coleccion o lista de movie</returns>
        /// <responsecode="200">Retorna todos lo registros</responsecode>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<UserDto>>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [Authorize(Roles = $"{nameof(RoleType.Administrator)},{nameof(RoleType.User)}")]
        [HttpGet("dto/mapper")]
        public async Task<IActionResult> GetUserDtoMapper(
           [FromQuery] UserQueryFilter userQueryFilter, int idAux)
        {
            try
            {
                var movies = await _userRepository.GetAllUserAsync(userQueryFilter);

                var moviesDto = _mapper.Map<IEnumerable<UserDto>>(movies.Pagination);

                var pagination = new Pagination
                {
                    TotalCount = movies.Pagination.TotalCount,
                    PageSize = movies.Pagination.PageSize,
                    CurrentPage = movies.Pagination.CurrentPage,
                    TotalPages = movies.Pagination.TotalPages,
                    HasNextPage = movies.Pagination.HasNextPage,
                    HasPreviousPage = movies.Pagination.HasPreviousPage
                };
                var response = new ApiResponse<IEnumerable<UserDto>>(moviesDto)
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
        /// Este metodo se utiliza para convertir un usuario en DTO que luego se 
        /// devuelve
        /// </remarks>
        /// <param name="id">El unico filtro de un id para recuperar a un usuario, 
        /// <param name="idAux">Identificador de la tabla</param>>
        /// <returns>Un objeto usuario</returns>
        /// <responsecode="200">Retorna el usuario correctamente</responsecode>
        /// <responsecode="404">No se encontro el dato</responsecode>
        /// <responsecode="500">Hubo un error al buscar el dato</responsecode>
        /// <responsecode="404">Error por mal ingreso del dato</responsecode>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<UserDto>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [Authorize(Roles = $"{nameof(RoleType.Administrator)},{nameof(RoleType.User)}")]
        [HttpGet("dto/mapper/{id}")]
        public async Task<IActionResult> GetUsersDtoMapperId(int id,int idAux)
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

            var user = await _userRepository.GetUserAsync(id);
            var userDto = _mapper.Map<UserDto>(user);

            var response = new ApiResponse<UserDto>(userDto);

            return Ok(response);
        }

        /// <summary>
        /// Insertar un objeto enviado en formato json para ser agregado y registrado
        /// </summary>
        /// <remarks>
        /// Este metodo se usa para insertar un objeto enviado en formato json
        /// </remarks>
        /// <param name="userDto">El objeto user dto que solo permite ingresar datos validos, 
        /// <param name="idAux">Identificador de la tabla</param>>
        /// <returns>El objeto insertado retornado</returns>
        /// <responsecode="200">Retorna el registro insertado</responsecode>
        /// <responsecode="500">Hubo un error al insertar el dato</responsecode>
        /// <responsecode="404">Error por mal ingreso del dato</responsecode>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<User>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpPost("dto/mapper/")]
        public async Task<IActionResult> InsertUserDtoMapper([FromBody] UserDto userDto, int idAux)
        {
            try
            {
                #region Validaciones
                // La validación automática se hace mediante el filtro
                // Esta validación manual es opcional
                var validationResult = await _validationService.ValidateAsync(userDto);

                if (!validationResult.IsValid)
                {
                    return BadRequest(new { Errors = validationResult.Errors });
                }
                #endregion

                var user = _mapper.Map<User>(userDto);
                await _userRepository.InsertUserAsync(user);

                var response = new ApiResponse<User>(user);
                return Ok(response);
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
            }
        }


        /// <summary>
        /// Actualiza los datos de un usuario existente por su id
        /// </summary>
        /// <remarks>
        /// Este metodo se utiliza actualizar un usuario por envio de datos nuevos pero el id no cambia
        /// </remarks>
        /// <param name="reviewDto">El UserDto que se manda para actualizar a un usuario existente registrada,
        /// <param name="id">El identificador unico del objeto a actualizar</param>
        /// <param name="idAux">Identificador de la tabla</param>>
        /// <returns>Un objeto usuario actualizado</returns>
        /// <responsecode="200">El usuario fue actualizado con los datos enviados</responsecode>
        /// <responsecode="404">No se encontro el dato</responsecode>
        /// <responsecode="500">Hubo un error al actualizar el dato</responsecode>
        /// <responsecode="404">Error por mal ingreso del dato</responsecode>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<User>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [Authorize(Roles = nameof(RoleType.User))]
        [HttpPut("dto/mapper/{id}")]
        public async Task<IActionResult> UpdateUserDtoMapper(int id,
            [FromBody] UserDto userDto)
        {
            try
            {
                var validationResult = await _validationService.ValidateAsync(userDto);
                if(!validationResult.IsValid)
                {
                    return BadRequest(new { Errors = validationResult.Errors });
                }
                if (id != userDto.Id)
                    return BadRequest("El Id del User no coincide");

                var user = await _userRepository.GetUserAsync(id);
                if (user == null)
                    return NotFound("User no encontrado");

                _mapper.Map(userDto, user);
                await _userRepository.UpdateUserAsync(user);

                var response = new ApiResponse<User>(user);
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
        /// Este metodo se utiliza para eliminar a un objeto user
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
        [Authorize(Roles = $"{nameof(RoleType.Administrator)},{nameof(RoleType.User)}")]
        [HttpDelete("dto/mapper/{id}")]
        public async Task<IActionResult> DeleteUserDtoMapper(int id, int idAux)
        {
            var user = await _userRepository.GetUserAsync(id);
            if (user == null)
                return NotFound("User no encontrado");

            await _userRepository.DeleteUserAsync(user);


            return NoContent();
        }

        /// <summary>
        /// Obtiene los usuarios que mas respuestas han obtenido en base a sus criticas
        /// </summary>
        /// <remarks>
        /// Este metodo se utiliza para obtener un reporte acerca de los usuarios mas respondidos
        /// </remarks>
        /// <param name="idAux">Identificador de la tabla</param>>
        /// <returns>Una coleccion usuarios que han sido mas respondidas o mas famosas</returns>
        /// <responsecode="200">Coleccion de datos</responsecode>
        /// <responsecode="404">No se encontraron los dato</responsecode>
        /// <responsecode="500">Hubo un error al encontrar los dato</responsecode>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<Top10UsersMostCommentedInTheirReview>>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [Authorize(Roles = $"{nameof(RoleType.Administrator)},{nameof(RoleType.User)}")]
        [HttpGet("dapper/1")]
        public async Task<IActionResult> GetTop10UsersMostCommentedInTheirReview(int idAux)
        {
            var users = await _userRepository.GetTop10UsersMostCommentedInTheirReview();
            var totalCount = users.Count();
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
            var response = new ApiResponse<IEnumerable<Top10UsersMostCommentedInTheirReview>>(users)
            {
                Pagination = pagination,
                Messages = users.Any()
                    ? null
                    : new[] { new Message { Type = "Warning", Description = "No se encontraron usuarios" } }
            };
            return Ok(response);
        }

        /// <summary>
        /// Obtiene los usuarios que mas han participado en la comunidad en base a sus comentarios hechos
        /// </summary>
        /// <remarks>
        /// Este metodo se utiliza para obtener un reporte acerca de los usuarios que mas han participado en las reviews de otros usuarios
        /// </remarks>
        /// <param name="idAux">Identificador de la tabla</param>>
        /// <returns>Una coleccion usuarios que mas comentarios han realizado</returns>
        /// <responsecode="200">Coleccion de datos</responsecode>
        /// <responsecode="404">No se encontraron los dato</responsecode>
        /// <responsecode="500">Hubo un error al encontrar los dato</responsecode>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<Top10UsersThatHasDoneMoreComments>>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [Authorize(Roles = $"{nameof(RoleType.Administrator)},{nameof(RoleType.User)}")]
        [HttpGet("dapper/2")]
        public async Task<IActionResult> GetTop10UsersThatHasDoneMoreComments(int idAux)
        {
            var users = await _userRepository.GetTop10UsersThatHasDoneMoreComments();

            var totalCount = users.Count();
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
            var response = new ApiResponse<IEnumerable<Top10UsersThatHasDoneMoreComments>>(users)
            {
                Pagination = pagination,
                Messages = users.Any()
                    ? null
                    : new[] { new Message { Type = "Warning", Description = "No se encontraron usuarios" } }
            };
            return Ok(response);
        }

        /// <summary>
        /// Obtiene los usuarios que han criticado a peliculas del ultimo ano
        /// </summary>
        /// <remarks>
        /// Este metodo se utiliza para obtener un reporte acerca de como los usuarios han participado en este ultimo ano
        /// </remarks>
        /// <param name="idAux">Identificador de la tabla</param>>
        /// <returns>Una coleccion usuarios que comentaron en peliculas del ultimo ano</returns>
        /// <responsecode="200">Coleccion de datos</responsecode>
        /// <responsecode="404">No se encontraron los dato</responsecode>
        /// <responsecode="500">Hubo un error al encontrar los dato</responsecode>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<UsersThatReviewLastYearMovies>>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [Authorize(Roles = $"{nameof(RoleType.Administrator)},{nameof(RoleType.User)}")]
        [HttpGet("dapper/3")]
        public async Task<IActionResult> GetUsersThatReviewLastYearMovies(int idAux)
        {
            var users = await _userRepository.GetUsersThatReviewLastYearMovies();

            var totalCount = users.Count();
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
            var response = new ApiResponse<IEnumerable<UsersThatReviewLastYearMovies>>(users)
            {
                Pagination = pagination,
                Messages = users.Any()
                    ? null
                    : new[] { new Message { Type = "Warning", Description = "No se encontraron usuarios" } }
            };

            return Ok(response);
        }
        #endregion
    }
}