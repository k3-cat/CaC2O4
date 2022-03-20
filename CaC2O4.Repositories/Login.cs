using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CaC2O4.Repositories;

public class Login {
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset IssueAt { get; set; }
}

public class LoginConfiguration : IEntityTypeConfiguration<Login> {
    public void Configure(EntityTypeBuilder<Login> builder) {
        builder
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.ClientCascade);
    }
}
