namespace CellServiceProvider.Models
{
    [Table("services")]
    public class Service : Entity
    {
        public Service(DbContext context) : base(context)
        {
        }

        [Default]
        [Key("id")]
        public long? Id { get; set; }

        [Field("name")]
        public string Name { get; set; }

        [Field("description")]
        public string Description { get; set; }

        [Default]
        [Field("cost")]
        public decimal? Cost { get; set; }
    }
}
