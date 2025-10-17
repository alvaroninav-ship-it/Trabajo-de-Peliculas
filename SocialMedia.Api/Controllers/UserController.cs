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

        #region Sin DTOs
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var users = await _userRepository.GetAllUserAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserId(int id)
        {
            var user = await _userRepository.GetUserAsync(id);
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> InsertUser(User user)
        {
            await _userRepository.InsertUserAsync(user);
            return Ok(user);
        }
        #endregion

        #region Con DTO
        [HttpGet("dto")]
        public async Task<IActionResult> GetUsersDto()
        {
            var users = await _userRepository.GetAllUserAsync();
            var usersDto = users.Select(u => new UserDto
            {
                Id = u.Id,
                FirstName=u.FirstName,
                LastName=u.LastName,
                DateOfBirth=u.DateOfBirth.ToString("dd-mm-yyyy"),
                Telephone=u.Telephone,
                Email=u.Email,
                IsActive=u.IsActive,
            });

            return Ok(usersDto);
        }

        [HttpGet("dto/{id}")]
        public async Task<IActionResult> GetUserIdDto(int id)
        {
            var user = await _userRepository.GetUserAsync(id);
            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth.ToString("dd-mm-yyyy"),
                Telephone = user.Telephone,
                Email = user.Email,
                IsActive = user.IsActive,
            };

            return Ok(userDto);
        }

        [HttpPost("dto")]
        public async Task<IActionResult> InsertUserDto(UserDto userDto)
        {
            var user = new User
            {
                Id = userDto.Id,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                DateOfBirth = Convert.ToDateTime(userDto.DateOfBirth),
                Telephone = userDto.Telephone,
                Email = userDto.Email,
                IsActive = userDto.IsActive,
            };

            await _userRepository.InsertUserAsync(user);
            return Ok(user);
        }

        [HttpPut("dto/{id}")]
        public async Task<IActionResult> UpdateUserDto(int id,
            [FromBody] UserDto userDto)
        {
            if (id != userDto.Id)
                return BadRequest("El Id del Usuario no coincide");

            var user = await _userRepository.GetUserAsync(id);
            if (user == null)
                return NotFound("Usuario no encontrado");

            user.Id = userDto.Id;
            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.DateOfBirth = Convert.ToDateTime(userDto.DateOfBirth);
            user.Telephone = userDto.Telephone;
            user.Email = userDto.Email;
            user.IsActive = userDto.IsActive;

            await _userRepository.UpdateUserAsync(user);
            return Ok(user);
        }

        [HttpDelete("dto/{id}")]
        public async Task<IActionResult> UpdateUserDto(int id)
        {
            var user = await _userRepository.GetUserAsync(id);
            if (user == null)
                return NotFound("User no encontrado");

            await _userRepository.DeleteUserAsync(user);
            return NoContent();
        }
        #endregion

        #region Dto Mapper
        [HttpGet("dto/mapper")]
        public async Task<IActionResult> GetUsersDtoMapper()
        {
            var users = await _userRepository.GetAllUserAsync();
            var usersDto = _mapper.Map<IEnumerable<UserDto>>(users);

            return Ok(usersDto);
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

        #endregion
    }
}