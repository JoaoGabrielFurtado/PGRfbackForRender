using Microsoft.EntityFrameworkCore;
namespace pgr.Model;
public class Exercicios 
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public string GrupoMuscular { get; set; }

    public virtual ICollection<TreinoExercicios> TreinoExercicios { get; set; }
}
