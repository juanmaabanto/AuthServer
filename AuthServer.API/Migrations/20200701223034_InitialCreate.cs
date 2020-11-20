using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Expertec.Sigeco.AuthServer.API.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "seguridad");

            migrationBuilder.CreateTable(
                name: "AplicacionCliente",
                schema: "seguridad",
                columns: table => new
                {
                    AplicacionClienteId = table.Column<string>(maxLength: 128, nullable: false),
                    Secreto = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(type: "varchar(50)", nullable: false),
                    TipoAplicacion = table.Column<int>(nullable: false),
                    TiempoVidaTokenActualizacion = table.Column<int>(nullable: false),
                    OrigenPermitido = table.Column<string>(type: "varchar(50)", nullable: false),
                    RedirigirUri = table.Column<string>(type: "varchar(100)", nullable: false),
                    EsTercero = table.Column<bool>(nullable: false),
                    Activo = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AplicacionCliente", x => x.AplicacionClienteId);
                });

            migrationBuilder.CreateTable(
                name: "Modulo",
                schema: "seguridad",
                columns: table => new
                {
                    ModuloId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(30)", nullable: false),
                    NombreCorto = table.Column<string>(type: "varchar(30)", nullable: false),
                    NombreRuta = table.Column<string>(type: "varchar(30)", nullable: false),
                    UriRuta = table.Column<string>(maxLength: 256, nullable: false),
                    Imagen = table.Column<string>(type: "varchar(15)", nullable: false),
                    Activo = table.Column<bool>(nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "varchar(30)", nullable: false),
                    NombreHost = table.Column<string>(type: "varchar(30)", nullable: false),
                    Host = table.Column<string>(maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modulo", x => x.ModuloId);
                });

            migrationBuilder.CreateTable(
                name: "Persona",
                schema: "seguridad",
                columns: table => new
                {
                    PersonaId = table.Column<int>(nullable: false),
                    NroDocumento = table.Column<string>(type: "varchar(15)", nullable: false),
                    PrimerNombre = table.Column<string>(type: "varchar(30)", nullable: false),
                    SegundoNombre = table.Column<string>(type: "varchar(30)", nullable: true),
                    ApellidoPaterno = table.Column<string>(type: "varchar(50)", nullable: false),
                    ApellidoMaterno = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persona", x => x.PersonaId);
                });

            migrationBuilder.CreateTable(
                name: "Recurso",
                schema: "seguridad",
                columns: table => new
                {
                    RecursoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(30)", nullable: false),
                    NombreHost = table.Column<string>(type: "varchar(30)", nullable: false),
                    UriHost = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    Activo = table.Column<bool>(nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recurso", x => x.RecursoId);
                });

            migrationBuilder.CreateTable(
                name: "Rol",
                schema: "seguridad",
                columns: table => new
                {
                    RolId = table.Column<string>(maxLength: 128, nullable: false),
                    Nombre = table.Column<string>(maxLength: 256, nullable: false),
                    NombreNormalizado = table.Column<string>(maxLength: 256, nullable: false),
                    SelloConcurrencia = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rol", x => x.RolId);
                });

            migrationBuilder.CreateTable(
                name: "TipoEspacioTrabajo",
                schema: "seguridad",
                columns: table => new
                {
                    TipoEspacioTrabajoId = table.Column<int>(nullable: false),
                    Nombre = table.Column<string>(type: "varchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoEspacioTrabajo", x => x.TipoEspacioTrabajoId);
                });

            migrationBuilder.CreateTable(
                name: "Opcion",
                schema: "seguridad",
                columns: table => new
                {
                    OpcionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModuloId = table.Column<int>(nullable: false),
                    PadreId = table.Column<int>(nullable: true),
                    Nombre = table.Column<string>(type: "varchar(30)", nullable: false),
                    Tooltip = table.Column<string>(type: "varchar(100)", nullable: false),
                    Secuencia = table.Column<int>(nullable: false),
                    ViewClass = table.Column<string>(type: "varchar(100)", nullable: false),
                    ViewType = table.Column<string>(type: "varchar(50)", nullable: false),
                    Icono = table.Column<string>(type: "varchar(30)", nullable: false),
                    Formulario = table.Column<bool>(nullable: false),
                    Activo = table.Column<bool>(nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: false),
                    Reactivo = table.Column<bool>(nullable: false),
                    Ruta = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Opcion", x => x.OpcionId);
                    table.ForeignKey(
                        name: "FK_Opcion_Modulo_ModuloId",
                        column: x => x.ModuloId,
                        principalSchema: "seguridad",
                        principalTable: "Modulo",
                        principalColumn: "ModuloId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Opcion_Opcion_PadreId",
                        column: x => x.PadreId,
                        principalSchema: "seguridad",
                        principalTable: "Opcion",
                        principalColumn: "OpcionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ModuloRecurso",
                schema: "seguridad",
                columns: table => new
                {
                    ModuloId = table.Column<int>(nullable: false),
                    RecursoId = table.Column<int>(nullable: false),
                    Activo = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuloRecurso", x => new { x.ModuloId, x.RecursoId });
                    table.ForeignKey(
                        name: "FK_ModuloRecurso_Modulo_ModuloId",
                        column: x => x.ModuloId,
                        principalSchema: "seguridad",
                        principalTable: "Modulo",
                        principalColumn: "ModuloId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ModuloRecurso_Recurso_RecursoId",
                        column: x => x.RecursoId,
                        principalSchema: "seguridad",
                        principalTable: "Recurso",
                        principalColumn: "RecursoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EspacioTrabajo",
                schema: "seguridad",
                columns: table => new
                {
                    EspacioTrabajoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoEspacioTrabajoId = table.Column<int>(nullable: false),
                    Codigo = table.Column<string>(type: "varchar(5)", nullable: false),
                    Nombre = table.Column<string>(type: "varchar(50)", nullable: false),
                    Dominio = table.Column<string>(type: "varchar(20)", nullable: false),
                    Contacto = table.Column<string>(type: "varchar(100)", nullable: false),
                    Direccion = table.Column<string>(type: "varchar(100)", nullable: false),
                    Ciudad = table.Column<string>(type: "varchar(20)", nullable: false),
                    Telefono = table.Column<string>(type: "varchar(20)", nullable: false),
                    Correo = table.Column<string>(type: "varchar(30)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime", nullable: true),
                    Activo = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EspacioTrabajo", x => x.EspacioTrabajoId);
                    table.ForeignKey(
                        name: "FK_EspacioTrabajo_TipoEspacioTrabajo_TipoEspacioTrabajoId",
                        column: x => x.TipoEspacioTrabajoId,
                        principalSchema: "seguridad",
                        principalTable: "TipoEspacioTrabajo",
                        principalColumn: "TipoEspacioTrabajoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DetalleOpcion",
                schema: "seguridad",
                columns: table => new
                {
                    DetalleOpcionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OpcionId = table.Column<int>(nullable: false),
                    Nombre = table.Column<string>(type: "varchar(20)", nullable: false),
                    NombreControlador = table.Column<string>(type: "varchar(30)", nullable: false),
                    NombreAccion = table.Column<string>(type: "varchar(50)", nullable: false),
                    Activo = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleOpcion", x => x.DetalleOpcionId);
                    table.ForeignKey(
                        name: "FK_DetalleOpcion_Opcion_OpcionId",
                        column: x => x.OpcionId,
                        principalSchema: "seguridad",
                        principalTable: "Opcion",
                        principalColumn: "OpcionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DetalleEspacioTrabajo",
                schema: "seguridad",
                columns: table => new
                {
                    DetalleEspacioTrabajoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EspacioTrabajoId = table.Column<int>(nullable: false),
                    ModuloId = table.Column<int>(nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime", nullable: true),
                    Activo = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleEspacioTrabajo", x => x.DetalleEspacioTrabajoId);
                    table.ForeignKey(
                        name: "FK_DetalleEspacioTrabajo_EspacioTrabajo_EspacioTrabajoId",
                        column: x => x.EspacioTrabajoId,
                        principalSchema: "seguridad",
                        principalTable: "EspacioTrabajo",
                        principalColumn: "EspacioTrabajoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetalleEspacioTrabajo_Modulo_ModuloId",
                        column: x => x.ModuloId,
                        principalSchema: "seguridad",
                        principalTable: "Modulo",
                        principalColumn: "ModuloId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Empresa",
                schema: "seguridad",
                columns: table => new
                {
                    EmpresaId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EspacioTrabajoId = table.Column<int>(nullable: false),
                    Codigo = table.Column<string>(type: "varchar(5)", nullable: false),
                    Ruc = table.Column<string>(type: "varchar(11)", nullable: false),
                    RazonSocial = table.Column<string>(type: "varchar(50)", nullable: false),
                    DomicilioFiscal = table.Column<string>(type: "varchar(200)", nullable: false),
                    NombreComercial = table.Column<string>(type: "varchar(100)", nullable: false),
                    Ubigeo = table.Column<string>(type: "varchar(10)", nullable: false),
                    CuentaDetraccion = table.Column<string>(type: "varchar(30)", nullable: true),
                    Logo = table.Column<string>(type: "varchar(50)", nullable: true),
                    ImagenUri = table.Column<string>(type: "varchar(50)", nullable: false),
                    ExoneradoIGV = table.Column<bool>(nullable: false),
                    IncluyeIGV = table.Column<bool>(nullable: false),
                    Activo = table.Column<bool>(nullable: false),
                    Anulado = table.Column<bool>(nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "varchar(30)", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    UsuarioModificacion = table.Column<string>(type: "varchar(30)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresa", x => x.EmpresaId);
                    table.ForeignKey(
                        name: "FK_Empresa_EspacioTrabajo_EspacioTrabajoId",
                        column: x => x.EspacioTrabajoId,
                        principalSchema: "seguridad",
                        principalTable: "EspacioTrabajo",
                        principalColumn: "EspacioTrabajoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                schema: "seguridad",
                columns: table => new
                {
                    UsuarioId = table.Column<string>(maxLength: 128, nullable: false),
                    NombreUsuario = table.Column<string>(maxLength: 256, nullable: false),
                    NombreUsuarioNormalizado = table.Column<string>(maxLength: 256, nullable: false),
                    Correo = table.Column<string>(maxLength: 256, nullable: true),
                    CorreoNormalizado = table.Column<string>(maxLength: 256, nullable: true),
                    CorreoConfirmado = table.Column<bool>(nullable: false),
                    Clave = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SelloSeguridad = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SelloConcurrencia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TelefonoConfirmado = table.Column<bool>(nullable: false),
                    DobleFactorHabilitado = table.Column<bool>(nullable: false),
                    FinBloqueo = table.Column<DateTimeOffset>(nullable: true),
                    BloqueoHabilitado = table.Column<bool>(nullable: false),
                    CuentaAccesoFallido = table.Column<int>(nullable: false),
                    EspacioTrabajoId = table.Column<int>(nullable: false),
                    PersonaId = table.Column<int>(nullable: false),
                    Nombre = table.Column<string>(type: "varchar(50)", nullable: false),
                    Imagen = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ExpiraClave = table.Column<DateTimeOffset>(nullable: true),
                    ExpiraClaveHabilitado = table.Column<bool>(nullable: false),
                    RequiereCambioClave = table.Column<bool>(nullable: false),
                    Activo = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.UsuarioId);
                    table.ForeignKey(
                        name: "FK_Usuario_EspacioTrabajo_EspacioTrabajoId",
                        column: x => x.EspacioTrabajoId,
                        principalSchema: "seguridad",
                        principalTable: "EspacioTrabajo",
                        principalColumn: "EspacioTrabajoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Usuario_Persona_PersonaId",
                        column: x => x.PersonaId,
                        principalSchema: "seguridad",
                        principalTable: "Persona",
                        principalColumn: "PersonaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Autorizacion",
                schema: "seguridad",
                columns: table => new
                {
                    EmpresaId = table.Column<int>(nullable: false),
                    OpcionId = table.Column<int>(nullable: false),
                    UsuarioId = table.Column<string>(nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "varchar(30)", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    UsuarioModificacion = table.Column<string>(type: "varchar(30)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Autorizacion", x => new { x.EmpresaId, x.OpcionId, x.UsuarioId });
                    table.ForeignKey(
                        name: "FK_Autorizacion_Empresa_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "seguridad",
                        principalTable: "Empresa",
                        principalColumn: "EmpresaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Autorizacion_Opcion_OpcionId",
                        column: x => x.OpcionId,
                        principalSchema: "seguridad",
                        principalTable: "Opcion",
                        principalColumn: "OpcionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Autorizacion_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalSchema: "seguridad",
                        principalTable: "Usuario",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Favorito",
                schema: "seguridad",
                columns: table => new
                {
                    EmpresaId = table.Column<int>(nullable: false),
                    OpcionId = table.Column<int>(nullable: false),
                    UsuarioId = table.Column<string>(nullable: false),
                    Activo = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorito", x => new { x.EmpresaId, x.OpcionId, x.UsuarioId });
                    table.ForeignKey(
                        name: "FK_Favorito_Empresa_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "seguridad",
                        principalTable: "Empresa",
                        principalColumn: "EmpresaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Favorito_Opcion_OpcionId",
                        column: x => x.OpcionId,
                        principalSchema: "seguridad",
                        principalTable: "Opcion",
                        principalColumn: "OpcionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Favorito_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalSchema: "seguridad",
                        principalTable: "Usuario",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistoricoClave",
                schema: "seguridad",
                columns: table => new
                {
                    HistoricoClaveId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<string>(nullable: false),
                    Clave = table.Column<string>(nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricoClave", x => x.HistoricoClaveId);
                    table.ForeignKey(
                        name: "FK_HistoricoClave_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalSchema: "seguridad",
                        principalTable: "Usuario",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TokenActualizacion",
                schema: "seguridad",
                columns: table => new
                {
                    TokenActualizacionId = table.Column<string>(maxLength: 128, nullable: false),
                    AplicacionClienteId = table.Column<string>(maxLength: 128, nullable: false),
                    UsuarioId = table.Column<string>(maxLength: 128, nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "datetime", nullable: false),
                    FechaExpiracion = table.Column<DateTime>(type: "datetime", nullable: false),
                    TicketProtegido = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenActualizacion", x => x.TokenActualizacionId);
                    table.ForeignKey(
                        name: "FK_TokenActualizacion_AplicacionCliente_AplicacionClienteId",
                        column: x => x.AplicacionClienteId,
                        principalSchema: "seguridad",
                        principalTable: "AplicacionCliente",
                        principalColumn: "AplicacionClienteId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TokenActualizacion_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalSchema: "seguridad",
                        principalTable: "Usuario",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioAplicacionCliente",
                schema: "seguridad",
                columns: table => new
                {
                    UsuarioId = table.Column<string>(maxLength: 128, nullable: false),
                    AplicacionClienteId = table.Column<string>(maxLength: 128, nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioAplicacionCliente", x => new { x.UsuarioId, x.AplicacionClienteId });
                    table.ForeignKey(
                        name: "FK_UsuarioAplicacionCliente_AplicacionCliente_AplicacionClienteId",
                        column: x => x.AplicacionClienteId,
                        principalSchema: "seguridad",
                        principalTable: "AplicacionCliente",
                        principalColumn: "AplicacionClienteId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuarioAplicacionCliente_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalSchema: "seguridad",
                        principalTable: "Usuario",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioEmpresa",
                schema: "seguridad",
                columns: table => new
                {
                    UsuarioId = table.Column<string>(nullable: false),
                    EmpresaId = table.Column<int>(nullable: false),
                    Principal = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioEmpresa", x => new { x.UsuarioId, x.EmpresaId });
                    table.ForeignKey(
                        name: "FK_UsuarioEmpresa_Empresa_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "seguridad",
                        principalTable: "Empresa",
                        principalColumn: "EmpresaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuarioEmpresa_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalSchema: "seguridad",
                        principalTable: "Usuario",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioReclamacion",
                schema: "seguridad",
                columns: table => new
                {
                    UsuarioReclamacionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<string>(maxLength: 128, nullable: false),
                    Tipo = table.Column<string>(nullable: false),
                    Valor = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioReclamacion", x => x.UsuarioReclamacionId);
                    table.ForeignKey(
                        name: "FK_UsuarioReclamacion_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalSchema: "seguridad",
                        principalTable: "Usuario",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioRol",
                schema: "seguridad",
                columns: table => new
                {
                    UsuarioId = table.Column<string>(maxLength: 128, nullable: false),
                    RolId = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioRol", x => new { x.UsuarioId, x.RolId });
                    table.ForeignKey(
                        name: "FK_UsuarioRol_Rol_RolId",
                        column: x => x.RolId,
                        principalSchema: "seguridad",
                        principalTable: "Rol",
                        principalColumn: "RolId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuarioRol_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalSchema: "seguridad",
                        principalTable: "Usuario",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DetalleAutorizacion",
                schema: "seguridad",
                columns: table => new
                {
                    EmpresaId = table.Column<int>(nullable: false),
                    OpcionId = table.Column<int>(nullable: false),
                    UsuarioId = table.Column<string>(nullable: false),
                    DetalleOpcionId = table.Column<int>(nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "varchar(30)", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    UsuarioModificacion = table.Column<string>(type: "varchar(30)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleAutorizacion", x => new { x.EmpresaId, x.OpcionId, x.UsuarioId, x.DetalleOpcionId });
                    table.ForeignKey(
                        name: "FK_DetalleAutorizacion_DetalleOpcion_DetalleOpcionId",
                        column: x => x.DetalleOpcionId,
                        principalSchema: "seguridad",
                        principalTable: "DetalleOpcion",
                        principalColumn: "DetalleOpcionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetalleAutorizacion_Autorizacion_EmpresaId_OpcionId_UsuarioId",
                        columns: x => new { x.EmpresaId, x.OpcionId, x.UsuarioId },
                        principalSchema: "seguridad",
                        principalTable: "Autorizacion",
                        principalColumns: new[] { "EmpresaId", "OpcionId", "UsuarioId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "seguridad",
                table: "Rol",
                columns: new[] { "RolId", "SelloConcurrencia", "Nombre", "NombreNormalizado" },
                values: new object[,]
                {
                    { "884215f8-8450-4914-9a3e-7769f01e8f85", "5ace4155-0c20-4e9b-a3a8-50311e82b4b9", "Administrador", "ADMINISTRADOR" },
                    { "9c871375-b636-444e-b75c-0250dd97b64d", "8dc9adf8-1e94-4216-b4b0-a43831313b9b", "Usuario", "USUARIO" }
                });

            migrationBuilder.InsertData(
                schema: "seguridad",
                table: "TipoEspacioTrabajo",
                columns: new[] { "TipoEspacioTrabajoId", "Nombre" },
                values: new object[,]
                {
                    { 1, "production" },
                    { 2, "test" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Autorizacion_OpcionId",
                schema: "seguridad",
                table: "Autorizacion",
                column: "OpcionId");

            migrationBuilder.CreateIndex(
                name: "IX_Autorizacion_UsuarioId",
                schema: "seguridad",
                table: "Autorizacion",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleAutorizacion_DetalleOpcionId",
                schema: "seguridad",
                table: "DetalleAutorizacion",
                column: "DetalleOpcionId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleEspacioTrabajo_EspacioTrabajoId",
                schema: "seguridad",
                table: "DetalleEspacioTrabajo",
                column: "EspacioTrabajoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleEspacioTrabajo_ModuloId",
                schema: "seguridad",
                table: "DetalleEspacioTrabajo",
                column: "ModuloId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleOpcion_OpcionId",
                schema: "seguridad",
                table: "DetalleOpcion",
                column: "OpcionId");

            migrationBuilder.CreateIndex(
                name: "IX_Empresa_EspacioTrabajoId",
                schema: "seguridad",
                table: "Empresa",
                column: "EspacioTrabajoId");

            migrationBuilder.CreateIndex(
                name: "IX_EspacioTrabajo_TipoEspacioTrabajoId",
                schema: "seguridad",
                table: "EspacioTrabajo",
                column: "TipoEspacioTrabajoId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorito_OpcionId",
                schema: "seguridad",
                table: "Favorito",
                column: "OpcionId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorito_UsuarioId",
                schema: "seguridad",
                table: "Favorito",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoClave_UsuarioId",
                schema: "seguridad",
                table: "HistoricoClave",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuloRecurso_RecursoId",
                schema: "seguridad",
                table: "ModuloRecurso",
                column: "RecursoId");

            migrationBuilder.CreateIndex(
                name: "IX_Opcion_ModuloId",
                schema: "seguridad",
                table: "Opcion",
                column: "ModuloId");

            migrationBuilder.CreateIndex(
                name: "IX_Opcion_PadreId",
                schema: "seguridad",
                table: "Opcion",
                column: "PadreId");

            migrationBuilder.CreateIndex(
                name: "IX_TokenActualizacion_AplicacionClienteId",
                schema: "seguridad",
                table: "TokenActualizacion",
                column: "AplicacionClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_TokenActualizacion_UsuarioId",
                schema: "seguridad",
                table: "TokenActualizacion",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_EspacioTrabajoId",
                schema: "seguridad",
                table: "Usuario",
                column: "EspacioTrabajoId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_PersonaId",
                schema: "seguridad",
                table: "Usuario",
                column: "PersonaId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioAplicacionCliente_AplicacionClienteId",
                schema: "seguridad",
                table: "UsuarioAplicacionCliente",
                column: "AplicacionClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioEmpresa_EmpresaId",
                schema: "seguridad",
                table: "UsuarioEmpresa",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioReclamacion_UsuarioId",
                schema: "seguridad",
                table: "UsuarioReclamacion",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRol_RolId",
                schema: "seguridad",
                table: "UsuarioRol",
                column: "RolId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetalleAutorizacion",
                schema: "seguridad");

            migrationBuilder.DropTable(
                name: "DetalleEspacioTrabajo",
                schema: "seguridad");

            migrationBuilder.DropTable(
                name: "Favorito",
                schema: "seguridad");

            migrationBuilder.DropTable(
                name: "HistoricoClave",
                schema: "seguridad");

            migrationBuilder.DropTable(
                name: "ModuloRecurso",
                schema: "seguridad");

            migrationBuilder.DropTable(
                name: "TokenActualizacion",
                schema: "seguridad");

            migrationBuilder.DropTable(
                name: "UsuarioAplicacionCliente",
                schema: "seguridad");

            migrationBuilder.DropTable(
                name: "UsuarioEmpresa",
                schema: "seguridad");

            migrationBuilder.DropTable(
                name: "UsuarioReclamacion",
                schema: "seguridad");

            migrationBuilder.DropTable(
                name: "UsuarioRol",
                schema: "seguridad");

            migrationBuilder.DropTable(
                name: "DetalleOpcion",
                schema: "seguridad");

            migrationBuilder.DropTable(
                name: "Autorizacion",
                schema: "seguridad");

            migrationBuilder.DropTable(
                name: "Recurso",
                schema: "seguridad");

            migrationBuilder.DropTable(
                name: "AplicacionCliente",
                schema: "seguridad");

            migrationBuilder.DropTable(
                name: "Rol",
                schema: "seguridad");

            migrationBuilder.DropTable(
                name: "Empresa",
                schema: "seguridad");

            migrationBuilder.DropTable(
                name: "Opcion",
                schema: "seguridad");

            migrationBuilder.DropTable(
                name: "Usuario",
                schema: "seguridad");

            migrationBuilder.DropTable(
                name: "Modulo",
                schema: "seguridad");

            migrationBuilder.DropTable(
                name: "EspacioTrabajo",
                schema: "seguridad");

            migrationBuilder.DropTable(
                name: "Persona",
                schema: "seguridad");

            migrationBuilder.DropTable(
                name: "TipoEspacioTrabajo",
                schema: "seguridad");
        }
    }
}
