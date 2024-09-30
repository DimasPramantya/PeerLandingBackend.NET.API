using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class migrationv30 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "trn_funding");

            migrationBuilder.AlterColumn<string>(
                name: "borrower_id",
                table: "mst_loans",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTime>(
                name: "funded_at",
                table: "mst_loans",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "lender_id",
                table: "mst_loans",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "total_amount",
                table: "mst_loans",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "total_repaid",
                table: "mst_loans",
                type: "numeric",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "mst_cash_flow",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mst_cash_flow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mst_cash_flow_mst_user_user_id",
                        column: x => x.user_id,
                        principalTable: "mst_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mst_loans_lender_id",
                table: "mst_loans",
                column: "lender_id");

            migrationBuilder.CreateIndex(
                name: "IX_mst_cash_flow_user_id",
                table: "mst_cash_flow",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_mst_loans_mst_user_lender_id",
                table: "mst_loans",
                column: "lender_id",
                principalTable: "mst_user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mst_loans_mst_user_lender_id",
                table: "mst_loans");

            migrationBuilder.DropTable(
                name: "mst_cash_flow");

            migrationBuilder.DropIndex(
                name: "IX_mst_loans_lender_id",
                table: "mst_loans");

            migrationBuilder.DropColumn(
                name: "funded_at",
                table: "mst_loans");

            migrationBuilder.DropColumn(
                name: "lender_id",
                table: "mst_loans");

            migrationBuilder.DropColumn(
                name: "total_amount",
                table: "mst_loans");

            migrationBuilder.DropColumn(
                name: "total_repaid",
                table: "mst_loans");

            migrationBuilder.AlterColumn<string>(
                name: "borrower_id",
                table: "mst_loans",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "trn_funding",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    LenderId = table.Column<string>(type: "text", nullable: false),
                    LoanId = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    FundedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trn_funding", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trn_funding_mst_loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "mst_loans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trn_funding_mst_user_LenderId",
                        column: x => x.LenderId,
                        principalTable: "mst_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_trn_funding_LenderId",
                table: "trn_funding",
                column: "LenderId");

            migrationBuilder.CreateIndex(
                name: "IX_trn_funding_LoanId",
                table: "trn_funding",
                column: "LoanId",
                unique: true);
        }
    }
}
