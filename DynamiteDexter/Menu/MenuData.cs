namespace DynamiteDexter
{
    public abstract class Data
    {
        private readonly string _name;
        public string Name { get { return _name; } }
        public Data(string name) { _name = name; }
    }

    public class DataInt : Data
    {
        public int Value { get; set; }
        public DataInt(string name, int value)
            : base(name)
        { Value = value; }
    }

    public class DataDouble : Data
    {
        public double Value { get; set; }
        public DataDouble(string name, double value)
            : base(name)
        { Value = value; }
    }

    public class DataBool : Data
    {
        public bool Value { get; set; }
        public DataBool(string name, bool value)
            : base(name)
        { Value = value; }
    }

    public class DataString : Data
    {
        public string Value { get; set; }
        public DataString(string name, string value)
            : base(name)
        { Value = value; }
    }

    public class DataNull : Data
    {
        public string Value { get { return null; } }
        public DataNull() : base(null) { }
    }
}

