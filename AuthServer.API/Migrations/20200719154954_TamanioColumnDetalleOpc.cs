using Microsoft.EntityFrameworkCore.Migrations;

namespace Expertec.Sigeco.AuthServer.API.Migrations
{
    public partial class TamanioColumnDetalleOpc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NombreControlador",
                schema: "seguridad",
                table: "DetalleOpcion",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(30)");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                schema: "seguridad",
                table: "DetalleOpcion",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)");

            migrationBuilder.UpdateData(
                schema: "seguridad",
                table: "Rol",
                keyColumn: "RolId",
                keyValue: "884215f8-8450-4914-9a3e-7769f01e8f85",
                column: "SelloConcurrencia",
                value: "63a189b6-ade6-49e8-90d7-c6a9f5208fea");

            migrationBuilder.UpdateData(
                schema: "seguridad",
                table: "Rol",
                keyColumn: "RolId",
                keyValue: "9c871375-b636-444e-b75c-0250dd97b64d",
                column: "SelloConcurrencia",
                value: "683c4774-db22-45fe-b102-ba1bd405c637");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NombreControlador",
                schema: "seguridad",
                table: "DetalleOpcion",
                type: "varchar(30)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                schema: "seguridad",
                table: "DetalleOpcion",
                type: "varchar(20)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)");

            migrationBuilder.UpdateData(
                schema: "seguridad",
                table: "Rol",
                keyColumn: "RolId",
                keyValue: "884215f8-8450-4914-9a3e-7769f01e8f85",
                column: "SelloConcurrencia",
                value: "06c4ce2a-f9d0-4cd0-a987-06d7f80bee8e");

            migrationBuilder.UpdateData(
                schema: "seguridad",
                table: "Rol",
                keyColumn: "RolId",
                keyValue: "9c871375-b636-444e-b75c-0250dd97b64d",
                column: "SelloConcurrencia",
                value: "652f8fc4-50f6-4c7c-bfa2-1766612364bb");
        }
    }
}
