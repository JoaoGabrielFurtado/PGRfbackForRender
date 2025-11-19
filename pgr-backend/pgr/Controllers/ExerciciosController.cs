using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pgr.Banco; 
using pgr.Model;

namespace pgr.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ExerciciosController : ControllerBase
{

    private readonly PgrDbCon _contexto;

    public ExerciciosController(PgrDbCon contexto)
    {
        _contexto = contexto;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Exercicios>>> GetExercicios()
    {
        var exercicios = await _contexto.Exercicios.ToListAsync();
        return Ok(exercicios);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Exercicios>> GetExercicioId(int id)
    {
        var exercicio = await _contexto.Exercicios.FirstOrDefaultAsync(i => i.Id.Equals(id));

        if (exercicio is null)
            return NotFound();

        return Ok(exercicio);
    }

    [HttpPost]
    public async Task<ActionResult<IEnumerable<Exercicios>>> PostExercicio(Exercicios exercicio)
    {
        if (exercicio is null)
            return BadRequest();


        await _contexto.Exercicios.AddAsync(exercicio);
        await _contexto.SaveChangesAsync();
        return CreatedAtAction(nameof(GetExercicioId), new { id = exercicio.Id }, exercicio);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<IEnumerable<Exercicios>>> PutExercicio(int id, Exercicios exercicio)
    {
        var exercicioAntigo = await _contexto.Exercicios.FindAsync(id);

        if (exercicio is null || exercicioAntigo is null)
            return NotFound();

        exercicioAntigo.Nome = exercicio.Nome;
        exercicioAntigo.Descricao = exercicio.Descricao;
        exercicioAntigo.GrupoMuscular = exercicio.GrupoMuscular;

        await _contexto.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<IEnumerable<Exercicios>>> DeleteExercicio(int id)
    {
        var exercicio = await _contexto.Exercicios.FindAsync(id);

        if (exercicio is null)
        {
            return NotFound();
        }

        _contexto.Remove(exercicio);
        await _contexto.SaveChangesAsync();
        return NoContent();
    }

}