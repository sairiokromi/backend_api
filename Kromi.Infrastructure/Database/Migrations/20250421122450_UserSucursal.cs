using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kromi.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class UserSucursal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SucursalId",
                table: "AspNetUsers",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Sucursal",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    EstaActivo = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sucursal", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SucursalId",
                table: "AspNetUsers",
                column: "SucursalId");

            migrationBuilder.CreateIndex(
                name: "IX_Sucursal_Codigo",
                table: "Sucursal",
                column: "Codigo");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Sucursal_SucursalId",
                table: "AspNetUsers",
                column: "SucursalId",
                principalTable: "Sucursal",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Sucursal_SucursalId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Sucursal");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SucursalId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SucursalId",
                table: "AspNetUsers");
        }
    }
}
