using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Migrations.Application
{
    /// <inheritdoc />
    public partial class RefineDeliveryMethodAndProductItemOrdered : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DeliveryMethods_SupportedCountry_SupportedCity",
                table: "DeliveryMethods");

            migrationBuilder.DropColumn(
                name: "SupportedCity",
                table: "DeliveryMethods");

            migrationBuilder.DropColumn(
                name: "SupportedCountry",
                table: "DeliveryMethods");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SupportedCity",
                table: "DeliveryMethods",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SupportedCountry",
                table: "DeliveryMethods",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryMethods_SupportedCountry_SupportedCity",
                table: "DeliveryMethods",
                columns: new[] { "SupportedCountry", "SupportedCity" });
        }
    }
}
