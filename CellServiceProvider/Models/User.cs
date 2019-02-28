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
        public string NickName { get; set; }

        [Nullable]
        [Field("full_name")]
        public string FullName { get; set; }

        [Field("group_id")]
        public long GroupId { get; set; }

        [Field("is_active")]
        public bool IsActive { get; set; }

        [Field("password")]
        public string Password { get; set; }
    }
}
