using CP4.Catalogo.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CP4.Catalogo.Data;

public sealed class CatalogoDbContext : DbContext
{
    public CatalogoDbContext(DbContextOptions<CatalogoDbContext> options) : base(options)
    {
    }

    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Produto> Produtos => Set<Produto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.ToTable("CATEGORIAS");
            entity.HasKey(categoria => categoria.Id);

            entity.Property(categoria => categoria.Id).HasColumnName("ID");
            entity.Property(categoria => categoria.Nome)
                .HasColumnName("NOME")
                .HasMaxLength(100)
                .IsRequired();
            entity.Property(categoria => categoria.Descricao)
                .HasColumnName("DESCRICAO")
                .HasMaxLength(500);

            entity.HasIndex(categoria => categoria.Nome).IsUnique();
        });

        modelBuilder.Entity<Produto>(entity =>
        {
            entity.ToTable("PRODUTOS");
            entity.HasKey(produto => produto.Id);

            entity.Property(produto => produto.Id).HasColumnName("ID");
            entity.Property(produto => produto.Nome)
                .HasColumnName("NOME")
                .HasMaxLength(150)
                .IsRequired();
            entity.Property(produto => produto.Descricao)
                .HasColumnName("DESCRICAO")
                .HasMaxLength(500);
            entity.Property(produto => produto.Preco)
                .HasColumnName("PRECO")
                .HasPrecision(12, 2);
            entity.Property(produto => produto.QuantidadeEstoque)
                .HasColumnName("QUANTIDADEESTOQUE");
            entity.Property(produto => produto.CategoriaId)
                .HasColumnName("CATEGORIAID");

            entity.HasIndex(produto => produto.CategoriaId);
            entity.HasOne(produto => produto.Categoria)
                .WithMany(categoria => categoria.Produtos)
                .HasForeignKey(produto => produto.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        });
    }
}
