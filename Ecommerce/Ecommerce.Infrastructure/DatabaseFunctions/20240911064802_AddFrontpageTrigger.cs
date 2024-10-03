using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Infrastructure.DatabaseFunctions
{
    /// <inheritdoc />
    public partial class AddFrontpageTrigger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION set_other_front_pages_to_unpublished()
                RETURNS TRIGGER AS $$
                BEGIN
                    IF NEW.is_published = TRUE THEN
                        UPDATE front_pages
                        SET is_published = FALSE
                        WHERE front_page_id <> NEW.front_page_id;
                    END IF;
                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;

                CREATE OR REPLACE TRIGGER set_other_front_pages_to_unpublished
                AFTER INSERT OR UPDATE ON front_pages
                FOR EACH ROW
                EXECUTE FUNCTION set_other_front_pages_to_unpublished();
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP TRIGGER set_other_front_pages_to_unpublished ON front_pages;
                DROP FUNCTION set_other_front_pages_to_unpublished;
            ");
        }
    }
}
