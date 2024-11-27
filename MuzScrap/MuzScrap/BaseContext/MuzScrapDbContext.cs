using Microsoft.EntityFrameworkCore;
using MuzScrap.Domain.Models;

namespace MuzScrap.BaseContext;

public partial class MuzScrapDbContext : DbContext
{
    public MuzScrapDbContext()
    {
    }

    public MuzScrapDbContext(DbContextOptions<MuzScrapDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Wishlist> Wishlists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MuzScrapBD;Trusted_Connection=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Brand)
                .HasMaxLength(100);

            entity.Property(e => e.Price)
                .HasMaxLength(100);

            entity.Property(e => e.ProductType)
                .HasMaxLength(50);

            entity.Property(e => e.Source)
                .HasMaxLength(500);

            entity.Property(e => e.Store)
                .HasMaxLength(200);

            entity.Property(e => e.Title)
                .HasMaxLength(200);

            entity.Property(e => e.Image)
                .HasMaxLength(500);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Login)
                .HasMaxLength(15);

            entity.Property(e => e.Password)
                .HasMaxLength(15);
        });

        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.HasKey(e => e.Id );

            entity.ToTable("Wishlist");

            entity.HasOne(d => d.Product)
                .WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wishlist_Product");

            entity.HasOne(d => d.User)
                .WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wishlist_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
