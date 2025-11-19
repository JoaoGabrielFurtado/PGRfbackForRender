using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pgr.Banco;
using pgr.Dto;
using pgr.Model;

namespace pgr.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{

    private readonly PgrDbCon _contexto;

    public UsuarioController(PgrDbCon contexto)
    {
        _contexto = contexto;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Usuario>> GetUsuarioId(int id)
    {
        var usuario = await _contexto.Users.FindAsync(id);

        if (usuario is null)
            return NotFound();

        UsuarioDto usuarioDto = new();

        usuarioDto.Id = usuario.Id;
        usuarioDto.Nome = usuario.Nome;
        usuarioDto.Email = usuario.Email;

        return Ok(usuarioDto);
    }

    [HttpPost]
    public async Task<ActionResult<UsuarioDto>> PostUsuario([FromBody]UsuarioCreateDto usuarioDto)
    {
        if (usuarioDto is null)
            return BadRequest();

        Usuario usuario = new()
        {
            Nome = usuarioDto.Nome,
            Email = usuarioDto.Email,
            PasswordHash = "hashed_" + usuarioDto.Senha
        };

        await _contexto.Users.AddAsync(usuario);
        await _contexto.SaveChangesAsync();

        UsuarioDto usuarioReturn = new()
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email
        };

        return CreatedAtAction(nameof(GetUsuarioId), new { id = usuario.Id }, usuarioReturn);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UsuarioDto>> PutUsuario(int id, [FromBody] UsuarioCreateDto usuarioAtualizado)
    {
        if (usuarioAtualizado is null)
            return BadRequest();

        Usuario? usuarioAntigo = await _contexto.Users.FindAsync(id);

        if (usuarioAntigo is null)
            return NotFound("Usuário não encontrado.");

        usuarioAntigo.Nome = usuarioAtualizado.Nome;
        usuarioAntigo.Email = usuarioAtualizado.Email;
        usuarioAntigo.PasswordHash = "hashed_" + usuarioAtualizado.Senha;
        await _contexto.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<UsuarioDto>> DeleteUsuario(int id)
    {
        var usuarioParaDeletar = await _contexto.Users.FindAsync(id);

        if (usuarioParaDeletar is null)
            return NotFound("Usuário não encontrado.");

        _contexto.Users.Remove(usuarioParaDeletar);
        await _contexto.SaveChangesAsync();
        return NoContent();
    }

}
