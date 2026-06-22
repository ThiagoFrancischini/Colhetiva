using Colhetiva.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;

namespace Colhetiva.Infrastructure.Context
{
    public class ColhetivaDbContext : DbContext
    {
        public ColhetivaDbContext(DbContextOptions<ColhetivaDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<Cidade> Cidades { get; set; }
        public DbSet<Estado> Estados { get; set; }
        public DbSet<UserContext> UserContexts { get; set; }
        public DbSet<Horta> Hortas { get; set; }
        public DbSet<Canteiro> Canteiros { get; set; }
        public DbSet<Solicitacao> Solicitacoes { get; set; }
        public DbSet<Ferramenta> Ferramentas { get; set; }
        public DbSet<Emprestimo> Emprestimos { get; set; }
        public DbSet<Organization> Organizations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.HasKey(o => o.Id);

                entity.Property(o => o.Nome).IsRequired().HasMaxLength(200);
                entity.Property(o => o.Cnpj).IsRequired(false).HasMaxLength(20);
                entity.Property(o => o.Tipo).IsRequired(false).HasMaxLength(100);

                entity.HasOne(o => o.Endereco)
                      .WithMany()
                      .HasForeignKey(o => o.EnderecoId)
                      .IsRequired(false);

                entity.HasMany(o => o.Hortas)
                      .WithOne(h => h.Organization)
                      .HasForeignKey(h => h.OrganizationId)
                      .IsRequired(false);
            });

            modelBuilder.Entity<Horta>(entity =>
            {
                entity.HasKey(h => h.Id);

                entity.Property(h => h.Nome).IsRequired().HasMaxLength(150);
                entity.Property(h => h.Regras).IsRequired(false);

                entity.HasOne(h => h.Endereco)
                      .WithMany()
                      .HasForeignKey(h => h.EnderecoId)
                      .IsRequired();

                entity.HasOne(h => h.Usuario)
                      .WithMany()
                      .HasForeignKey(h => h.UsuarioId)
                      .IsRequired();

                entity.HasOne(h => h.Organization)
                      .WithMany(o => o.Hortas)
                      .HasForeignKey(h => h.OrganizationId)
                      .IsRequired(false);
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Nome).IsRequired().HasMaxLength(150);
                entity.Property(u => u.CPF).IsRequired().HasMaxLength(11);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(100);

                entity.HasOne(u => u.Endereco)
                      .WithOne()
                      .HasForeignKey<Usuario>(u => u.EnderecoId)
                      .IsRequired();

                entity.HasMany(u => u.UserContexts)
                      .WithOne(uc => uc.Usuario)
                      .HasForeignKey(uc => uc.UsuarioId)
                      .IsRequired();

                entity.HasOne(u => u.Organization)
                      .WithMany(o => o.Usuarios)
                      .HasForeignKey(u => u.OrganizationId)
                      .IsRequired(false);
            }

        }
    }
}