using Microsoft.EntityFrameworkCore;
namespace pgr.Model;
public class TreinoExercicios 
{
    public int Id { get; set; }
    public int TreinoId { get; set; }
    public int ExercicioId { get; set; }
    public int Series { get; set; }
    public int Repeticoes { get; set; }

    public virtual Treinos Treino { get; set; }

    public virtual Exercicios Exercicio { get; set; }

    public virtual ICollection<RegistroDeTreinos> RegistroDeTreinos { get; set; }
}
