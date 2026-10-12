using Microsoft.EntityFrameworkCore;
using Echo.src.Domain.Entities;
namespace Echo.src.Infrastructure.Persistence.SQL
{
    public class JamsDBContext:DBContext
    {
        public DBSet<Jam> Jam { get; set; }
        public DBSet<JamQueueItem> JamQueueItem { get; set; }
        public DBSet<User> User { get; set; }

        public DBSet<JamParticipant> JamParticipant { get; set; }
        public JamsDBContext(DBContextOptions<JamsDBContext> options) : base(options)
        {
            
        }

        public override void OnModelCreating(ModelBuilder model)
        {

        }
    }
}
