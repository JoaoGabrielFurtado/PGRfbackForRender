using Microsoft.EntityFrameworkCore;
namespace pgr.Model;
public class Treinos 
{
    public int Id { get; set; }
    public string UsuarioId { get; set; }
    public string NomeDoTreino { get; set; }
    public string Descricao { get; set; }
    public DateTime DataCriacao { get; set; }

    public virtual Usuario Usuario { get; set; }

    public virtual ICollection<TreinoExercicios> TreinoExercicios { get; set; }


}
