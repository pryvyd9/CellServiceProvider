using DbFramework;

namespace CellServiceProvider.Models
{
    [Table("user_groups")]
    public sealed class UserGroup : Entity
    {
        public UserGroup(DbContext context) : base(context)
        {
        }

        [Key("id")]
        public Db<int> Id { get; set; }

        [Field("name")]
        public Db<string> Name { get; set; }
    }
}
