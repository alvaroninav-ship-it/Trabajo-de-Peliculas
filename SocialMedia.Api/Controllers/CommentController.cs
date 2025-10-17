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

        #region Sin DTOs
        [HttpGet]
        public async Task<IActionResult> GetComment()
        {
            var comments = await _commentServices.GetAllCommentAsync();
            return Ok(comments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCommentId(int id)
        {
            var comment = await _commentServices.GetCommentAsync(id);
            return Ok(comment);
        }

        [HttpPost]
        public async Task<IActionResult> InsertComment(Comment comment)
        {
            await _commentServices.InsertCommentAsync(comment);
            return Ok(comment);
        }
        #endregion

        #region Con DTO
        [HttpGet("dto")]
        public async Task<IActionResult> GetCommentDto()
        {
            var comments = await _commentServices.GetAllCommentAsync();
            var commentsDto = comments.Select(c => new CommentDto
            {
                Id = c.Id,
                ReviewId = c.ReviewId,
                UserId = c.UserId,
                Description = c.Description,
                Date = c.Date.ToString("dd-mm-yyyy"),
                IsActive = c.IsActive,
            });

            return Ok(commentsDto);
        }

        [HttpGet("dto/{id}")]
        public async Task<IActionResult> GetCommentIdDto(int id)
        {
            var comment = await _commentServices.GetCommentAsync(id);
            var commentDto = new CommentDto
            {
                Id = comment.Id,
                ReviewId = comment.ReviewId,
                UserId = comment.UserId,
                Description = comment.Description,
                Date = comment.Date.ToString("dd-mm-yyyy"),
                IsActive = comment.IsActive,
            };

            return Ok(commentDto);
        }

        [HttpPost("dto")]
        public async Task<IActionResult> InsertCommentDto(CommentDto commentDto)
        {
            var comment = new Comment
            {
                Id = commentDto.Id,
                ReviewId = commentDto.ReviewId,
                UserId = commentDto.UserId,
                Description = commentDto.Description,
                Date = Convert.ToDateTime(commentDto.Date),
                IsActive = commentDto.IsActive,
            };

            await _commentServices.InsertCommentAsync(comment);
            return Ok(comment);
        }

        [HttpPut("dto/{id}")]
        public async Task<IActionResult> UpdateCommentDto(int id,
            [FromBody] CommentDto commentDto)
        {
            if (id != commentDto.Id)
                return BadRequest("El Id del comentario no coincide");

            var comment = await _commentServices.GetCommentAsync(id);
            if (comment == null)
                return NotFound("comentario no encontrado");

            comment.Id = commentDto.Id;
            comment.ReviewId = commentDto.ReviewId;
            comment.UserId = commentDto.UserId;
            comment.Description = commentDto.Description;
            comment.Date = Convert.ToDateTime(commentDto.Date);
            comment.IsActive = commentDto.IsActive;
           

            await _commentServices.UpdateCommentAsync(comment);
            return Ok(comment);
        }

        [HttpDelete("dto/{id}")]
        public async Task<IActionResult> UpdateCommentDto(int id)
        {
            var comment = await _commentServices.GetCommentAsync(id);
            if (comment == null)
                return NotFound("Comentario no encontrado");

            await _commentServices.DeleteCommentAsync(comment);
            return NoContent();
        }
        #endregion

        #region Dto Mapper
        [HttpGet("dto/mapper")]
        public async Task<IActionResult> GetCommentsDtoMapper()
        {
            var comments = await _commentServices.GetAllCommentAsync();
            var commentsDto = _mapper.Map<IEnumerable<CommentDto>>(comments);

            return Ok(commentsDto);
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
