using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Colhetiva.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Organizacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId",
                table: "Usuarios",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId",
                table: "Hortas",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Cnpj = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Tipo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EnderecoId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organizations_Enderecos_EnderecoId",
                        column: x => x.EnderecoId,
                        principalTable: "Enderecos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_OrganizationId",
                table: "Usuarios",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Hortas_OrganizationId",
                table: "Hortas",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_EnderecoId",
                table: "Organizations",
                column: "EnderecoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hortas_Organizations_OrganizationId",
                table: "Hortas",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Organizations_OrganizationId",
                table: "Usuarios",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hortas_Organizations_OrganizationId",
                table: "Hortas");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Organizations_OrganizationId",
                table: "Usuarios");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_OrganizationId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Hortas_OrganizationId",
                table: "Hortas");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Hortas");
        }
    }
}
