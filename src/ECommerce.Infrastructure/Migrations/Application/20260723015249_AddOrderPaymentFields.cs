using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Migrations.Application
{
    /// <inheritdoc />
    public partial class AddOrderPaymentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PaidAtUtc",
                table: "Orders",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentIntentId",
                table: "Orders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PaymentIntentId",
                table: "Orders",
                column: "PaymentIntentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_PaymentIntentId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaidAtUtc",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentIntentId",
                table: "Orders");
        }
    }
}
