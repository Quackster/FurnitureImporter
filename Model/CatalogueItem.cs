using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public class CatalogueItem
{
    public int Id { get; set; }
    public string? SaleCode { get; set; }
    public string? PageId { get; set; }
    public int OrderId { get; set; } = 0;
    public int Price { get; set; } = 3;
    public bool IsHidden { get; set; } = false;
    public int Amount { get; set; } = 1;
    public int? DefinitionId { get; set; }
    public int ItemSpecialSpriteId { get; set; } = 0;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool IsPackage { get; set; } = false;
    public string? PackageName { get; set; }
    public string? PackageDescription { get; set; }
}

public class CatalogueItemConfiguration : IEntityTypeConfiguration<CatalogueItem>
{
    public void Configure(EntityTypeBuilder<CatalogueItem> builder)
    {
        builder.ToTable("catalogue_items");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.SaleCode).HasColumnName("sale_code").HasMaxLength(255);
        builder.Property(x => x.PageId).HasColumnName("page_id");
        builder.Property(x => x.OrderId).HasColumnName("order_id");
        builder.Property(x => x.Price).HasColumnName("price");
        builder.Property(x => x.IsHidden).HasColumnName("is_hidden");
        builder.Property(x => x.Amount).HasColumnName("amount");
        builder.Property(x => x.DefinitionId).HasColumnName("definition_id");
        builder.Property(x => x.ItemSpecialSpriteId).HasColumnName("item_specialspriteid");
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(255).IsRequired();
        builder.Property(x => x.IsPackage).HasColumnName("is_package");
        builder.Property(x => x.PackageName).HasColumnName("package_name").HasMaxLength(255);
        builder.Property(x => x.PackageDescription).HasColumnName("package_description").HasMaxLength(256);
    }
}
