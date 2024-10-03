using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Infrastructure.DatabaseFunctions
{
    /// <inheritdoc />
    public partial class GetProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION public.get_products(
                    p_page integer DEFAULT 1,
                    p_page_size integer DEFAULT 10,
                    p_parent_category_id integer DEFAULT NULL::integer)
                    RETURNS TABLE(product_id integer, title character varying, description character varying, price numeric, stock integer, category_id integer) 
                    LANGUAGE 'plpgsql'
                    COST 100
                    VOLATILE PARALLEL UNSAFE
                    ROWS 1000
                AS $BODY$
                BEGIN 
                    IF p_parent_category_id IS NULL THEN 
                        RETURN QUERY
                        SELECT 
                            p.product_id,
                            p.title,
                            p.description,
                            p.price,
                            p.stock,
                            p.category_id
                        FROM products p
                        LIMIT p_page_size OFFSET (p_page - 1) * p_page_size;
                    ELSE 
                        RETURN QUERY WITH RECURSIVE category_tree AS (
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
                        SELECT 
                            p.product_id,
                            p.title,
                            p.description,
                            p.price,
                            p.stock,
                            p.category_id
                        FROM products p
                        WHERE p.category_id IN (SELECT ct.category_id FROM category_tree ct)  -- Уточнение для устранения неоднозначности
                        LIMIT p_page_size OFFSET (p_page - 1) * p_page_size;
                    END IF;
                END;
                $BODY$;            
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION public.get_products(integer, integer, integer)");
        }
    }
}
