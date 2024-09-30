using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class migrationv33 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "mst_cash_flow",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "mst_cash_flow",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<decimal>(
                name: "balance_after",
                table: "mst_cash_flow",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "balance_before",
                table: "mst_cash_flow",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "type",
                table: "mst_cash_flow",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "balance_after",
                table: "mst_cash_flow");

            migrationBuilder.DropColumn(
                name: "balance_before",
                table: "mst_cash_flow");

            migrationBuilder.DropColumn(
                name: "type",
                table: "mst_cash_flow");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "mst_cash_flow",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "mst_cash_flow",
                newName: "created_at");
        }
    }
}
