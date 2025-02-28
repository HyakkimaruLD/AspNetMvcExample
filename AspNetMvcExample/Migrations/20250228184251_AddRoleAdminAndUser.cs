using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspNetMvcExample.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleAdminAndUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(table:"AspNetRoles",
                columns:["Name", "NormalizedName"],
                new object[] { "Admin", "ADMIN" });

            migrationBuilder.InsertData(table: "AspNetRoles",
                columns: ["Name", "NormalizedName"],
                new object[] { "User", "USER" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
