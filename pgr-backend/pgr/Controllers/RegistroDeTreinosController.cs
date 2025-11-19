using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pgr.Banco;
using pgr.Dto;
using pgr.Model;
using System.Security.Claims;

namespace pgr.Controllers;

[Authorize]
[ApiController]
[Route("api/registrosdetreino")]
public class RegistroDeTreinosController : ControllerBase
{
    private readonly PgrDbCon _contexto;
    public RegistroDeTreinosController(PgrDbCon contexto)
    {
        _contexto = contexto;
    }

    [HttpGet]
    public async Task<IActionResult> GetRegistros([FromQuery] int treinoExercicioId)
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(usuarioId)) return Unauthorized();

        var treinoExercicio = await _contexto.TreinoExercicios
            .Include(te => te.Treino)
            .FirstOrDefaultAsync(te => te.Id == treinoExercicioId && te.Treino.UsuarioId == usuarioId);

        if (treinoExercicio is null)
        {
            return Ok(new List<RegistroDto>());
        }

        var registrosDto = await _contexto.RegistroDeTreinos
            .Include(r => r.TreinoExercicio.Exercicio)
            .Where(r => r.TreinoExercicioId == treinoExercicioId)
            .OrderByDescending(r => r.Data)
            .Select(r => new RegistroDto
            {
                Id = r.Id,
                TreinoExercicioId = r.TreinoExercicioId,
                NomeExercicio = r.TreinoExercicio.Exercicio.Nome,
                GrupoMuscular = r.TreinoExercicio.Exercicio.GrupoMuscular,
                Data = r.Data,
                Carga = r.Carga,
                Observacoes = r.Observacoes
            })
            .ToListAsync();

        return Ok(registrosDto);
    }

    [HttpPost]
    public async Task<IActionResult> CriarRegistro([FromBody] RegistroCreateDto dto)
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(usuarioId)) return Unauthorized();

        var treinoExercicio = await _contexto.TreinoExercicios
            .Include(te => te.Treino)
            .FirstOrDefaultAsync(te => te.Id == dto.TreinoExercicioId && te.Treino.UsuarioId == usuarioId);

        if (treinoExercicio is null)
        {
            return Forbid();
        }

        RegistroDeTreinos registro = new RegistroDeTreinos
        {
            TreinoExercicioId = dto.TreinoExercicioId,
            Carga = string.Join(",", dto.Carga),
            Observacoes = dto.Observacoes,
            Data = DateTime.UtcNow
        };

        await _contexto.RegistroDeTreinos.AddAsync(registro);
        await _contexto.SaveChangesAsync();

        return Ok(new { message = "Performance registada com sucesso!" });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRegistro(int id, [FromBody] RegistroUpdateDto dto)
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(usuarioId)) return Unauthorized();

        var registroParaAtualizar = await _contexto.RegistroDeTreinos
            .Include(r => r.TreinoExercicio)
                .ThenInclude(te => te.Treino)
            .FirstOrDefaultAsync(r => r.Id == id && r.TreinoExercicio.Treino.UsuarioId == usuarioId);

        if (registroParaAtualizar is null)
            return NotFound();

        registroParaAtualizar.Carga = string.Join(",", dto.Carga);
        registroParaAtualizar.Observacoes = dto.Observacoes;

        await _contexto.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRegistro(int id)
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(usuarioId)) return Unauthorized();

        var registroParaDeletar = await _contexto.RegistroDeTreinos
        .Include(r => r.TreinoExercicio)
        .ThenInclude(te => te.Treino)
        .FirstOrDefaultAsync(r => r.Id == id && r.TreinoExercicio.Treino.UsuarioId == usuarioId);

        if (registroParaDeletar is null)
            return NotFound();

        _contexto.RegistroDeTreinos.Remove(registroParaDeletar);
        await _contexto.SaveChangesAsync();
        return NoContent();
    }

}
