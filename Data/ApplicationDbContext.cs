using finansee_api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace finansee_api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<CategoriaDespesa> CategoriasDespesa => Set<CategoriaDespesa>();
    public DbSet<NaturezaDespesa> NaturezasDespesa => Set<NaturezaDespesa>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Projeto> Projetos => Set<Projeto>();
    public DbSet<ItemOrcado> ItensOrcados => Set<ItemOrcado>();
    public DbSet<ItemRealizado> ItensRealizados => Set<ItemRealizado>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var itemStatusConverter = new ValueConverter<ItemOrcadoStatus, string>(
            value => value == ItemOrcadoStatus.Concluido
                ? "concluido"
                : "nao_iniciado",
            value => value == "concluido"
                ? ItemOrcadoStatus.Concluido
                : ItemOrcadoStatus.NaoIniciado);

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuario");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Nome)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Email)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.SenhaHash)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(x => x.Perfil)
                .HasMaxLength(20)
                .IsRequired();

            entity.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<CategoriaDespesa>(entity =>
        {
            entity.ToTable("categorias_despesa");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Nome)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.Descricao)
                .HasColumnType("text");

            entity.HasData(
                new CategoriaDespesa
                {
                    Id = 1,
                    Nome = "Custeio",
                    Descricao = "Despesas operacionais e de consumo."
                },
                new CategoriaDespesa
                {
                    Id = 2,
                    Nome = "Capital",
                    Descricao = "Despesas com bens permanentes e investimentos."
                });
        });

        modelBuilder.Entity<NaturezaDespesa>(entity =>
        {
            entity.ToTable("naturezas_despesa");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Nome)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.UnidadeMedida)
                .HasMaxLength(20);

            entity.Property(x => x.CategoriaDespesaId)
                .HasColumnName("fk_categoria");

            entity.HasOne(x => x.CategoriaDespesa)
                .WithMany(x => x.Naturezas)
                .HasForeignKey(x => x.CategoriaDespesaId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasData(
                new NaturezaDespesa
                {
                    Id = 1,
                    CategoriaDespesaId = 1,
                    Nome = "Material de Consumo",
                    UnidadeMedida = "un"
                },
                new NaturezaDespesa
                {
                    Id = 2,
                    CategoriaDespesaId = 1,
                    Nome = "Serviços de Terceiros",
                    UnidadeMedida = "serv"
                },
                new NaturezaDespesa
                {
                    Id = 3,
                    CategoriaDespesaId = 2,
                    Nome = "Equipamentos",
                    UnidadeMedida = "un"
                },
                new NaturezaDespesa
                {
                    Id = 4,
                    CategoriaDespesaId = 2,
                    Nome = "Software",
                    UnidadeMedida = "lic"
                });
        });

        modelBuilder.Entity<Projeto>(entity =>
        {
            entity.ToTable("projeto");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Titulo)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.NumeroEdital)
                .HasMaxLength(50);

            entity.Property(x => x.ValorTotalOrcamento)
                .HasPrecision(15, 2);

            entity.Property(x => x.UsuarioResponsavelId)
                .HasColumnName("fk_usuario_responsavel");

            entity.HasOne(x => x.UsuarioResponsavel)
                .WithMany(x => x.ProjetosResponsaveis)
                .HasForeignKey(x => x.UsuarioResponsavelId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ItemOrcado>(entity =>
        {
            entity.ToTable("itens_orcado");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Status)
                .HasConversion(itemStatusConverter)
                .HasMaxLength(20);

            entity.Property(x => x.ProjetoId)
                .HasColumnName("fk_projeto");

            entity.Property(x => x.NaturezaDespesaId)
                .HasColumnName("fk_natureza");

            entity.Property(x => x.Descricao)
                .HasColumnType("text");

            entity.Property(x => x.QuantidadeOrcada)
                .HasPrecision(12, 2);

            entity.Property(x => x.ValorUnitarioOrcado)
                .HasPrecision(15, 2);

            entity.Property(x => x.ValorTotalOrcado)
                .HasPrecision(15, 2);

            entity.HasOne(x => x.Projeto)
                .WithMany(x => x.ItensOrcados)
                .HasForeignKey(x => x.ProjetoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.NaturezaDespesa)
                .WithMany(x => x.ItensOrcados)
                .HasForeignKey(x => x.NaturezaDespesaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ItemRealizado>(entity =>
        {
            entity.ToTable("itens_realizado");
            entity.HasKey(x => x.Id);

            entity.Property(x => x.ItemOrcadoId)
                .HasColumnName("fk_item_orcado");

            entity.Property(x => x.ProjetoId)
                .HasColumnName("fk_projeto");

            entity.Property(x => x.NaturezaDespesaId)
                .HasColumnName("fk_natureza");

            entity.Property(x => x.UsuarioRegistroId)
                .HasColumnName("fk_usuario_registro");

            entity.Property(x => x.NumeroDocumento)
                .HasMaxLength(50);

            entity.Property(x => x.Fornecedor)
                .HasMaxLength(200);

            entity.Property(x => x.Justificativa)
                .HasColumnType("text");

            entity.Property(x => x.Observacao)
                .HasColumnType("text");

            entity.Property(x => x.QuantidadeRealizada)
                .HasPrecision(12, 2);

            entity.Property(x => x.ValorUnitarioRealizado)
                .HasPrecision(15, 2);

            entity.Property(x => x.ValorTotalRealizado)
                .HasPrecision(15, 2);

            entity.HasOne(x => x.ItemOrcado)
                .WithMany(x => x.ItensRealizados)
                .HasForeignKey(x => x.ItemOrcadoId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(x => x.Projeto)
                .WithMany(x => x.ItensRealizados)
                .HasForeignKey(x => x.ProjetoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.NaturezaDespesa)
                .WithMany(x => x.ItensRealizados)
                .HasForeignKey(x => x.NaturezaDespesaId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(x => x.UsuarioRegistro)
                .WithMany(x => x.ItensRegistrados)
                .HasForeignKey(x => x.UsuarioRegistroId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
