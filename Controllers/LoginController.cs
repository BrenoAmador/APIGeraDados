using APITesteDev.Controllers.Classes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace APITesteDev.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Login([FromBody] Login request)
        {
            var usuario = Usuario.Usuarios.FirstOrDefault(u => u.Email == request.Email);

            if (usuario == null || !usuario.ValidarSenha(request.Senha))
                return Unauthorized("Usuário ou senha inválidos.");

            var keyString = Environment.GetEnvironmentVariable("JWT_KEY");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                message = $"Bem-vindo, {usuario.Nome}!",
                usuario = usuario.Nome.ToString(),
                token = tokenString
            });
        }
        [Authorize]
        [HttpPost("validar")]
        public IActionResult ValidarToken()
        {
            var nome = User.Identity?.Name;
            return Ok(new { valid = true, usuario = nome });
        }
    }
}
