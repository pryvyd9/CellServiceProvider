using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CellServiceProvider.Models
{
    [Table("users")]
    public class User : Entity
    {
        public User(DbContext context) : base(context)
        {
        }

        [Default]
        [Key("id")]
        public Db<long> Id { get; set; }

        [Field("nickname")]
        public Db<string> NickName { get; set; }

        [Nullable]
        [Field("full_name")]
        public Db<string> FullName { get; set; }

        [Field("group_id")]
        public Db<long> GroupId { get; set; }

        [Default]
        [Field("is_active")]
        public Db<bool> IsActive { get; set; }

        [Field("password")]
        public Db<string> Password { get; set; }
    }
}
