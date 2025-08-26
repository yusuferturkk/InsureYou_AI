using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsureYou_AI.Migrations
{
    /// <inheritdoc />
    public partial class mig_edit_pricingplan_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "PricingPlans");

            migrationBuilder.DropColumn(
                name: "PlanName",
                table: "PricingPlans");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "PricingPlans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PlanName",
                table: "PricingPlans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
