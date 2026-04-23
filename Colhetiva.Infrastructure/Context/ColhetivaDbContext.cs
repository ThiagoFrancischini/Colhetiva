using Colhetiva.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
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
            });

            modelBuilder.Entity<UserContext>(entity =>
            {
                entity.HasKey(uc => uc.Id);
                entity.Property(uc => uc.Role).IsRequired();
                entity.Property(uc => uc.HortaId).IsRequired(false);
            });

            modelBuilder.Entity<Endereco>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Rua).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Bairro).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Numero).HasMaxLength(20);
                entity.Property(e => e.Cep).IsRequired().HasMaxLength(8);

                entity.HasOne(e => e.Cidade)
                      .WithMany()
                      .HasForeignKey(e => e.CidadeId)
                      .IsRequired();
            });

            modelBuilder.Entity<Cidade>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Nome).IsRequired().HasMaxLength(100);

                entity.HasOne(c => c.Estado)
                      .WithMany()
                      .HasForeignKey(c => c.EstadoId)
                      .IsRequired();
            });

             modelBuilder.Entity<Estado>(entity =>
             {
                 entity.HasKey(e => e.Id);
 
                 entity.Property(e => e.Nome).IsRequired().HasMaxLength(75);
                 entity.Property(e => e.Sigla).IsRequired().HasMaxLength(2);
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
             });

             modelBuilder.Entity<Canteiro>(entity =>
             {
                 entity.HasKey(c => c.Id);
 
                 entity.Property(c => c.Identificacao).IsRequired().HasMaxLength(50);
                 entity.Property(c => c.Dimensoes).IsRequired(false).HasMaxLength(50);
                 entity.Property(c => c.Status).IsRequired();
 
                 entity.HasOne(c => c.Horta)
                       .WithMany(h => h.Canteiros)
                       .HasForeignKey(c => c.HortaId)
                       .IsRequired();
             });

             modelBuilder.Entity<Solicitacao>(entity =>
             {
                 entity.HasKey(s => s.Id);
 
                 entity.Property(s => s.DataPedido).IsRequired();
                 entity.Property(s => s.Status).IsRequired();
 
                 entity.HasOne(s => s.Usuario)
                       .WithMany()
                       .HasForeignKey(s => s.UsuarioId)
                       .IsRequired();
 
                 entity.HasOne(s => s.Canteiro)
                       .WithMany()
                       .HasForeignKey(s => s.CanteiroId)
                       .IsRequired();
             });

             modelBuilder.Entity<Ferramenta>(entity =>
             {
                 entity.HasKey(f => f.Id);
 
                 entity.Property(f => f.Nome).IsRequired().HasMaxLength(150);
                 entity.Property(f => f.Status).IsRequired();
 
                 entity.HasOne(f => f.Horta)
                       .WithMany(h => h.Ferramentas)
                       .HasForeignKey(f => f.HortaId)
                       .IsRequired();
             });

             modelBuilder.Entity<Emprestimo>(entity =>
             {
                 entity.HasKey(e => e.Id);
 
                 entity.Property(e => e.DataRetirada).IsRequired();
                 entity.Property(e => e.DataDevolucao).IsRequired(false);
 
                 entity.HasOne(e => e.Usuario)
                       .WithMany()
                       .HasForeignKey(e => e.UsuarioId)
                       .IsRequired();
 
                 entity.HasOne(e => e.Ferramenta)
                       .WithMany()
                       .HasForeignKey(e => e.FerramentaId)
                       .IsRequired();
             });
         }
    }
}
