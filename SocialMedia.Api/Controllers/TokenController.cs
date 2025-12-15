using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Movies.Core.Entities;
using Movies.Core.Interfaces;

namespace SocialMedia.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ISecurityService _securityService;
        private readonly IPasswordService _passwordService;


        public TokenController(IConfiguration configuration, ISecurityService securityService, IPasswordService passwordService)
        {
            _configuration = configuration;
            _securityService = securityService;
            _passwordService = passwordService;

        }

        [HttpGet("TestConeccion")]
        public async Task<IActionResult> TestConeccion()
        {
            try
            {
                var result = new
                {
                    ConnectionMySql = _configuration["ConnectionStrings:ConnectionMySql"],
                    ConnectionSqlServer = _configuration["ConnectionStrings:ConnectionSqlServer"]
                };

                return Ok(result);
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
            }
        }


        [HttpGet("Config")]
        public async Task<IActionResult> GetConfig()
        {
            try
            {
                var connectionStringMySql = _configuration["ConnectionStrings:ConnectionMySql"];
                var connectionStringSqlServer = _configuration["ConnectionStrings:ConnectionSqlServer"];

                var result = new
                {
                    connectionStringMySql = connectionStringMySql ?? "My SQL NO CONFIGURADO",
                    connectionStringSqlServer = connectionStringSqlServer ?? "SQL SERVER NO CONFIGURADO",
                    AllConnectionStrings = _configuration.GetSection("ConnectionStrings").GetChildren().Select(x => new { Key = x.Key, Value = x.Value }),
                    Environment = _configuration["ASPNETCORE_ENVIRONMENT"] ?? "ASPNETCORE_ENVIRONMENT NO CONFIGURADO",
                    Authentication = _configuration.GetSection("Authentication").GetChildren().Select(x => new { Key = x.Key, Value = x.Value })
                };

                return Ok(result);
            }
            catch (Exception err)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, err.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Authentication(UserLogin userLogin)
        {
            try
            {
                //Si es un usuario válido
                var validation = await IsValidUser(userLogin);

                if (validation.Item1)
                {
                    var token = GenerateToken(validation.Item2);
                    return Ok(new { token });
                }
                else
                {
                    throw new Exception("Usuario o contraseña incorrecta");
                }
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private async Task<(bool, Security)> IsValidUser(UserLogin login)
        {
            var user = await _securityService.GetLoginByCredentials(login);
            var isValid = _passwordService.Check(user.Password, login.Password);
            return (isValid, user);
        }

        private string GenerateToken(Security security)
        {
            //Header
            var symmetricSecurityKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"]));
            var signingCredentials =
                new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var header = new JwtHeader(signingCredentials);

            //Claims (Cuerpo)
            var claims = new[]
            {
            new Claim("Login", security.Login),
            new Claim("Name", security.Name),
            new Claim(ClaimTypes.Role, security.Role.ToString()),
            };

            //Payload
            var payload = new JwtPayload(
                issuer: _configuration["Authentication:Issuer"],
                audience: _configuration["Authentication:Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(2)
            );

            //Generar el token JWT
            var token = new JwtSecurityToken(header, payload);

            //Serializar el token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        

    }


}
