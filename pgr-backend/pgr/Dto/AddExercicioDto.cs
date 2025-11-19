using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace pgr.Dto;
public class AddExercicioDto
{
    [Required(ErrorMessage = "O Id do exercício é obrigatório.")]
    public int ExercicioId { get; set; }

    [Required(ErrorMessage = "O número de séries é obrigatório.")]
    [Range(1, 100, ErrorMessage = "O número de séries deve ser entre 1 e 100.")]
    public int Series { get; set; }

    [Required(ErrorMessage = "O número de repetições é obrigatório.")]
    [Range(1, 100, ErrorMessage = "O número de repetições deve ser entre 1 e 100.")]
    public int Repeticoes { get; set; }
}
