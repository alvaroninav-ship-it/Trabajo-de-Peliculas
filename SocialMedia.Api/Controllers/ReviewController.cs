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



        [HttpGet("dto/mapper/{id}")]
        public async Task<IActionResult> GetReviewsDtoMapperId(int id)
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

        [HttpPost("dto/mapper/")]
        public async Task<IActionResult> InsertReviewDtoMapper([FromBody] ReviewDto reviewDto)
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

        [HttpPut("dto/mapper/{id}")]
        public async Task<IActionResult> UpdateReviewDtoMapper(int id,
            [FromBody] ReviewDto reviewDto)
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

        [HttpDelete("dto/mapper/{id}")]
        public async Task<IActionResult> DeleteReviewDtoMapper(int id)
        {
            var review = await _reviewServices.GetReviewAsync(id);
            if (review == null)
                return NotFound("Review no encontrado");

            await _reviewServices.DeleteReviewAsync(review);


            return NoContent();
        }

        [HttpGet("dapper/1/{genre}")]
        public async Task<IActionResult> GetReviewsThatRefersAnSpecificGenre(string genre)
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
        [HttpGet("dapper/2")]
        public async Task<IActionResult> GetReviewsThatWereDoneByUsers20YearsOldOrYounger()
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
        [HttpGet("dapper/3")]
        public async Task<IActionResult> GetTop10MostCommentedReviews()
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
