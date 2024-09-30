using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class migrationv37 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "balance_after",
                table: "mst_cash_flow");

            migrationBuilder.DropColumn(
                name: "balance_before",
                table: "mst_cash_flow");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
