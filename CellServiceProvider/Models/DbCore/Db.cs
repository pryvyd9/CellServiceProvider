namespace CellServiceProvider.Models
{
    public interface IDbField
    {
        bool IsAssigned { get; }

        bool IsNull { get; }

        object Value { get; }
    }

    public struct Db<T> : IDbField
    {
        public bool IsAssigned { get; private set; }

        private bool _isNotNull;

        public bool IsNull
        {
            get => !_isNotNull;
            private set => _isNotNull = !value;
        }

        private T _value;

        public T Value
        {
            get => _value;
            private set
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

        object IDbField.Value => Value;
        

        public Db(T value)
        {
            _isNotNull = true;

            IsAssigned = true;

            _value = value;
        }
        
        public static implicit operator T(Db<T> db)
        {
            return db.Value;
        }

        public static implicit operator Db<T>(T db)
        {
            if (db == null)
            {
                return new Db<T>
                {
                    Value = db,
                    IsAssigned = true,
                    IsNull = true,
                };
            }

            return new Db<T>
            {
                Value = db,
                IsAssigned = true
            };
        }

        //public static implicit operator Db<object>(Db<T> db)
        //{
        //    return new Db<object>
        //    {
        //        Value = db,
        //        IsAssigned = db.IsAssigned,
        //        IsNull = db.IsNull
        //    };
        //}

    }
}
