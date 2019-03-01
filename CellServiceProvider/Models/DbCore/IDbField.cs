namespace CellServiceProvider.Models
{
    public interface IDbField
    {
        bool IsAssigned { get; }

        bool IsNull { get; }

        object Value { get; }
    }
}
