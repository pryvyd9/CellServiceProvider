namespace DbFramework
{

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

        public override string ToString()
        {
            return IsNull ? string.Empty : Value.ToString();
        }

    }
}
