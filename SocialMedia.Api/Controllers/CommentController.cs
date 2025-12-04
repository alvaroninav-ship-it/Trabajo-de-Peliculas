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
        [Authorize(Roles =nameof(RoleType.Administrator))]
        [HttpGet("dto/mapper")]
        public async Task<IActionResult> GetCommentDtoMapper(
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

        /// <summary>
        /// Recupera un objeto de transferencia de datos segun filtro
        /// </summary>
        /// <remarks>
        /// Este metodo se utiliza para convertir un comment en DTO que luego se 
        /// devuelve
        /// </remarks>
        /// <param name="id">El unico filtro de un id para recuperar a un comentario, 
        /// <param name="idAux">Identificador de la tabla</param>>
        /// <returns>Un objeto comment</returns>
        /// <responsecode="200">Retorna el comentario correctamente</responsecode>
        /// <responsecode="404">No se encontro el dato</responsecode>
        /// <responsecode="500">Hubo un error al buscar el dato</responsecode>
        /// <responsecode="404">Error por mal ingreso del dato</responsecode>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<CommentDto>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [Authorize(Roles =nameof(RoleType.Administrator))]
        [HttpGet("dto/mapper/{id}")]
        public async Task<IActionResult> GetCommentsDtoMapperId(int id,int idAux)
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

        /// <summary>
        /// Insertar un objeto enviado en formato json para ser agregado y registrado
        /// </summary>
        /// <remarks>
        /// Este metodo se usa para insertar un objeto enviado en formato json
        /// </remarks>
        /// <param name="commentDto">El objeto comment dto que solo permite ingresar datos validos, 
        /// <param name="idAux">Identificador de la tabla</param>>
        /// <returns>El objeto insertado retornado</returns>
        /// <responsecode="200">Retorna el registro insertado</responsecode>
        /// <responsecode="500">Hubo un error al insertar el dato</responsecode>
        /// <responsecode="404">Error por mal ingreso del dato</responsecode>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<Comment>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [Authorize(Roles = $"{nameof(RoleType.Administrator)},{nameof(RoleType.User)}")]
        [HttpPost("dto/mapper/")]
        public async Task<IActionResult> InsertCommentDtoMapper([FromBody] CommentDto commentDto,int idAux)
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

        /// <summary>
        /// Actualiza los datos de un comentario existente por su id
        /// </summary>
        /// <remarks>
        /// Este metodo se utiliza actualizar un comentario por envio de datos nuevos pero el id no cambia
        /// </remarks>
        /// <param name="commentDto">El commentDto que se manda para actualizar a un comentario existente registrado,
        /// <param name="id">El identificador unico del objeto a actualizar</param>
        /// <param name="idAux">Identificador de la tabla</param>>
        /// <returns>Un objeto actor actualizado</returns>
        /// <responsecode="200">El comentario fue actualizado con los datos enviados</responsecode>
        /// <responsecode="404">No se encontro el dato</responsecode>
        /// <responsecode="500">Hubo un error al actualizar el dato</responsecode>
        /// <responsecode="404">Error por mal ingreso del dato</responsecode>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<Comment>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [Authorize(Roles = $"{nameof(RoleType.Administrator)},{nameof(RoleType.User)}")]
        [HttpPut("dto/mapper/{id}")]
        public async Task<IActionResult> UpdateCommentDtoMapper(int id,
            [FromBody] CommentDto commentDto,int idAux)
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

        /// <summary>
        /// Elimina a un objeto actor de los registros
        /// </summary>
        /// <remarks>
        /// Este metodo se utiliza para eliminar a un objeto comentario
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
        [HttpDelete("dto/mapper/{id}")]
        [Authorize(Roles = $"{nameof(RoleType.Administrator)},{nameof(RoleType.User)}")]
        public async Task<IActionResult> DeleteCommentDtoMapper(int id, int idAux)
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
