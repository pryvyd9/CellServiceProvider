using DbFramework;

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
        public Db<int> Id { get; set; }

        [Field("name")]
        public Db<string> Name { get; set; }

        [Default]
        [Field("description")]
        public Db<string> Description { get; set; }

        [Default]
        [Field("cost")]
        public Db<decimal> Cost { get; set; }
    }
}
