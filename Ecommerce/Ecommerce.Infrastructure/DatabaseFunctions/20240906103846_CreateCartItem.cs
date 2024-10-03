using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Infrastructure.DatabaseFunctions
{
    /// <inheritdoc />
    public partial class CreateCartItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION public.create_cart(
                    p_user_id integer,
                    p_product_id integer,
                    p_quantity integer)
                    RETURNS SETOF cart_items
                    LANGUAGE 'plpgsql'
                AS $BODY$
                DECLARE
                    result_row cart_items%ROWTYPE;
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1
                        FROM users
                        WHERE users.user_id = p_user_id
                    ) THEN
                        RAISE EXCEPTION 'Invalid user id';
                    END IF;

                    IF NOT EXISTS (
                        SELECT 1
                        FROM products
                        WHERE products.product_id = p_product_id
                    ) THEN
                        RAISE EXCEPTION 'Invalid product id';
                    END IF;

                    UPDATE cart_items
                    SET quantity = quantity + p_quantity
                    WHERE user_id = p_user_id AND product_id = p_product_id
                    RETURNING * INTO result_row;

                    IF NOT FOUND THEN
                        INSERT INTO cart_items (user_id, product_id, quantity)
                        VALUES (p_user_id, p_product_id, p_quantity)
                        RETURNING * INTO result_row;
                    END IF;

                    RETURN NEXT result_row;
                END;
                $BODY$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP FUNCTION public.create_cart(integer, integer, integer);
            ");
        }
    }
}
