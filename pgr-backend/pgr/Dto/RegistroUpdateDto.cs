using System.ComponentModel.DataAnnotations;

namespace pgr.Dto;
public class RegistroUpdateDto
{
    [Required]
    [Range(0, 1000)]
    public int[] Carga { get; set; }

    [MaxLength(500)]
    public string? Observacoes { get; set; }
}
