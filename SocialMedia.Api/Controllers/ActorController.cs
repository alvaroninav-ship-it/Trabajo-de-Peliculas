using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Response;
using Movies.Core.CustomEntities;
using Movies.Core.Entities;
using Movies.Core.Interfaces;
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

        #region Sin DTOs
        [HttpGet]
        public async Task<IActionResult> GetActor()
        {
            var actors = await _actorServices.GetAllActorAsync();
            return Ok(actors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetActorId(int id)
        {
            var actor = await _actorServices.GetActorAsync(id);
            return Ok(actor);
        }

        [HttpPost]
        public async Task<IActionResult> InsertActor(Actor actor)
        {
            await _actorServices.InsertActorAsync(actor);
            return Ok(actor);
        }
        #endregion

        #region Con DTO
        [HttpGet("dto")]
        public async Task<IActionResult> GetActorsDto()
        {
            var actors = await _actorServices.GetAllActorAsync();
            var actorsDto = actors.Select(a => new ActorDto
            {
                Id = a.Id,
                MovieId =a.MovieId,
                FirstName=a.FirstName,
                LastName=a.LastName,
                Email=a.Email,
                DateOfBirth=a.DateOfBirth.ToString("dd-MM-yyyy"),
                IsActive=a.IsActive,
            });

            return Ok(actorsDto);
        }

        [HttpGet("dto/{id}")]
        public async Task<IActionResult> GetActorIdDto(int id)
        {
            var actor = await _actorServices.GetActorAsync(id);
            var actorDto = new ActorDto
            {
                Id = actor.Id,
                MovieId = actor.MovieId,
                FirstName=actor.FirstName,
                LastName = actor.LastName,
                Email = actor.Email,
                DateOfBirth = actor.DateOfBirth.ToString("dd-MM-yyyy"),
                IsActive = actor.IsActive,
            };

            return Ok(actorDto);
        }

        [HttpPost("dto")]
        public async Task<IActionResult> InsertActorDto(ActorDto actorDto)
        {
            var actor = new Actor
            {
                Id = actorDto.Id,
                MovieId = actorDto.MovieId,
                FirstName = actorDto.FirstName,
                LastName = actorDto.LastName,
                Email = actorDto.Email,
                DateOfBirth = Convert.ToDateTime(actorDto.DateOfBirth),
                IsActive = actorDto.IsActive,
            };

            await _actorServices.InsertActorAsync(actor);
            return Ok(actor);
        }

        [HttpPut("dto/{id}")]
        public async Task<IActionResult> UpdateActorDto(int id,
            [FromBody] ActorDto actorDto)
        {
            if (id != actorDto.Id)
                return BadRequest("El Id del actor no coincide");

            var actor = await _actorServices.GetActorAsync(id);
            if (actor == null)
                return NotFound("Post no encontrado");

            actor.Id = actorDto.Id;
            actor.MovieId = actorDto.MovieId;
            actor.FirstName = actorDto.FirstName;
            actor.LastName = actorDto.LastName;
            actor.Email = actorDto.Email;
            actor.DateOfBirth = Convert.ToDateTime(actorDto.DateOfBirth);
            actor.IsActive = actorDto.IsActive;
           

            await _actorServices.UpdateActorAsync(actor);
            return Ok(actor);
        }

        [HttpDelete("dto/{id}")]
        public async Task<IActionResult> UpdateActorDto(int id)
        {
            var actor = await _actorServices.GetActorAsync(id);
            if (actor == null)
                return NotFound("Post no encontrado");

            await _actorServices.DeleteActorAsync(actor);
            return NoContent();
        }
        #endregion

        #region Dto Mapper
        [HttpGet("dto/mapper")]
        public async Task<IActionResult> GetActorsDtoMapper()
        {
            var actors = await _actorServices.GetAllActorAsync();
            var actorsDto = _mapper.Map<IEnumerable<ActorDto>>(actors);

            return Ok(actorsDto);
        }


        [HttpGet("dto/mapper/{id}")]
        public async Task<IActionResult> GetActorsDtoMapperId(int id)
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

        [HttpPost("dto/mapper/")]
        public async Task<IActionResult> InsertActorDtoMapper([FromBody] ActorDto actorDto)
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
                await _actorServices.InsertActorByMovieExist(actor);

                var response = new ApiResponse<Actor>(actor);
                return Ok(response);
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
            }
        }

        [HttpPut("dto/mapper/{id}")]
        public async Task<IActionResult> UpdateActorDtoMapper(int id,
            [FromBody] ActorDto actorDto)
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

        [HttpDelete("dto/mapper/{id}")]
        public async Task<IActionResult> DeleteActorDtoMapper(int id)
        {
            var actor = await _actorServices.GetActorAsync(id);
            if (actor == null)
                return NotFound("Actor no encontrado");

            await _actorServices.DeleteActorAsync(actor);


            return NoContent();
        }
        #endregion
    }
}