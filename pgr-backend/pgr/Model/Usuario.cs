using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace pgr.Model;
public class Usuario : IdentityUser
{
    public string Nome { get; set; }

    public virtual ICollection<Treinos> Treinos { get; set; }
}
