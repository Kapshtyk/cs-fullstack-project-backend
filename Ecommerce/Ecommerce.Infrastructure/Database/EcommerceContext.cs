using Microsoft.EntityFrameworkCore;
using Ecommerce.Domain.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Ecommerce.Services.Common.Interfaces;
using Ecommerce.Services.Common.Interceptors;

namespace Ecommerce.Infrastructure.Database
{
    public class EcommerceContext(DbContextOptions<EcommerceContext> options, IFileService fileService) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<SaltUser> SaltUsers { get; set; }
        public DbSet<FrontPage> FrontPages { get; set; }

        private readonly IFileService _fileService = fileService;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>()
                .Property(b => b.Id)
                .HasColumnName("order_id");


            modelBuilder.Entity<Order>()
                .Property(b => b.OrderDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Product>()
                .Property(b => b.Id)
                .HasColumnName("product_id");

            modelBuilder.Entity<Product>().ToTable(t =>
            {
                t.HasCheckConstraint("CK_Product_Price_AboveZero", "\"price\" >= 0.01");
                t.HasCheckConstraint("CK_Product_Stock_NonNegative", "\"stock\" >= 0");
            });

            modelBuilder.Entity<ProductImage>()
                .Property(b => b.Id)
                .HasColumnName("product_image_id");

            modelBuilder.Entity<OrderItem>()
                .Property(b => b.Id)
                .HasColumnName("order_item_id");

            modelBuilder.Entity<OrderItem>().ToTable(t =>
            {
                t.HasCheckConstraint("CK_OrderItem_Quantity_Positive", "\"quantity\" > 0");
                t.HasCheckConstraint("CK_OrderItem_Price_AboveZero", "\"price\" >= 0.01");
            });

            modelBuilder.Entity<CartItem>()
                .Property(b => b.Id)
                .HasColumnName("cart_item_id");

            modelBuilder.Entity<CartItem>().ToTable(t =>
            {
                t.HasCheckConstraint("CK_CartItem_Quantity_Positive", "\"quantity\" > 0");
            });

            modelBuilder.Entity<Review>()
                .Property(b => b.Id)
                .HasColumnName("review_id");

            modelBuilder.Entity<Review>().ToTable(t =>
            {
                t.HasCheckConstraint("CK_Review_Rating_Between1And5", "\"rating\" BETWEEN 1 AND 5");
            });

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId);

            modelBuilder.Entity<User>()
                .Property(b => b.Id)
                .HasColumnName("user_id");

            modelBuilder.Entity<User>().ToTable(t =>
                {
                    t.HasCheckConstraint("CK_User_Email_ValidFormat", "\"email\" ~* '^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$'");
                })
                .Property(u => u.Role)
                .HasConversion(new EnumToStringConverter<Role>());

            modelBuilder.Entity<Category>()
                .Property(b => b.Id)
                .HasColumnName("category_id");

            modelBuilder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Category>().ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Category_Parent_Category_Id_Not_Equal_To_Id", "\"category_id\" <> \"parent_category_id\"");
                });

            modelBuilder.Entity<SaltUser>()
                .Property(b => b.Id)
                .HasColumnName("salt_user_id");

            modelBuilder.Entity<User>()
                .HasOne(u => u.SaltUser)
                .WithOne(su => su.User)
                .HasForeignKey<SaltUser>(su => su.UserId);

            modelBuilder.Entity<FrontPage>()
                .Property(b => b.Id)
                .HasColumnName("front_page_id");

            modelBuilder.Entity<FrontPage>()
                .HasOne(fp => fp.SelectedProduct)
                .WithMany(p => p.FrontPages)
                .HasForeignKey(fp => fp.SelectedProductId);

            modelBuilder.Entity<FrontPage>().ToTable(t =>
                {
                    t.HasCheckConstraint("CK_FrontPage_HeroBanner_NotEmpty", "LENGTH(\"hero_banner\") > 0");
                    t.HasCheckConstraint("CK_FrontPage_HeroBannerText_NotEmpty", "LENGTH(\"hero_banner_text\") > 0");
                });

            modelBuilder.Entity<FrontPage>()
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<FrontPage>()
                .Property(b => b.IsPublished)
                .HasDefaultValue(false);

            modelBuilder.HasDbFunction(typeof(EcommerceContext).GetMethod(nameof(GetCategories), [typeof(int), typeof(int), typeof(int)]) ?? throw new NotImplementedException("GetCategories is not implemented")).HasName("get_categories");
            modelBuilder.HasDbFunction(typeof(EcommerceContext).GetMethod(nameof(GetProducts), [typeof(int), typeof(int), typeof(int)]) ?? throw new NotImplementedException("GetProducts is not implemented")).HasName("get_products");
            modelBuilder.HasDbFunction(typeof(EcommerceContext).GetMethod(nameof(CreateCartItem), [typeof(int), typeof(int), typeof(int)]) ?? throw new NotImplementedException("CreateCartItem is not implemented")).HasName("create_cart");
            modelBuilder.HasDbFunction(typeof(EcommerceContext).GetMethod(nameof(CreateOrderFromCart), [typeof(int)]) ?? throw new NotImplementedException("CreateOrderFromCart is not implemented")).HasName("create_order_from_cart");
            modelBuilder.HasDbFunction(typeof(EcommerceContext).GetMethod(nameof(GetTopProducts), [typeof(int)]) ?? throw new NotImplementedException("GetTopProducts is not implemented")).HasName("get_top_products");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.AddInterceptors(new ImagesSavingInterceptor(_fileService));
        }

        public IQueryable<Category> GetCategories(int? page, int? pageSize, int? parentCategoryId = null) => FromExpression(() => GetCategories(page, pageSize, parentCategoryId));
        public IQueryable<Product> GetProducts(int? page, int? pageSize, int? categoryId = null) => FromExpression(() => GetProducts(page, pageSize, categoryId));
        public IQueryable<CartItem> CreateCartItem(int userId, int productId, int quantity) => FromExpression(() => CreateCartItem(userId, productId, quantity));
        public IQueryable<Order> CreateOrderFromCart(int userId) => FromExpression(() => CreateOrderFromCart(userId));
        public IQueryable<Product> GetTopProducts(int numberOfProducts) => FromExpression(() => GetTopProducts(numberOfProducts));
    }
}
