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
        }
    }
}
