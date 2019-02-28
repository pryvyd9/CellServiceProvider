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
        public long? Id { get; set; }

        [Field("user_id")]
        public long? UserId { get; set; }

        [Default]
        [Field("cost")]
        public decimal? Cost { get; set; }

        [Default]
        [Field("due_date")]
        public System.DateTime? DueDate { get; set; }

        [Default]
        [Field("is_paid")]
        public bool? IsPaid { get; set; }

    }
}
