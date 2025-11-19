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
[Route("api/[controller]")]
public class TreinosController : ControllerBase
{
    private readonly PgrDbCon _contexto;
    public TreinosController(PgrDbCon contexto)
    {
        _contexto = contexto;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Treinos>>> GetTreinos()
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(usuarioId))
        {
            return Unauthorized();
        }

        var treinos = await _contexto.Treinos.Where(id => id.UsuarioId == usuarioId).ToListAsync();
        return Ok(treinos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TreinoDetailsDto>> GetTreinosId(int id)
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var treino = await _contexto.Treinos.Include(t => t.TreinoExercicios).ThenInclude(te => te.Exercicio).FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

        if (treino == null)
        {
            return NotFound();
        }

        var treinoDto = new TreinoDetailsDto
        {
            Id = treino.Id,
            NomeDoTreino = treino.NomeDoTreino,
            Descricao = treino.Descricao,
            TreinoExercicios = treino.TreinoExercicios.Select(te => new TreinoExercicioDto
            {
                Id = te.Id,
                Series = te.Series,
                Repeticoes = te.Repeticoes,
                Exercicio = new ExercicioDto
                {
                    Id = te.Exercicio.Id,
                    Nome = te.Exercicio.Nome,
                    GrupoMuscular = te.Exercicio.GrupoMuscular
                }
            }).ToList()
        };

        return Ok(treinoDto);
    }

    [HttpPost]
    public async Task<ActionResult<Treinos>> PostTreino(NovoTreinoDto treinoDto)
    {
        if (treinoDto is null)
            return BadRequest("Preencha o treino");

        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(usuarioId))
        {
            return Unauthorized();
        }

        Treinos treino = new Treinos
        {
            UsuarioId = usuarioId,
            NomeDoTreino = treinoDto.NomeTreino,
            Descricao = treinoDto.Descricao,
            DataCriacao = DateTime.UtcNow
        };

        await _contexto.AddAsync(treino);
        await _contexto.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTreinosId), new { id = treino.Id }, treino);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Treinos>> PutTreinosId(int id, NovoTreinoDto treinoAtualizadoDto)
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(usuarioId))
        {
            return Unauthorized();
        }

        var treino = await _contexto.Treinos.FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

        if (treino == null)
        {
            return NotFound();
        }

        treino.NomeDoTreino = treinoAtualizadoDto.NomeTreino;
        treino.Descricao = treinoAtualizadoDto.Descricao;
        await _contexto.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Treinos>> DeleteTreinosId(int id)
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(usuarioId))
        {
            return Unauthorized();
        }

        var treino = await _contexto.Treinos.FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

        if (treino == null)
        {
            return NotFound();
        }

        _contexto.Remove(treino);
        await _contexto.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{treinoId}/exercicios")]
    public async Task<ActionResult<TreinoExercicios>> PostTreinoExercicio(int treinoId, AddExercicioDto exDto){

        if (exDto is null)
            return BadRequest("Erro 1");

        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(usuarioId))
            return Unauthorized("Erro 2");

        var treino = await _contexto.Treinos.FirstOrDefaultAsync(t => t.Id == treinoId && t.UsuarioId == usuarioId);

        if (treino is null)
            return Forbid("Erro 3");

        var exercicio = await _contexto.Exercicios.FirstOrDefaultAsync(e => e.Id == exDto.ExercicioId);

        if (exercicio is null)
            return BadRequest("Exercício não encontrado.");

        var jaExiste = await _contexto.TreinoExercicios.AnyAsync(te => te.TreinoId == treinoId && te.ExercicioId == exDto.ExercicioId);

        if (jaExiste)
        {
            return BadRequest("Este exercício já foi adicionado a este treino.");
        }

        TreinoExercicios teex = new TreinoExercicios
        {
            TreinoId = treinoId,
            ExercicioId = exercicio.Id,
            Series = exDto.Series,
            Repeticoes = exDto.Repeticoes,
        };

        var resultado = new
        {
            id = teex.Id,
            treinoId = teex.TreinoId,
            exercicioId = teex.ExercicioId,
            series = teex.Series,
            repeticoes = teex.Repeticoes
        };

        await _contexto.TreinoExercicios.AddAsync(teex);
        await _contexto.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTreinosId), new { id = treinoId }, resultado);
    }

    [HttpDelete("{treinoId}/exercicios/{exercicioId}")]
    public async Task<IActionResult> RemoveExercicioTreino(int treinoId, int exercicioId)
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var treinoExercicioParaDeletar = await _contexto.TreinoExercicios.FirstOrDefaultAsync(te =>
                te.TreinoId == treinoId &&
                te.ExercicioId == exercicioId &&
                te.Treino.UsuarioId == usuarioId);

        if (treinoExercicioParaDeletar == null)
        {
            return NotFound();
        }

        _contexto.TreinoExercicios.Remove(treinoExercicioParaDeletar);
        await _contexto.SaveChangesAsync();

        return NoContent();
    }

}
