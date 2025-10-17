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
    public class MovieController : ControllerBase
    {
        private readonly IMovieServices _movieServices;
        private readonly IMapper _mapper;
        private readonly IValidationService _validationService;
        public MovieController(IMovieServices movieServices,
            IMapper mapper,IValidationService validationService)
        {
            _movieServices = movieServices;
            _mapper = mapper;
            _validationService = validationService;
        }

        #region Sin DTOs
        [HttpGet]
        public async Task<IActionResult> GetMovie()
        {
            var movies = await _movieServices.GetAllMovieAsync();
            return Ok(movies);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovieId(int id)
        {
            var movie = await _movieServices.GetMovieAsync(id);
            return Ok(movie);
        }

        [HttpPost]
        public async Task<IActionResult> InsertMovie(Movie movie)
        {
            await _movieServices.InsertMovieAsync(movie);
            return Ok(movie);
        }
        #endregion

        #region Con DTO
        [HttpGet("dto")]
        public async Task<IActionResult> GetMoviesDto()
        {
            var movies = await _movieServices.GetAllMovieAsync();
            var moviesDto = movies.Select(m => new MovieDto
            {
                Id = m.Id,
                Title=m.Title,
                Description=m.Description,
                ReleaseDate=m.ReleaseDate.ToString("dd-mm-yyyy"),
                Genre=m.Genre,
                Length=m.Length
                ,
            });

            return Ok(moviesDto);
        }

        [HttpGet("dto/{id}")]
        public async Task<IActionResult> GetMovieIdDto(int id)
        {
            var movie = await _movieServices.GetMovieAsync(id);
            var movieDto = new MovieDto
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                ReleaseDate = movie.ReleaseDate.ToString("dd-mm-yyyy"),
                Genre = movie.Genre,
                Length = movie.Length
            };

            return Ok(movieDto);
        }

        [HttpPost("dto")]
        public async Task<IActionResult> InsertMovieDto(MovieDto movieDto)
        {
            var movie = new Movie
            {
                Id = movieDto.Id,
                Title = movieDto.Title,
                Description = movieDto.Description,
                ReleaseDate = Convert.ToDateTime(movieDto.ReleaseDate),
                Genre = movieDto.Genre,
                Length = movieDto.Length
            };

            await _movieServices.InsertMovieAsync(movie);
            return Ok(movie);
        }

        [HttpPut("dto/{id}")]
        public async Task<IActionResult> UpdateMovieDto(int id,
            [FromBody] MovieDto movieDto)
        {
            if (id != movieDto.Id)
                return BadRequest("El Id de la pelicula no coincide");

            var movie = await _movieServices.GetMovieAsync(id);
            if (movie == null)
                return NotFound("Pelicula no encontrada");

            movie.Id = movieDto.Id;
            movie.Title = movieDto.Title;
            movie.Description = movieDto.Description;
            movie.ReleaseDate = Convert.ToDateTime(movieDto.ReleaseDate);
            movie.Genre = movieDto.Genre;
            movie.Length = movieDto.Length;

            await _movieServices.UpdateMovieAsync(movie);
            return Ok(movie);
        }

        [HttpDelete("dto/{id}")]
        public async Task<IActionResult> UpdateMovieDto(int id)
        {
            var movie = await _movieServices.GetMovieAsync(id);
            if (movie == null)
                return NotFound("Pelicula no encontrado");

            await _movieServices.DeleteMovieAsync(movie);
            return NoContent();
        }
        #endregion

        #region Dto Mapper
        [HttpGet("dto/mapper")]
        public async Task<IActionResult> GetMoviesDtoMapper()
        {
            var movies = await _movieServices.GetAllMovieAsync();
            var moviesDto = _mapper.Map<IEnumerable<MovieDto>>(movies);

            return Ok(moviesDto);
        }


        [HttpGet("dto/mapper/{id}")]
        public async Task<IActionResult> GetMoviesDtoMapperId(int id)
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

            var movie = await _movieServices.GetMovieAsync(id);
            var movieDto = _mapper.Map<MovieDto>(movie);

            var response = new ApiResponse<MovieDto>(movieDto);

            return Ok(response);
        }

        [HttpPost("dto/mapper/")]
        public async Task<IActionResult> InsertMovieDtoMapper([FromBody] MovieDto movieDto)
        {
            try
            {
                #region Validaciones
                // La validación automática se hace mediante el filtro
                // Esta validación manual es opcional
                var validationResult = await _validationService.ValidateAsync(movieDto);

                if (!validationResult.IsValid)
                {
                    return BadRequest(new { Errors = validationResult.Errors });
                }
                #endregion

                var movie = _mapper.Map<Movie>(movieDto);
                await _movieServices.InsertMovieJustOne(movie);

                var response = new ApiResponse<Movie>(movie);
                return Ok(response);
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
            }
        }

        [HttpPut("dto/mapper/{id}")]
        public async Task<IActionResult> UpdateMovieDtoMapper(int id,
            [FromBody] MovieDto movieDto)
        {
            try
            {
                var validationResult = await _validationService.ValidateAsync(movieDto);
                if(!validationResult.IsValid)
                {
                    return BadRequest(new { Errors = validationResult.Errors });
                }
                if (id != movieDto.Id)
                    return BadRequest("El Id de la pelicula no coincide");

                var movie = await _movieServices.GetMovieAsync(id);
                if (movie == null)
                    return NotFound("Pelicula no encontrado");

                _mapper.Map(movieDto, movie);
                await _movieServices.UpdateMovieAsync(movie);

                var response = new ApiResponse<Movie>(movie);
                return Ok(response);
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
            }
        }

        [HttpDelete("dto/mapper/{id}")]
        public async Task<IActionResult> DeleteMovieDtoMapper(int id)
        {
            var movie = await _movieServices.GetMovieAsync(id);
            if (movie == null)
                return NotFound("Pelicula no encontrada");

            await _movieServices.DeleteMovieAsync(movie);


            return NoContent();
        }
        #endregion
    }
}