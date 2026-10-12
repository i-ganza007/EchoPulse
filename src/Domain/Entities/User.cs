using System.ComponentModel.DataAnnotations;
namespace Echo.src.Domain.Entities
{
    public class User
    {
        [Key]
        public Guid userId { get; set; }
        [Required]
        [Length(5,50)]
        public string displayName { get; set; }

        [EmailAddress]
        public string email { get; set; }
        public DateTime createdAt { get; set; } = DateTime.Now;

        public DateTime? lastSeenAt { get; set; }

        public bool isBanned { get; set; } = false;

        public Guid avatarUrl { get; set; }

        public ICollection<Jam> jamsJoined { get; set; } = new List<Jam>() { };



        // Used by EF Core when materializing entities
        private User() { }

        public User(Guid UserId ,string DisplayName,string Email,Guid AvatarUrl,bool IsBanned,DateTime? LastSeenAt = null) 
        {
            displayName = DisplayName;
            email = Email;
            userId = UserId;
            avatarUrl = AvatarUrl;
            isBanned = IsBanned;
            lastSeenAt = LastSeenAt;
        }
    }
}
