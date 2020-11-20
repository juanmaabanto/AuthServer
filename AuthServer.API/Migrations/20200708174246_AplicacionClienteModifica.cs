using Microsoft.EntityFrameworkCore.Migrations;

namespace Expertec.Sigeco.AuthServer.API.Migrations
{
    public partial class AplicacionClienteModifica : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Permitido",
                schema: "seguridad",
                table: "UsuarioAplicacionCliente",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TieneAcceso",
                schema: "seguridad",
                table: "UsuarioAplicacionCliente",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AplicacionClienteId",
                schema: "seguridad",
                table: "Modulo",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.CreateIndex(
                name: "IX_Modulo_AplicacionClienteId",
                schema: "seguridad",
                table: "Modulo",
                column: "AplicacionClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Modulo_AplicacionCliente_AplicacionClienteId",
                schema: "seguridad",
                table: "Modulo",
                column: "AplicacionClienteId",
                principalSchema: "seguridad",
                principalTable: "AplicacionCliente",
                principalColumn: "AplicacionClienteId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Modulo_AplicacionCliente_AplicacionClienteId",
                schema: "seguridad",
                table: "Modulo");

            migrationBuilder.DropIndex(
                name: "IX_Modulo_AplicacionClienteId",
                schema: "seguridad",
                table: "Modulo");

            migrationBuilder.DropColumn(
                name: "Permitido",
                schema: "seguridad",
                table: "UsuarioAplicacionCliente");

            migrationBuilder.DropColumn(
                name: "TieneAcceso",
                schema: "seguridad",
                table: "UsuarioAplicacionCliente");

            migrationBuilder.DropColumn(
                name: "AplicacionClienteId",
                schema: "seguridad",
                table: "Modulo");

            migrationBuilder.UpdateData(
                schema: "seguridad",
                table: "Rol",
                keyColumn: "RolId",
                keyValue: "884215f8-8450-4914-9a3e-7769f01e8f85",
                column: "SelloConcurrencia",
                value: "5ace4155-0c20-4e9b-a3a8-50311e82b4b9");

            migrationBuilder.UpdateData(
                schema: "seguridad",
                table: "Rol",
                keyColumn: "RolId",
                keyValue: "9c871375-b636-444e-b75c-0250dd97b64d",
                column: "SelloConcurrencia",
                value: "8dc9adf8-1e94-4216-b4b0-a43831313b9b");
        }
    }
}
