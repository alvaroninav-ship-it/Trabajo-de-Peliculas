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



        [HttpGet("dto/mapper/{id}")]
        public async Task<IActionResult> GetUsersDtoMapperId(int id)
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

        [HttpPost("dto/mapper/")]
        public async Task<IActionResult> InsertUserDtoMapper([FromBody] UserDto userDto)
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

        [HttpDelete("dto/mapper/{id}")]
        public async Task<IActionResult> DeleteCommentDtoMapper(int id)
        {
            var user = await _userRepository.GetUserAsync(id);
            if (user == null)
                return NotFound("User no encontrado");

            await _userRepository.DeleteUserAsync(user);


            return NoContent();
        }

        [HttpGet("dapper/1")]
        public async Task<IActionResult> GetTop10UsersMostCommentedInTheirReview()
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
        [HttpGet("dapper/2")]
        public async Task<IActionResult> GetTop10UsersThatHasDoneMoreComments()
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
        [HttpGet("dapper/3")]
        public async Task<IActionResult> GetUsersThatReviewLastYearMovies()
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