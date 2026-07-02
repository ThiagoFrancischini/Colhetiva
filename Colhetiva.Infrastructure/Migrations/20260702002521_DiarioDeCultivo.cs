using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Colhetiva.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DiarioDeCultivo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FotoPerfilUrl",
                table: "Usuarios",
                type: "character varying(2083)",
                maxLength: 2083,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RegistrosAtividades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    HortaId = table.Column<Guid>(type: "uuid", nullable: false),
                    CanteiroId = table.Column<Guid>(type: "uuid", nullable: true),
                    DataHora = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Atividade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Observacoes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FotoUrl = table.Column<string>(type: "character varying(2083)", maxLength: 2083, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosAtividades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosAtividades_Canteiros_CanteiroId",
                        column: x => x.CanteiroId,
                        principalTable: "Canteiros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RegistrosAtividades_Hortas_HortaId",
                        column: x => x.HortaId,
                        principalTable: "Hortas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RegistrosAtividades_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosAtividades_CanteiroId",
                table: "RegistrosAtividades",
                column: "CanteiroId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosAtividades_HortaId",
                table: "RegistrosAtividades",
                column: "HortaId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosAtividades_UsuarioId",
                table: "RegistrosAtividades",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrosAtividades");

            migrationBuilder.DropColumn(
                name: "FotoPerfilUrl",
                table: "Usuarios");
        }
    }
}
