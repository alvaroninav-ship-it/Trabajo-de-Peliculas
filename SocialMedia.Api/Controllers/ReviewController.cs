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

        #region Sin DTOs
        [HttpGet]
        public async Task<IActionResult> GetReview()
        {
            var reviews = await _reviewServices.GetAllReviewAsync();
            return Ok(reviews);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReviewId(int id)
        {
            var review = await _reviewServices.GetReviewAsync(id);
            return Ok(review);
        }

        [HttpPost]
        public async Task<IActionResult> InsertReview(Review review)
        {
            await _reviewServices.InsertReviewAsync(review);
            return Ok(review);
        }
        #endregion

        #region Con DTO
        [HttpGet("dto")]
        public async Task<IActionResult> GetReviewsDto()
        {
            var reviews = await _reviewServices.GetAllReviewAsync();
            var reviewsDto = reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                UserId = r.UserId,
                MovieId=r.MovieId,
                Date = r.Date.ToString("dd-mm-yyyy"),
                Description = r.Description,
                Grade=r.Grade,
            });

            return Ok(reviewsDto);
        }

        [HttpGet("dto/{id}")]
        public async Task<IActionResult> GetReviewIdDto(int id)
        {
            var review = await _reviewServices.GetReviewAsync(id);
            var reviewDto = new ReviewDto
            {
                Id = review.Id,
                UserId = review.UserId,
                MovieId=review.MovieId,
                Date = review.Date.ToString("dd-mm-yyyy"),
                Description = review.Description,
                Grade = review.Grade,
            };

            return Ok(reviewDto);
        }

        [HttpPost("dto")]
        public async Task<IActionResult> InsertReviewDto(ReviewDto reviewDto)
        {
            var review = new Review
            {
                Id = reviewDto.Id,
                UserId = reviewDto.UserId,
                MovieId=reviewDto.MovieId,
                Date = Convert.ToDateTime(reviewDto.Date),
                Description = reviewDto.Description,
                Grade = reviewDto.Grade,
            };

            await _reviewServices.InsertReviewAsync(review);
            return Ok(review);
        }

        [HttpPut("dto/{id}")]
        public async Task<IActionResult> UpdateReviewDto(int id,
            [FromBody] ReviewDto reviewDto)
        {
            if (id != reviewDto.Id)
                return BadRequest("El Id de la critica no coincide");

            var review = await _reviewServices.GetReviewAsync(id);
            if (review == null)
                return NotFound("Critica no encontrado");

            review.Id = reviewDto.Id;
            review.UserId = reviewDto.UserId;
            review.MovieId = reviewDto.MovieId;
            review.Date = Convert.ToDateTime(reviewDto.Date);
            review.Description = reviewDto.Description;
            review.Grade = reviewDto.Grade;

            await _reviewServices.UpdateReviewAsync(review);
            return Ok(review);
        }

        [HttpDelete("dto/{id}")]
        public async Task<IActionResult> UpdateReviewDto(int id)
        {
            var review = await _reviewServices.GetReviewAsync(id);
            if (review == null)
                return NotFound("Critica no encontrado");

            await _reviewServices.DeleteReviewAsync(review);
            return NoContent();
        }
        #endregion

        #region Dto Mapper
        [HttpGet("dto/mapper")]
        public async Task<IActionResult> GetReviewsDtoMapper()
        {
            var reviews = await _reviewServices.GetAllReviewAsync();
            var reviewsDto = _mapper.Map<IEnumerable<ReviewDto>>(reviews);

            return Ok(reviewsDto);
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
        #endregion
    }
}
