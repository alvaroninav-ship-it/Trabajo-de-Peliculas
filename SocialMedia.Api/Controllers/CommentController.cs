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
    public class CommentController : ControllerBase
    {
        private readonly ICommentServices _commentServices; 
        private readonly IMapper _mapper;
        private readonly IValidationService _validationService;
        public CommentController(ICommentServices commentServices,
            IMapper mapper, IValidationService validationService)
        {
            _commentServices = commentServices;
            _mapper = mapper;
            _validationService = validationService;
        }


        #region Dto Mapper
        /// <summary>
        /// Recupera una lista paginada de publicaciones como objetos de transferencia de datos segun filtro
        /// </summary>
        /// <remarks>
        /// Este metodo se utiliza para convertir las publicaciones recuperadas en DTOs que luego se 
        /// devuelven en registros paginados
        /// </remarks>
        /// <param name="commentQueryFilter">Los filtros de aplican al recuperar los comentarios como la paginacion y busqueda, 
        /// <param name="idAux">Identificador de la tabla</param>>
        /// si no se envian los parametros se retornan todos los registros</param>
        /// <returns>Coleccion o lista de comment</returns>
        /// <responsecode="200">Retorna todos lo registros</responsecode>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<CommentDto>>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [HttpGet("dto/mapper")]
        public async Task<IActionResult> GetActorDtoMapper(
           [FromQuery] CommentQueryFilter commentQueryFilter, int idAux)
        {
            try
            {
                var comments = await _commentServices.GetAllCommentAsync(commentQueryFilter);

                var commentsDto = _mapper.Map<IEnumerable<CommentDto>>(comments.Pagination);

                var pagination = new Pagination
                {
                    TotalCount = comments.Pagination.TotalCount,
                    PageSize = comments.Pagination.PageSize,
                    CurrentPage = comments.Pagination.CurrentPage,
                    TotalPages = comments.Pagination.TotalPages,
                    HasNextPage = comments.Pagination.HasNextPage,
                    HasPreviousPage = comments.Pagination.HasPreviousPage
                };
                var response = new ApiResponse<IEnumerable<CommentDto>>(commentsDto)
                {
                    Pagination = pagination,
                    Messages = comments.Messages
                };

                return StatusCode((int)comments.StatusCode, response);
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
        public async Task<IActionResult> GetCommentsDtoMapperId(int id)
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

            var comment = await _commentServices.GetCommentAsync(id);
            var commentDto = _mapper.Map<CommentDto>(comment);

            var response = new ApiResponse<CommentDto>(commentDto);

            return Ok(response);
        }

        [HttpPost("dto/mapper/")]
        public async Task<IActionResult> InsertCommentDtoMapper([FromBody] CommentDto commentDto)
        {
            try
            {
                #region Validaciones
                // La validación automática se hace mediante el filtro
                // Esta validación manual es opcional
                var validationResult = await _validationService.ValidateAsync(commentDto);

                if (!validationResult.IsValid)
                {
                    return BadRequest(new { Errors = validationResult.Errors });
                }
                #endregion

                var comment = _mapper.Map<Comment>(commentDto);
                await _commentServices.InsertCommentAsync(comment);

                var response = new ApiResponse<Comment>(comment);
                return Ok(response);
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
            }
        }

        [HttpPut("dto/mapper/{id}")]
        public async Task<IActionResult> UpdateCommentDtoMapper(int id,
            [FromBody] CommentDto commentDto)
        {
            try
            {
                var validationResult = await _validationService.ValidateAsync(commentDto);
                if(!validationResult.IsValid)
                {
                    return BadRequest(new { Errors = validationResult.Errors });
                }
                if (id != commentDto.Id)
                    return BadRequest("El Id del Comentario no coincide");

                var comment = await _commentServices.GetCommentAsync(id);
                if (comment == null)
                    return NotFound("Comentario no encontrado");

                _mapper.Map(commentDto, comment);
                await _commentServices.UpdateCommentAsync(comment);

                var response = new ApiResponse<Comment>(comment);
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
            var comment = await _commentServices.GetCommentAsync(id);
            if (comment == null)
                return NotFound("Comentario no encontrado");

            await _commentServices.DeleteCommentAsync(comment);


            return NoContent();
        }

        #endregion

    }
}
