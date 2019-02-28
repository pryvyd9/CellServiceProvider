namespace CellServiceProvider.Models
{
    public sealed class Db<T>
    {
        public bool IsAssigned { get; private set; }

        public bool IsNull { get; private set; } = true;

        private T _value;

        private T Value
        {
            get => _value;
            set
            {
                if (value == null)
                {
                    IsNull = true;
                    IsAssigned = true;
                    return;
                }
                else
                {
                    IsNull = false;
                    IsAssigned = true;
                    _value = value;
                    return;
                }

            }
        }

        public static implicit operator T(Db<T> db)
        {
            return db.Value;
        }

        public static implicit operator Db<T>(T db)
        {
            return new Db<T> { Value = db };
        }

        public void Test()
        {
            Db<int> i = 12;
        }

    }
}
