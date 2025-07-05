using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public class CataloguePage
{
    public int Id { get; set; }
    public int? OrderId { get; set; }
    public int? MinRole { get; set; }
    public bool IndexVisible { get; set; } = true;
    public bool IsClubOnly { get; set; } = false;
    public string? NameIndex { get; set; }
    public string LinkList { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Layout { get; set; }
    public string? ImageHeadline { get; set; }
    public string? ImageTeasers { get; set; }
    public string Body { get; set; } = string.Empty;
    public string? LabelPick { get; set; }
    public string? LabelExtraS { get; set; }
    public string? LabelExtraT { get; set; }
}

public class CataloguePageConfiguration : IEntityTypeConfiguration<CataloguePage>
{
    public void Configure(EntityTypeBuilder<CataloguePage> builder)
    {
        builder.ToTable("catalogue_pages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.OrderId).HasColumnName("order_id");
        builder.Property(x => x.MinRole).HasColumnName("min_role");
        builder.Property(x => x.IndexVisible).HasColumnName("index_visible");
        builder.Property(x => x.IsClubOnly).HasColumnName("is_club_only");
        builder.Property(x => x.NameIndex).HasColumnName("name_index").HasMaxLength(255);
        builder.Property(x => x.LinkList).HasColumnName("link_list").HasMaxLength(255).IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(255);
        builder.Property(x => x.Layout).HasColumnName("layout").HasMaxLength(255);
        builder.Property(x => x.ImageHeadline).HasColumnName("image_headline").HasMaxLength(255);
        builder.Property(x => x.ImageTeasers).HasColumnName("image_teasers").HasMaxLength(255);
        builder.Property(x => x.Body).HasColumnName("body");
        builder.Property(x => x.LabelPick).HasColumnName("label_pick").HasMaxLength(255);
        builder.Property(x => x.LabelExtraS).HasColumnName("label_extra_s").HasMaxLength(255);
        builder.Property(x => x.LabelExtraT).HasColumnName("label_extra_t");
    }
}
