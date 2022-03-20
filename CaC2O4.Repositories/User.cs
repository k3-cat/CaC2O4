using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CaC2O4.Repositories;

public class User {
    public Guid Id { get; set; }
    public String Name { get; set; } = null!;
    public String Phone { get; set; } = null!;
    public String Password { get; set; } = null!;
    public UserAcl Acl { get; set; }
    public DateTimeOffset LastAccess { get; set; }
}

public class UserConfiguration : IEntityTypeConfiguration<User> {
    public void Configure(EntityTypeBuilder<User> builder) {
        builder.HasIndex(x => x.Name).IsUnique();

        builder
            .Property(x => x.Id)
            .HasDefaultValueSql("gen_random_uuid()");
    }
}
