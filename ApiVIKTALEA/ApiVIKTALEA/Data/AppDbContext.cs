using ApiVIKTALEA.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiVIKTALEA.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Orden> Ordenes { get; set; }
    public DbSet<DetalleOrden> DetallesOrden { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Usuarios");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
            entity.Property(e => e.PasswordHash).HasMaxLength(256).IsRequired();
            entity.Property(e => e.Rol).HasMaxLength(20).IsRequired().HasDefaultValue("Operador");
            entity.HasIndex(e => e.Username).IsUnique();
            ConfigureAudit(entity);
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.ToTable("Categorias");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Estado).HasDefaultValue(true);
            ConfigureAudit(entity);
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.ToTable("Productos");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Descripcion).HasMaxLength(500);
            entity.Property(e => e.Precio).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.HasOne(e => e.Categoria)
                  .WithMany(c => c.Productos)
                  .HasForeignKey(e => e.CategoriaId)
                  .OnDelete(DeleteBehavior.Restrict);
            ConfigureAudit(entity);
        });

        modelBuilder.Entity<Orden>(entity =>
        {
            entity.ToTable("Ordenes");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Estado).HasMaxLength(20).IsRequired().HasDefaultValue("Pendiente");
            entity.Property(e => e.Total).HasColumnType("decimal(18,2)").IsRequired();
            ConfigureAudit(entity);
        });

        modelBuilder.Entity<DetalleOrden>(entity =>
        {
            entity.ToTable("DetallesOrden");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.Subtotal).HasColumnType("decimal(18,2)").IsRequired();
            entity.HasOne(e => e.Orden)
                  .WithMany(o => o.Detalles)
                  .HasForeignKey(e => e.OrdenId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Producto)
                  .WithMany(p => p.DetallesOrden)
                  .HasForeignKey(e => e.ProductoId)
                  .OnDelete(DeleteBehavior.Restrict);
            ConfigureAudit(entity);
        });
    }

    private static void ConfigureAudit<T>(EntityTypeBuilder<T> entity) where T : AuditableEntity
    {
        entity.Property(e => e.UsuarioCreacion).HasMaxLength(50).IsRequired();
        entity.Property(e => e.FechaCreacion).IsRequired();
        entity.Property(e => e.UsuarioModificacion).HasMaxLength(50);
        entity.Property(e => e.FechaModificacion);
        entity.Property(e => e.UsuarioEliminacion).HasMaxLength(50);
        entity.Property(e => e.FechaEliminacion);
    }
}
