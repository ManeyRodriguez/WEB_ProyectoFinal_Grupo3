using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Threading;
using WEB_ProyectoFinal_Grupo3.Areas.Identity.Data;
using WEB_ProyectoFinal_Grupo3.Models;

namespace WEB_ProyectoFinal_Grupo3.Data;

public class DBContext : IdentityDbContext<Usuario>
{
    public DBContext(DbContextOptions<DBContext> options)
        : base(options)
    {
    }

    public DbSet<WEB_ProyectoFinal_Grupo3.Models.Paquete> Paquetes { get; set; } = default!;
    public DbSet<WEB_ProyectoFinal_Grupo3.Models.Categoria> Categorias { get; set; } = default!;
    public DbSet<WEB_ProyectoFinal_Grupo3.Models.Venta> Venta { get; set; } = default!;

  

    protected override void OnModelCreating(ModelBuilder builder)
    {


        base.OnModelCreating(builder);

  

        builder.Entity<Usuario>()
        .HasIndex(m => m.Cedula)    
        .IsUnique();

        builder.Entity<Usuario>()
         .HasIndex(u => u.PhoneNumber)
         .IsUnique();

        builder.Entity<Usuario>()
           .HasIndex(u => u.Email)
           .IsUnique();
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
