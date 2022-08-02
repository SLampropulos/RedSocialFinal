using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RedSocialFinal.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    palabra = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    dni = table.Column<string>(type: "varchar(50)", nullable: false),
                    nombre = table.Column<string>(type: "varchar(50)", nullable: false),
                    apellido = table.Column<string>(type: "varchar(50)", nullable: false),
                    mail = table.Column<string>(type: "varchar(255)", nullable: false),
                    pass = table.Column<string>(type: "varchar(50)", nullable: false),
                    intentosFallidos = table.Column<int>(type: "int", nullable: false),
                    bloqueado = table.Column<bool>(type: "bit", nullable: false),
                    esAdmin = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    contenido = table.Column<string>(type: "varchar(255)", nullable: false),
                    fecha = table.Column<DateTime>(type: "datetime", nullable: false),
                    idUsuario = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.id);
                    table.ForeignKey(
                        name: "FK_Posts_Usuarios_idUsuario",
                        column: x => x.idUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Usuario_Amigo",
                columns: table => new
                {
                    num_usr = table.Column<int>(type: "int", nullable: false),
                    num_usr2 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario_Amigo", x => new { x.num_usr, x.num_usr2 });
                    table.ForeignKey(
                        name: "FK_Usuario_Amigo_Usuarios_num_usr",
                        column: x => x.num_usr,
                        principalTable: "Usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Usuario_Amigo_Usuarios_num_usr2",
                        column: x => x.num_usr2,
                        principalTable: "Usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comentarios",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    contenido = table.Column<string>(type: "varchar(255)", nullable: false),
                    fecha = table.Column<DateTime>(type: "datetime", nullable: false),
                    idUsuario = table.Column<int>(type: "int", nullable: false),
                    idPost = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comentarios", x => x.id);
                    table.ForeignKey(
                        name: "FK_Comentarios_Posts_idPost",
                        column: x => x.idPost,
                        principalTable: "Posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comentarios_Usuarios_idUsuario",
                        column: x => x.idUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reacciones",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tipo = table.Column<int>(type: "int", nullable: false),
                    idUsuario = table.Column<int>(type: "int", nullable: false),
                    idPost = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reacciones", x => x.id);
                    table.ForeignKey(
                        name: "FK_Reacciones_Posts_idPost",
                        column: x => x.idPost,
                        principalTable: "Posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reacciones_Usuarios_idUsuario",
                        column: x => x.idUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tag_Post",
                columns: table => new
                {
                    idTag = table.Column<int>(type: "int", nullable: false),
                    idPost = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag_Post", x => new { x.idTag, x.idPost });
                    table.ForeignKey(
                        name: "FK_Tag_Post_Posts_idPost",
                        column: x => x.idPost,
                        principalTable: "Posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tag_Post_Tags_idTag",
                        column: x => x.idTag,
                        principalTable: "Tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "id", "palabra" },
                values: new object[,]
                {
                    { 1, "111" },
                    { 2, "222" },
                    { 3, "333" },
                    { 4, "444" },
                    { 5, "555" }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "id", "apellido", "bloqueado", "dni", "esAdmin", "intentosFallidos", "mail", "nombre", "pass" },
                values: new object[,]
                {
                    { 1, "111", false, "11111111", true, 0, "111@111", "111", "111" },
                    { 2, "222", false, "22222222", false, 0, "222@222", "222", "222" },
                    { 3, "333", false, "33333333", false, 0, "333@333", "333", "333" },
                    { 4, "444", false, "44444444", false, 0, "444@444", "444", "444" },
                    { 5, "555", true, "55555555", false, 3, "555@555", "555", "555" }
                });

            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "id", "contenido", "fecha", "idUsuario" },
                values: new object[,]
                {
                    { 1, "111", new DateTime(2022, 8, 2, 0, 38, 44, 16, DateTimeKind.Local).AddTicks(8162), 1 },
                    { 2, "222", new DateTime(2022, 8, 2, 0, 38, 44, 16, DateTimeKind.Local).AddTicks(8175), 1 },
                    { 3, "333", new DateTime(2022, 8, 2, 0, 38, 44, 16, DateTimeKind.Local).AddTicks(8179), 2 },
                    { 4, "444", new DateTime(2022, 8, 2, 0, 38, 44, 16, DateTimeKind.Local).AddTicks(8183), 3 },
                    { 5, "555", new DateTime(2022, 8, 2, 0, 38, 44, 16, DateTimeKind.Local).AddTicks(8184), 4 }
                });

            migrationBuilder.InsertData(
                table: "Usuario_Amigo",
                columns: new[] { "num_usr", "num_usr2" },
                values: new object[,]
                {
                    { 1, 2 },
                    { 1, 3 },
                    { 2, 4 },
                    { 3, 4 },
                    { 4, 2 },
                    { 4, 3 },
                    { 4, 5 },
                    { 5, 4 }
                });

            migrationBuilder.InsertData(
                table: "Comentarios",
                columns: new[] { "id", "contenido", "fecha", "idPost", "idUsuario" },
                values: new object[,]
                {
                    { 1, "111", new DateTime(2022, 8, 2, 0, 38, 44, 16, DateTimeKind.Local).AddTicks(8253), 1, 1 },
                    { 2, "222", new DateTime(2022, 8, 2, 0, 38, 44, 16, DateTimeKind.Local).AddTicks(8258), 1, 5 },
                    { 3, "333", new DateTime(2022, 8, 2, 0, 38, 44, 16, DateTimeKind.Local).AddTicks(8259), 2, 2 },
                    { 4, "444", new DateTime(2022, 8, 2, 0, 38, 44, 16, DateTimeKind.Local).AddTicks(8262), 3, 3 },
                    { 5, "555", new DateTime(2022, 8, 2, 0, 38, 44, 16, DateTimeKind.Local).AddTicks(8264), 4, 4 }
                });

            migrationBuilder.InsertData(
                table: "Reacciones",
                columns: new[] { "id", "idPost", "idUsuario", "tipo" },
                values: new object[,]
                {
                    { 1, 1, 1, 1 },
                    { 2, 1, 5, 1 },
                    { 3, 2, 2, 1 },
                    { 4, 3, 3, 1 },
                    { 5, 4, 4, 1 }
                });

            migrationBuilder.InsertData(
                table: "Tag_Post",
                columns: new[] { "idPost", "idTag" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 2, 3 },
                    { 3, 4 },
                    { 4, 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_idPost",
                table: "Comentarios",
                column: "idPost");

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_idUsuario",
                table: "Comentarios",
                column: "idUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_idUsuario",
                table: "Posts",
                column: "idUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Reacciones_idPost",
                table: "Reacciones",
                column: "idPost");

            migrationBuilder.CreateIndex(
                name: "IX_Reacciones_idUsuario",
                table: "Reacciones",
                column: "idUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_Post_idPost",
                table: "Tag_Post",
                column: "idPost");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_Amigo_num_usr2",
                table: "Usuario_Amigo",
                column: "num_usr2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comentarios");

            migrationBuilder.DropTable(
                name: "Reacciones");

            migrationBuilder.DropTable(
                name: "Tag_Post");

            migrationBuilder.DropTable(
                name: "Usuario_Amigo");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
