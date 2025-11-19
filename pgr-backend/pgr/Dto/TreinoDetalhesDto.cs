namespace pgr.Dto;

public class TreinoDetailsDto
{
    public int Id { get; set; }
    public string NomeDoTreino { get; set; }
    public string Descricao { get; set; }
    public List<TreinoExercicioDto> TreinoExercicios { get; set; } = new List<TreinoExercicioDto>();
}
