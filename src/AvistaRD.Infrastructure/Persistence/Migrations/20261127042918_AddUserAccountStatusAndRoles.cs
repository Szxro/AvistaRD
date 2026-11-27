using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AvistaRD.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAccountStatusAndRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.AddColumn<string>(
                name: "full_name",
                table: "AspNetUsers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "AspNetUsers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Pending");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "concurrency_stamp", "name", "normalized_name" },
                values: new object[,]
                {
                    { "495db6b9-9e46-46a6-8eb2-902af489b3c2", "495db6b9-9e46-46a6-8eb2-902af489b3c2", "Ciudadano", "CIUDADANO" },
                    { "6fac0b57-765d-4bc7-a257-b800e8d13c35", "6fac0b57-765d-4bc7-a257-b800e8d13c35", "Guardaparques", "GUARDAPARQUES" },
                    { "9e1d9be6-3fad-4dc6-9d03-f37d001c63b6", "9e1d9be6-3fad-4dc6-9d03-f37d001c63b6", "Validador", "VALIDADOR" },
                    { "eedda775-1352-41e7-9574-caf858748797", "eedda775-1352-41e7-9574-caf858748797", "Administrador", "ADMINISTRADOR" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_users_status",
                table: "AspNetUsers",
                column: "status");

            // Accounts created before this migration could already sign in: they stay active, and the ones
            // without a role become Ciudadano, the role that doesn't need approval
            migrationBuilder.Sql("UPDATE [AspNetUsers] SET [status] = N'Active';");

            migrationBuilder.Sql(
                """
                INSERT INTO [AspNetUserRoles] ([user_id], [role_id])
                SELECT [u].[id], N'495db6b9-9e46-46a6-8eb2-902af489b3c2'
                FROM [AspNetUsers] AS [u]
                WHERE NOT EXISTS (SELECT 1 FROM [AspNetUserRoles] AS [ur] WHERE [ur].[user_id] = [u].[id]);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_asp_net_users_status",
                table: "AspNetUsers");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "495db6b9-9e46-46a6-8eb2-902af489b3c2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "6fac0b57-765d-4bc7-a257-b800e8d13c35");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "9e1d9be6-3fad-4dc6-9d03-f37d001c63b6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "eedda775-1352-41e7-9574-caf858748797");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "full_name",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "status",
                table: "AspNetUsers");
        }
    }
}
