using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CaC2O4.Repositories;

public class Document {
    public Guid Id { get; set; }
    public DocSubject Subject { get; set; }
    public Guid SubjectId { get; set; }
    public String Title { get; set; } = null!;
    public FileState State { get; set; }
    public List<String> Versions { get; set; } = new();
    public String Log { get; set; } = null!;
    public DateTimeOffset Timestamp { get; set; }
}

public class DocumentConfiguration : IEntityTypeConfiguration<Document> {
    public void Configure(EntityTypeBuilder<Document> builder) {
        builder
            .HasIndex(x => new { x.SubjectId, x.Title })
            .IsUnique();
    }
}
