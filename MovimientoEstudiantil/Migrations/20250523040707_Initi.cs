using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovimientoEstudiantil.Migrations
{
    /// <inheritdoc />
    public partial class Initi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Provincia",
                columns: table => new
                {
                    id_provincia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provincia", x => x.id_provincia);
                });

            migrationBuilder.CreateTable(
                name: "Sede",
                columns: table => new
                {
                    id_sede = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    provincia_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sede", x => x.id_sede);
                    table.ForeignKey(
                        name: "FK_Sede_Provincia_provincia_id",
                        column: x => x.provincia_id,
                        principalTable: "Provincia",
                        principalColumn: "id_provincia",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Estudiante",
                columns: table => new
                {
                    id_estudiante = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    correo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    provincia_id = table.Column<int>(type: "int", nullable: false),
                    sede_id = table.Column<int>(type: "int", nullable: false),
                    satisfaccion_carrera = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    anioIngreso = table.Column<int>(type: "int", nullable: false),
                    ProvinciaidProvincia = table.Column<int>(type: "int", nullable: true),
                    SedeidSede = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estudiante", x => x.id_estudiante);
                    table.ForeignKey(
                        name: "FK_Estudiante_Provincia_ProvinciaidProvincia",
                        column: x => x.ProvinciaidProvincia,
                        principalTable: "Provincia",
                        principalColumn: "id_provincia");
                    table.ForeignKey(
                        name: "FK_Estudiante_Provincia_provincia_id",
                        column: x => x.provincia_id,
                        principalTable: "Provincia",
                        principalColumn: "id_provincia",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Estudiante_Sede_SedeidSede",
                        column: x => x.SedeidSede,
                        principalTable: "Sede",
                        principalColumn: "id_sede");
                    table.ForeignKey(
                        name: "FK_Estudiante_Sede_sede_id",
                        column: x => x.sede_id,
                        principalTable: "Sede",
                        principalColumn: "id_sede",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    id_usuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sede_id = table.Column<int>(type: "int", nullable: false),
                    correo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    contrasena = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    rol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    fecha_registro = table.Column<DateTime>(type: "date", nullable: false),
                    SedeidSede = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.id_usuario);
                    table.ForeignKey(
                        name: "FK_Usuario_Sede_SedeidSede",
                        column: x => x.SedeidSede,
                        principalTable: "Sede",
                        principalColumn: "id_sede");
                    table.ForeignKey(
                        name: "FK_Usuario_Sede_sede_id",
                        column: x => x.sede_id,
                        principalTable: "Sede",
                        principalColumn: "id_sede",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistorialRegistros",
                columns: table => new
                {
                    id_historial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    usuario_id = table.Column<int>(type: "int", nullable: false),
                    accion = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    fecha_registro = table.Column<DateTime>(type: "date", nullable: false),
                    hora_registro = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialRegistros", x => x.id_historial);
                    table.ForeignKey(
                        name: "FK_HistorialRegistros_Usuario_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "Usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Estudiante_provincia_id",
                table: "Estudiante",
                column: "provincia_id");

            migrationBuilder.CreateIndex(
                name: "IX_Estudiante_ProvinciaidProvincia",
                table: "Estudiante",
                column: "ProvinciaidProvincia");

            migrationBuilder.CreateIndex(
                name: "IX_Estudiante_sede_id",
                table: "Estudiante",
                column: "sede_id");

            migrationBuilder.CreateIndex(
                name: "IX_Estudiante_SedeidSede",
                table: "Estudiante",
                column: "SedeidSede");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialRegistros_usuario_id",
                table: "HistorialRegistros",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_Sede_provincia_id",
                table: "Sede",
                column: "provincia_id");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_sede_id",
                table: "Usuario",
                column: "sede_id");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_SedeidSede",
                table: "Usuario",
                column: "SedeidSede");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Estudiante");

            migrationBuilder.DropTable(
                name: "HistorialRegistros");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Sede");

            migrationBuilder.DropTable(
                name: "Provincia");
        }
    }
}
