using Microsoft.EntityFrameworkCore;

namespace ApiMongoDB.Infrastructure.Persistence.MsSql.EF.DataModel;

public partial class PcsListsContext : DbContext
{
    public PcsListsContext()
    {
    }

    public PcsListsContext(DbContextOptions<PcsListsContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=BUILDPCS-DATABASE.portpcs.com;Database=PcsRailway;user id=developer;password=developer;Integrated Security=false;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
