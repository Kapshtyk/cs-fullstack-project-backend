using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Ecommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFrontpage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "front_pages",
                columns: table => new
                {
                    front_page_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    hero_banner = table.Column<string>(type: "varchar(256)", nullable: false),
                    hero_banner_text = table.Column<string>(type: "varchar(256)", nullable: false),
                    selected_product_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    is_published = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_front_pages", x => x.front_page_id);
                    table.CheckConstraint("CK_FrontPage_HeroBanner_NotEmpty", "LENGTH(\"hero_banner\") > 0");
                    table.CheckConstraint("CK_FrontPage_HeroBannerText_NotEmpty", "LENGTH(\"hero_banner_text\") > 0");
                    table.ForeignKey(
                        name: "fk_front_pages_products_selected_product_id",
                        column: x => x.selected_product_id,
                        principalTable: "products",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_front_pages_selected_product_id",
                table: "front_pages",
                column: "selected_product_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "front_pages");
        }
    }
}
