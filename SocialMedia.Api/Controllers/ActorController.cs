using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Movies.Api.Responses;
using Movies.Core.CustomEntities;
using Movies.Core.Entities;
using Movies.Core.Enum;
using Movies.Core.Interfaces;
using Movies.Core.QueryFilters;
using Movies.Infrastructure.DTOs;
using Movies.Infrastructure.Validators;

namespace Movies.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActorController : ControllerBase
    {
        private readonly IActorServices _actorServices;
        private readonly IMapper _mapper;
        private readonly IValidationService _validationService;
        public ActorController(IActorServices actorServices,
            IMapper mapper, IValidationService validationService)
        {
            _actorServices = actorServices;
            _mapper = mapper;
            _validationService = validationService;
        }

        #region Dto Mapper

        /// <summary>
        /// Recupera una lista paginada de publicaciones como objetos de transferencia de datos segun filtro
        /// </summary>
        /// <remarks>
        /// Este metodo se utiliza para convertir los actores recuperadas en DTOs que luego se 
        /// devuelven en registros paginados
        /// </remarks>
        /// <param name="actorQueryFilter">Los filtros de aplican al recuperar los actores como la paginacion y busqueda, 
        /// <param name="idAux">Identificador de la tabla</param>>
        /// si no se envian los parametros se retornan todos los registros</param>
        /// <returns>Coleccion o lista de actor</returns>
        /// <responsecode="200">Retorna todos lo registros</responsecode>
        /// <responsecode="404">No se encontro el dato</responsecode>
        /// <responsecode="500">Hubo un error al eliminar el dato</responsecode>
        /// <responsecode="400">Error por mal ingreso del dato</responsecode>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<ActorDto>>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [Authorize(Roles = $"{nameof(RoleType.Administrator)},{nameof(RoleType.Provider)}")]
        [HttpGet("dto/mapper")]
        public async Task<IActionResult> GetActorDtoMapper(
           [FromQuery] ActorQueryFilter actorQueryFilter,int idAux)
        {
            try
            {
                var actors = await _actorServices.GetAllActorAsync(actorQueryFilter);

                var actorsDto = _mapper.Map<IEnumerable<ActorDto>>(actors.Pagination);

                var pagination = new Pagination
                {
                    TotalCount = actors.Pagination.TotalCount,
                    PageSize = actors.Pagination.PageSize,
                    CurrentPage = actors.Pagination.CurrentPage,
                    TotalPages = actors.Pagination.TotalPages,
                    HasNextPage = actors.Pagination.HasNextPage,
                    HasPreviousPage = actors.Pagination.HasPreviousPage
                };
                var response = new ApiResponse<IEnumerable<ActorDto>>(actorsDto)
                {
                    Pagination = pagination,
                    Messages = actors.Messages
                };

                return StatusCode((int)actors.StatusCode, response);
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
        /// Este metodo se utiliza para convertir un actor en DTO que luego se 
        /// devuelve
        /// </remarks>
        /// <param name="id">El unico filtro de un id para recuperar a un actor, 
        /// <param name="idAux">Identificador de la tabla</param>>
        /// <returns>Un objeto actor</returns>
        /// <responsecode="200">Retorna el actor correctamente</responsecode>
        /// <responsecode="404">No se encontro el dato</responsecode>
        /// <responsecode="500">Hubo un error al buscar el dato</responsecode>
        /// <responsecode="404">Error por mal ingreso del dato</responsecode>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<ActorDto>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [Authorize(Roles = $"{nameof(RoleType.Administrator)},{nameof(RoleType.Provider)}")]
        [HttpGet("dto/mapper/{id}")]
        public async Task<IActionResult> GetActorsDtoMapperId(int id,int idAux)
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

            var actor = await _actorServices.GetActorAsync(id);
            var actorDto = _mapper.Map<ActorDto>(actor);

            var response = new ApiResponse<ActorDto>(actorDto);

            return Ok(response);
        }

        /// <summary>
        /// Insertar un objeto enviado en formato json para ser agregado y registrado
        /// </summary>
        /// <remarks>
        /// Este metodo se usa para insertar un objeto enviado en formato json
        /// </remarks>
        /// <param name="actorDto">El objeto actor dto que solo permite ingresar datos validos, 
        /// <param name="idAux">Identificador de la tabla</param>>
        /// <returns>El objeto insertado retornado</returns>
        /// <responsecode="200">Retorna el registro insertado</responsecode>
        /// <responsecode="500">Hubo un error al insertar el dato</responsecode>
        /// <responsecode="404">Error por mal ingreso del dato</responsecode>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<Actor>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [Authorize(Roles = nameof(RoleType.Provider))]
        [HttpPost("dto/mapper/")]
        public async Task<IActionResult> InsertActorDtoMapper([FromBody] ActorDto actorDto,int idAux)
        {
            try
            {
                #region Validaciones
                // La validación automática se hace mediante el filtro
                // Esta validación manual es opcional
                var validationResult = await _validationService.ValidateAsync(actorDto);

                if (!validationResult.IsValid)
                {
                    return BadRequest(new { Errors = validationResult.Errors });
                }
                #endregion

                var actor = _mapper.Map<Actor>(actorDto);
                await _actorServices.InsertActorAsync(actor);

                var response = new ApiResponse<Actor>(actor);
                return Ok(response);
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
            }
        }

        /// <summary>
        /// Actualiza los datos de un actor existente por su id
        /// </summary>
        /// <remarks>
        /// Este metodo se utiliza actualizar un actor por envio de datos nuevos pero el id no cambia
        /// </remarks>
        /// <param name="actorDto">El actorDto que se manda para actualizar a un Actor existente registrado, 
        /// <param name="id">Identificador unico de el objeto a actualizar</param>
        /// <param name="idAux">Identificador de la tabla</param>>
        /// <returns>Un objeto actor actualizado</returns>
        /// <responsecode="200">El actor fue actualizado con los datos enviados</responsecode>
        /// <responsecode="404">No se encontro el dato</responsecode>
        /// <responsecode="500">Hubo un error al actualizar el dato</responsecode>
        /// <responsecode="404">Error por mal ingreso del dato</responsecode>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<Actor>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [Authorize(Roles = nameof(RoleType.Provider))]
        [HttpPut("dto/mapper/{id}")]
        public async Task<IActionResult> UpdateActorDtoMapper(int id,
            [FromBody] ActorDto actorDto,int idAux)
        {
            try
            {
                var validationResult = await _validationService.ValidateAsync(actorDto);    
                if(!validationResult.IsValid)
                {
                    return BadRequest(new { Errors = validationResult.Errors });
                }
                if (id != actorDto.Id)
                    return BadRequest("El Id del actor no coincide");

                var actor = await _actorServices.GetActorAsync(id);
                if (actor == null)
                    return NotFound("Actor no encontrado");

                _mapper.Map(actorDto, actor);
                await _actorServices.UpdateActorAsync(actor);

                var response = new ApiResponse<Actor>(actor);
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
        /// Este metodo se utiliza para eliminar a un objeto actor
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
        public async Task<IActionResult> DeleteActorDtoMapper(int id,int idAux)
        {
            var actor = await _actorServices.GetActorAsync(id);
            if (actor == null)
                return NotFound("Actor no encontrado");

            await _actorServices.DeleteActorAsync(actor);


            return NoContent();
        }


        /// <summary>
        /// Obtiene los actores mas jovenes registrados
        /// </summary>
        /// <remarks>
        /// Este metodo se utiliza para obtener un reporte acerca de los actores por edad
        /// </remarks>
        /// <param name="idAux">Identificador de la tabla</param>>
        /// <returns>Una coleccion de reportes de actores de los mas jovenes</returns>
        /// <responsecode="200">Coleccion de datos</responsecode>
        /// <responsecode="404">No se encontraron los dato</responsecode>
        /// <responsecode="500">Hubo un error al encontrar los dato</responsecode>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<Top10TheYoungestActors>>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [Authorize(Roles = $"{nameof(RoleType.Administrator)},{nameof(RoleType.Provider)}")]
        [HttpGet("dapper/1")]
        public async Task<IActionResult> GetTop10TheYoungestActors(int idAux)
        {
            var actors = await _actorServices.GetTop10TheYoungestActors();

            var totalCount = actors.Count();
            var pagination = new Pagination
            {
                TotalCount = totalCount,
                PageSize = 10,      
                CurrentPage = 1,
                TotalPages = 1,
                HasNextPage = false,
                HasPreviousPage = false
            };

            var response = new ApiResponse<IEnumerable<Top10TheYoungestActors>>(actors)
            {
                Pagination = pagination,
                Messages = actors.Any()
                    ? null
                    : new[] { new Message { Type = "Warning", Description = "No se encontraron actores" } }
            };
            return Ok(response);
        }

        #endregion
    }
}