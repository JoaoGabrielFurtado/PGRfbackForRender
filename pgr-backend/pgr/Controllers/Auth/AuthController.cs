using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using pgr.Dto.Auth;
using pgr.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace pgr.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<Usuario> _userManager;
    private readonly SignInManager<Usuario> _signInManager;
    private readonly IConfiguration _configuration;

    public AuthController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Registro([FromBody] RegisterDto registroDto)
    {
        if (registroDto is null)
            return BadRequest();

        var emailUser = await _userManager.FindByEmailAsync(registroDto.Email);

        if (emailUser is not null)
            return BadRequest("E-mail já cadastrado");

        var novoUsuario = new Usuario { Nome = registroDto.Nome, Email = registroDto.Email, UserName = registroDto.Email };

        var retorno = await _userManager.CreateAsync(novoUsuario, registroDto.Senha);

        if (!(retorno.Succeeded))
            return BadRequest(retorno.Errors);

        return Ok(new { message = "Usuário criado com sucesso!" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        if (loginDto is null)
            return BadRequest("Requisição inválida.");

        var usuario = await _userManager.FindByEmailAsync(loginDto.Email);

        if (usuario is null || !await _userManager.CheckPasswordAsync(usuario, loginDto.Senha))
        {
            return Unauthorized("Email ou senha inválidos.");
        }

        var tokenString = GenerateJwtToken(usuario);
        return Ok(new {token = tokenString });
    }

    private string GenerateJwtToken(Usuario user)
    {
        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id), 
        new Claim(ClaimTypes.Email, user.Email),       
        new Claim(ClaimTypes.GivenName, user.Nome)
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
