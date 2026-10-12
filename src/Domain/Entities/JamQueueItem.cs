using System.ComponentModel.DataAnnotations;

namespace Echo.src.Domain.Entities
{
    public class JamQueueItem
    {
        [Key]
        public Guid jamQueueItemid { get; set; } 
        public Guid jamId { get; set; }
        public Jam jamModel { get; set; }
        public string trackId { get; set; }

        public JamQueueItem NextItem { get; set; } = null;

        public JamQueueItem PreviousItem { get; set; } = null;

        public string? trackTitle { get; set; }
        public string? trackArtist { get; set; }
        public Guid addedByUserId { get; set; }
        
        public User addingUser { get; set; }
        public int position { get; set; }
        public DateTime addedAt { get; set; } = DateTime.Now;
        public bool isPlayed { get; set; } = false;

        public JamQueueItem(Guid JamQueueItemid,Guid JamId,string TrackId,Guid AddedByUserId,int Position,bool IsPlayed)
        {
            jamQueueItemid = JamQueueItemid;
            jamId = JamId;
            trackId = TrackId;
            addedByUserId = AddedByUserId;
            position = Position;
            isPlayed = IsPlayed;
        }

        public JamQueueItem(Guid JamQueueItemid, Guid JamId, string TrackId, Guid AddedByUserId, int Position, bool IsPlayed,string TrackTitle,string TrackArtist):this(JamQueueItemid, JamId,TrackId,AddedByUserId,Position,IsPlayed)
        {
            trackTitle = TrackTitle;
            trackArtist = TrackArtist;
        }



    }
}
