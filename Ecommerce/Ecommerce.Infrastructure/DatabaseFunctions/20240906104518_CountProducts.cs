using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Infrastructure.DatabaseFunctions
{
    /// <inheritdoc />
    public partial class CountProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION public.count_products(
                    p_parent_category_id integer DEFAULT NULL::integer)
                    RETURNS integer
                    LANGUAGE 'plpgsql'
                    COST 100
                    VOLATILE PARALLEL UNSAFE
                AS $BODY$
                DECLARE
                    product_count integer;
                BEGIN 
                    IF p_parent_category_id IS NULL THEN 
                        SELECT COUNT(*)
                        INTO product_count
                        FROM products;
                    ELSE 
                        WITH RECURSIVE category_tree AS (
                            SELECT 
                                c.category_id
                            FROM categories c
                            WHERE c.category_id = p_parent_category_id
                            UNION ALL
                            SELECT 
                                c.category_id
                            FROM categories c
                            INNER JOIN category_tree ct ON c.parent_category_id = ct.category_id
                        )
                        SELECT COUNT(*)
                        INTO product_count
                        FROM products p
                        WHERE p.category_id IN (SELECT ct.category_id FROM category_tree ct);
                    END IF;
                    
                    RETURN product_count;
                END;
                $BODY$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION public.count_products(integer);");
        }
    }
}
