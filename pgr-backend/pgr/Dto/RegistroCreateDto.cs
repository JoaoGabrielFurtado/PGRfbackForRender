using System.ComponentModel.DataAnnotations;

namespace pgr.Dto;
public class RegistroCreateDto
{
    [Required(ErrorMessage = "É obrigatório informar a qual exercício este registro pertence.")]
    [Range(1, int.MaxValue, ErrorMessage = "O Id do Treino-Exercício é inválido.")]
    public int TreinoExercicioId { get; set; }

    [Required(ErrorMessage = "É obrigatório informar a carga utilizada.")]
    public int[] Carga { get; set; }

    [MaxLength(500, ErrorMessage = "As observações não podem exceder 500 caracteres.")]
    public string? Observacoes { get; set; }
}
