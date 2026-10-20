using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Echo.src.Domain.Enums;
namespace Echo.src.Domain.Entities
{
    public class Jam
    {
        [Key]
        public Guid jamId { get; set; }
        public Guid userId { get; set; }
        public User userModel { get; set; }

        [Length(5, 10)]
        [Required]
        public string title { get; set; }
        [Length(20, 1000)]
        public string? description { get; set; } = null;

        [Required]
        public string h3Index { get; set; }

        public int h3Resolution { get; set; }
        public bool isPublic { get; set; } = true;

        public Guid jamQueueItemId { get; set; }

        public JamQueueItem startingTrack { get; set; } = null;

        public ICollection<JamQueueItem> jamItems { get; set; } = new List<JamQueueItem>();

        public JamStatus status { get; set; } = JamStatus.Active;

        public string? currentTrackId { get; set; } = null;

        public string? currentPositionMs { get; set; }

        public bool isplaying { get; set; } = false;

        public int participantCount { get; set; } = 1;

        public DateTime createdAt { get; set; } = DateTime.Now;

        public DateTime? endedAt { get; set; } = null;

        public DateTime? lastActivityAt { get; set; } = null;

        public ICollection<User> participants { get; set; } = new List<User> {};

        // Used by EF Core when materializing entities
        private Jam() { }

        public Jam(string Title,string H3Index,int H3Resolution,bool IsPublic,int Status,Guid JamQueueItemId)
        {
            title = Title;
            jamQueueItemId = JamQueueItemId;
            h3Index = H3Index;
            h3Resolution = H3Resolution;
            isPublic = IsPublic;
            status = (JamStatus)Status;
        }
        public Jam(string Title, string H3Index, int H3Resolution, bool IsPublic, int Status,string Description,string CurrentTrackId,string CurrentPositionMs,DateTime EndedAt,DateTime LastActivityAt, Guid JamQueueItemId) :this( Title,  H3Index,  H3Resolution,  IsPublic,  Status, JamQueueItemId) {
            description = Description;
            currentTrackId = CurrentTrackId;
            currentPositionMs = CurrentPositionMs;
            endedAt = EndedAt;
            lastActivityAt=LastActivityAt;
        }

       



    }
}
