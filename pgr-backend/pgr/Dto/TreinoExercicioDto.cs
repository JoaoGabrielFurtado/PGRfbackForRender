namespace pgr.Dto;

public class TreinoExercicioDto
{
    public int Id { get; set; }
    public int Series { get; set; }
    public int Repeticoes { get; set; }
    public ExercicioDto Exercicio { get; set; } 
}
