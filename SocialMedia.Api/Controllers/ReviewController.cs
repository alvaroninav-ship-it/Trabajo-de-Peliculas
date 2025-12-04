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
    public class ReviewController : ControllerBase
    {
        private readonly IReviewServices _reviewServices;
        private readonly IMapper _mapper;
        private readonly IValidationService _validationService;
        public ReviewController(IReviewServices reviewServices,
            IMapper mapper, IValidationService validationService)
        {
            _reviewServices = reviewServices;
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
        /// <param name="reviewQueryFilter">Los filtros de aplican al recuperar las peliculas como paginacion y busqueda, 
        /// <param name="idAux">Identificador de la tabla</param>>
        /// si no se envian los parametros se retornan todos los registros</param>
        /// <returns>Coleccion o lista de review</returns>
        /// <responsecode="200">Retorna todos lo registros</responsecode>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<ReviewDto>>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [Authorize(Roles = $"{nameof(RoleType.Administrator)},{nameof(RoleType.Provider)}, {nameof(RoleType.User)}")]
        [HttpGet("dto/mapper")]
        public async Task<IActionResult> GetReviewDtoMapper(
           [FromQuery] ReviewQueryFilter reviewQueryFilter, int idAux)
        {
            try
            {
                var reviews = await _reviewServices.GetAllReviewAsync(reviewQueryFilter);

                var reviewsDto = _mapper.Map<IEnumerable<ReviewDto>>(reviews.Pagination);

                var pagination = new Pagination
                {
                    TotalCount = reviews.Pagination.TotalCount,
                    PageSize = reviews.Pagination.PageSize,
                    CurrentPage = reviews.Pagination.CurrentPage,
                    TotalPages = reviews.Pagination.TotalPages,
                    HasNextPage = reviews.Pagination.HasNextPage,
                    HasPreviousPage = reviews.Pagination.HasPreviousPage
                };
                var response = new ApiResponse<IEnumerable<ReviewDto>>(reviewsDto)
                {
                    Pagination = pagination,
                    Messages = reviews.Messages
                };

                return StatusCode((int)reviews.StatusCode, response);
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
        /// Este metodo se utiliza para convertir una critica en DTO que luego se 
        /// devuelve
        /// </remarks>
        /// <param name="id">El unico filtro de un id para recuperar a una critica, 
        /// <param name="idAux">Identificador de la tabla</param>>
        /// <returns>Un objeto review</returns>
        /// <responsecode="200">Retorna la review correctamente</responsecode>
        /// <responsecode="404">No se encontro el dato</responsecode>
        /// <responsecode="500">Hubo un error al buscar el dato</responsecode>
        /// <responsecode="404">Error por mal ingreso del dato</responsecode>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<ReviewDto>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [Authorize(Roles = $"{nameof(RoleType.Administrator)},{nameof(RoleType.User)}")]
        [HttpGet("dto/mapper/{id}")]
        public async Task<IActionResult> GetReviewsDtoMapperId(int id,int idAux)
        {
            try
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

                var review = await _reviewServices.GetReviewAsync(id);
                var reviewDto = _mapper.Map<ReviewDto>(review);

                var response = new ApiResponse<ReviewDto>(reviewDto);

                return Ok(response);
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Insertar un objeto enviado en formato json para ser agregado y registrado
        /// </summary>
        /// <remarks>
        /// Este metodo se usa para insertar un objeto enviado en formato json
        /// </remarks>
        /// <param name="reviewDto">El objeto review dto que solo permite ingresar datos validos, 
        /// <param name="idAux">Identificador de la tabla</param>>
        /// <returns>El objeto insertado retornado</returns>
        /// <responsecode="200">Retorna el registro insertado</responsecode>
        /// <responsecode="500">Hubo un error al insertar el dato</responsecode>
        /// <responsecode="404">Error por mal ingreso del dato</responsecode>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<Actor>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [Authorize(Roles = nameof(RoleType.User))]
        [HttpPost("dto/mapper/")]
        public async Task<IActionResult> InsertReviewDtoMapper([FromBody] ReviewDto reviewDto,int idAux)
        {
            try
            {
                #region Validaciones
                // La validación automática se hace mediante el filtro
                // Esta validación manual es opcional
                var validationResult = await _validationService.ValidateAsync(reviewDto);

                if (!validationResult.IsValid)
                {
                    return BadRequest(new { Errors = validationResult.Errors });
                }
                #endregion

                var review = _mapper.Map<Review>(reviewDto);
                await _reviewServices.InsertReviewByUserAsync(review);

                var response = new ApiResponse<Review>(review);
                return Ok(response);
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,err.Message);
            }
        }



        /// <summary>
        /// Actualiza los datos de una review existente por su id
        /// </summary>
        /// <remarks>
        /// Este metodo se utiliza actualizar una critica por envio de datos nuevos pero el id no cambia
        /// </remarks>
        /// <param name="reviewDto">El ReviewDto que se manda para actualizar a una critica existente registrada,
        /// <param name="id">El identificador unico del objeto a actualizar</param>
        /// <param name="idAux">Identificador de la tabla</param>>
        /// <returns>Un objeto critica actualizado</returns>
        /// <responsecode="200">La critica fue actualizado con los datos enviados</responsecode>
        /// <responsecode="404">No se encontro el dato</responsecode>
        /// <responsecode="500">Hubo un error al actualizar el dato</responsecode>
        /// <responsecode="404">Error por mal ingreso del dato</responsecode>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<Review>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [Authorize(Roles = nameof(RoleType.User))]
        [HttpPut("dto/mapper/{id}")]
        public async Task<IActionResult> UpdateReviewDtoMapper(int id,
            [FromBody] ReviewDto reviewDto,int idAux)
        {
            try
            {
                var validations= await _validationService.ValidateAsync(reviewDto);
                if(!validations.IsValid)
                {
                    return BadRequest(new { Errors = validations.Errors });
                }
                if (id != reviewDto.Id)
                    return BadRequest("El Id del Review no coincide");

                var review = await _reviewServices.GetReviewAsync(id);
                if (review == null)
                    return NotFound("Review no encontrada");

                _mapper.Map(reviewDto, review);
                await _reviewServices.UpdateReviewAsync(review);

                var response = new ApiResponse<Review>(review);
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
        /// Este metodo se utiliza para eliminar a un objeto critica
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
        public async Task<IActionResult> DeleteReviewDtoMapper(int id,int idAux)
        {
            var review = await _reviewServices.GetReviewAsync(id);
            if (review == null)
                return NotFound("Review no encontrado");

            await _reviewServices.DeleteReviewAsync(review);


            return NoContent();
        }

        /// <summary>
        /// Obtiene las criticas en base a un genero especifico
        /// </summary>
        /// <remarks>
        /// Este metodo se utiliza para obtener un reporte acerca de un genero de peliculas como las criticas que ha recibido
        /// </remarks>
        /// <param name="genre">Genero con el cual se hara la filtracion de reviews</param>
        /// <param name="idAux">Identificador de la tabla</param>>
        /// <returns>Una coleccion reviews a peliculas de cierto genero</returns>
        /// <responsecode="200">Coleccion de datos</responsecode>
        /// <responsecode="404">No se encontraron los dato</responsecode>
        /// <responsecode="500">Hubo un error al encontrar los dato</responsecode>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<ReviewsThatRefersAnSpecificGenre>>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [Authorize(Roles = $"{nameof(RoleType.Administrator)},{nameof(RoleType.User)}")]
        [HttpGet("dapper/1/{genre}")]
        public async Task<IActionResult> GetReviewsThatRefersAnSpecificGenre(string genre,int idAux)
        {
            var reviews = await _reviewServices.GetReviewsThatRefersAnSpecificGenre(genre);

            var totalCount = reviews.Count();
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
            var response = new ApiResponse<IEnumerable<ReviewsThatRefersAnSpecificGenre>>(reviews)
            {
                Pagination = pagination,
                Messages = reviews.Any()
                    ? null
                    : new[] { new Message { Type = "Warning", Description = "No se encontraron criticas" } }
            };
            return Ok(response);
        }

        /// <summary>
        /// Obtiene las criticas de publico menor
        /// </summary>
        /// <remarks>
        /// Este metodo se utiliza para obtener un reporte acerca de personas menores a 20 anos y su opinion en general
        /// </remarks>
        /// <param name="idAux">Identificador de la tabla</param>>
        /// <returns>Una coleccion reviews por parte del publico joven</returns>
        /// <responsecode="200">Coleccion de datos</responsecode>
        /// <responsecode="404">No se encontraron los dato</responsecode>
        /// <responsecode="500">Hubo un error al encontrar los dato</responsecode>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<ReviewsThatWereDoneByUsers20YearsOldOrYounger>>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [Authorize(Roles = nameof(RoleType.Administrator))]
        [HttpGet("dapper/2")]
        public async Task<IActionResult> GetReviewsThatWereDoneByUsers20YearsOldOrYounger(int idAux)
        {
            var reviews = await _reviewServices.GetReviewsThatWereDoneByUsers20YearsOldOrYounger();

            var totalCount = reviews.Count();
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
            var response = new ApiResponse<IEnumerable<ReviewsThatWereDoneByUsers20YearsOldOrYounger>>(reviews)
            {
                Pagination = pagination,
                Messages = reviews.Any()
                    ? null
                    : new[] { new Message { Type = "Warning", Description = "No se encontraron criticas" } }
            };
            return Ok(response);
        }

        /// <summary>
        /// Obtiene las criticas mas conocidas o mas respondidas 
        /// </summary>
        /// <remarks>
        /// Este metodo se utiliza para obtener un reporte acerca criticas mas respondidas
        /// </remarks>
        /// <param name="idAux">Identificador de la tabla</param>>
        /// <returns>Una coleccion reviews que son las mas respondidas o mas famosas</returns>
        /// <responsecode="200">Coleccion de datos</responsecode>
        /// <responsecode="404">No se encontraron los dato</responsecode>
        /// <responsecode="500">Hubo un error al encontrar los dato</responsecode>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<Top10MostCommentedReviews>>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [Authorize(Roles = $"{nameof(RoleType.Administrator)},{nameof(RoleType.User)}")]
        [HttpGet("dapper/3")]
        public async Task<IActionResult> GetTop10MostCommentedReviews(int idAux)
        {
            var reviews = await _reviewServices.GetTop10MostCommentedReviews();

            var totalCount = reviews.Count();
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
            var response = new ApiResponse<IEnumerable<Top10MostCommentedReviews>>(reviews)
            {
                Pagination = pagination,
                Messages = reviews.Any()
                    ? null
                    : new[] { new Message { Type = "Warning", Description = "No se encontraron criticas" } }
            };

            return Ok(response);
        }
        #endregion
    }
}
