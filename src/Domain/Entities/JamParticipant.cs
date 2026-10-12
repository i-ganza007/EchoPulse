using System.ComponentModel.DataAnnotations;

namespace Echo.src.Domain.Entities
{
    public class JamParticipant
    {
        [Key]
        public Guid jamParticipantId { get; set; }

        public Guid userId { get; set; }

        public User userModel { get; set; }

        public Guid jamId { get; set; }
        public Jam jamModel { get; set; }
        public DateTime joinedAt { get; set; } = DateTime.Now;
        public DateTime? leftAt { get; set; } = null;
        public bool isHost { get; set; } = false;

        public JamParticipant(Guid JamParticipantId,Guid UserId, Guid JamId,bool IsHost) {
            jamParticipantId = JamParticipantId;
            userId = UserId;
            jamId = JamId;
            isHost = IsHost;
        }
    }
}
