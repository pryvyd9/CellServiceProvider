namespace CellServiceProvider.Models
{
    [Table("users_to_services")]
    public class UserToService : Entity
    {
        public UserToService(DbContext context) : base(context)
        {
        }

        [Key("user_id")]
        public long? UserId { get; set; }

        [Key("service_id")]
        public long? ServiceId { get; set; }
    }
}
