namespace CellServiceProvider.Models
{
    [Table("user_groups")]
    public sealed class UserGroup : Entity
    {
        public UserGroup(DbContext context) : base(context)
        {
        }

        [Key("id")]
        public long Id { get; set; }

        [Field("name")]
        public string Name { get; set; }
    }
}
