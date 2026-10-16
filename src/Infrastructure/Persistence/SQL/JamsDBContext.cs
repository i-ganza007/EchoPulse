using Microsoft.EntityFrameworkCore;
using Echo.src.Domain.Entities;
namespace Echo.src.Infrastructure.Persistence.SQL
{
    public class JamsDBContext : DbContext
    {
        public DbSet<Jam> Jam { get; set; }
        public DbSet<JamQueueItem> JamQueueItem { get; set; }
        public DbSet<User> User { get; set; }

        public DbSet<JamParticipant> JamParticipant { get; set; }
        public JamsDBContext(DbContextOptions<JamsDBContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Jam>()
                .HasMany(u => u.participants).WithMany(u=>u.jamsJoined);

            modelBuilder.Entity<Jam>()
                .HasOne(p => p.userModel);

            modelBuilder.Entity<Jam>()
                .HasIndex(x => x.jamId)
                .IsUnique();

            modelBuilder.Entity<Jam>()
                .HasMany(x=>x.jamItems).WithOne(x=>x.jamModel);

            //modelBuilder.Entity<JamQueueItem>()
            //    .HasOne(x => x.jamModel);

            modelBuilder.Entity<User>()
                .HasIndex(x=>x.userId).IsUnique();

            modelBuilder.Entity<User>()
                .HasMany(x => x.jamsJoined);


        }
    }
}
