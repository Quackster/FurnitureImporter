using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public class ItemDefinition
{
    public int Id { get; set; }
    public string? Sprite { get; set; }
    public int SpriteId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Colour { get; set; } = "0,0,0";
    public int Length { get; set; } = 0;
    public int Width { get; set; } = 0;
    public double TopHeight { get; set; } = 0;
    public string MaxStatus { get; set; } = "0";
    public string Behaviour { get; set; } = string.Empty;
    public string Interactor { get; set; } = "default";
    public bool IsTradable { get; set; } = true;
    public bool IsRecyclable { get; set; } = true;
    public string DrinkIds { get; set; } = string.Empty;
}

public class ItemDefinitionConfiguration : IEntityTypeConfiguration<ItemDefinition>
{
    public void Configure(EntityTypeBuilder<ItemDefinition> builder)
    {
        builder.ToTable("items_definitions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Sprite).HasColumnName("sprite").HasMaxLength(50);
        builder.Property(x => x.SpriteId).HasColumnName("sprite_id");
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(255).IsRequired();
        builder.Property(x => x.Colour).HasColumnName("colour").HasMaxLength(100);
        builder.Property(x => x.Length).HasColumnName("length");
        builder.Property(x => x.Width).HasColumnName("width");
        builder.Property(x => x.TopHeight).HasColumnName("top_height");
        builder.Property(x => x.MaxStatus).HasColumnName("max_status").HasMaxLength(11);
        builder.Property(x => x.Behaviour).HasColumnName("behaviour").HasMaxLength(150);
        builder.Property(x => x.Interactor).HasColumnName("interactor").HasMaxLength(150);
        builder.Property(x => x.IsTradable).HasColumnName("is_tradable");
        builder.Property(x => x.IsRecyclable).HasColumnName("is_recyclable");
        builder.Property(x => x.DrinkIds).HasColumnName("drink_ids");
    }
}
