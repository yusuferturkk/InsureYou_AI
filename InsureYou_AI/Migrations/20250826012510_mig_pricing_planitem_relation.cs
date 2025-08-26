using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsureYou_AI.Migrations
{
    /// <inheritdoc />
    public partial class mig_pricing_planitem_relation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PricingPlanItem",
                columns: table => new
                {
                    PricingPlanItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PricingPlanId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricingPlanItem", x => x.PricingPlanItemId);
                    table.ForeignKey(
                        name: "FK_PricingPlanItem_PricingPlans_PricingPlanId",
                        column: x => x.PricingPlanId,
                        principalTable: "PricingPlans",
                        principalColumn: "PricingPlanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PricingPlanItem_PricingPlanId",
                table: "PricingPlanItem",
                column: "PricingPlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PricingPlanItem");
        }
    }
}
