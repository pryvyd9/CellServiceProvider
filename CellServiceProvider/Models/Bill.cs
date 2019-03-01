using System;

namespace CellServiceProvider.Models
{
    [Table("bills")]
    public class Bill : Entity
    {
        public Bill(DbContext context) : base(context)
        {
        }

        [Default]
        [Key("id")]
        public Db<int> Id { get; set; }

        [Field("user_id")]
        public Db<int> UserId { get; set; }

        [Default]
        [Field("cost")]
        public Db<decimal> Cost { get; set; }

        [Default]
        [Field("due_date")]
        public Db<DateTime> DueDate { get; set; }

        [Default]
        [Field("is_paid")]
        public Db<bool> IsPaid { get; set; }

    }
}
