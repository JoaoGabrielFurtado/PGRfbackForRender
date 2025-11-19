using Microsoft.EntityFrameworkCore;
namespace pgr.Model;
public class RegistroDeTreinos
{
    public int Id { get; set; }
    public int TreinoExercicioId { get; set; }
    public DateTime Data { get; set; }
    public string Carga { get; set; }
    public string Observacoes { get; set; }

    public virtual TreinoExercicios TreinoExercicio { get; set; }

}
