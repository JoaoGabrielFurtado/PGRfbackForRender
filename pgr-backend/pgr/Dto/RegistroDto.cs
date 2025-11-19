namespace pgr.Dto;
public class RegistroDto
{
    public int Id { get; set; }

    public int TreinoExercicioId { get; set; }

    public string NomeExercicio { get; set; }
    public string GrupoMuscular { get; set; }

    public DateTime Data { get; set; }

    public string Carga { get; set; }

    public string? Observacoes { get; set; }
}
