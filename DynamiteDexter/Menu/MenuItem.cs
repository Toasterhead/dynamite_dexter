namespace DynamiteDexter
{
    public abstract class MenuItem
    {
        protected Data[] data;

        public MenuItem(Data[] data)
        { this.data = data; }

        public MenuItem(Data data)
        {
            this.data = new Data[1];
            this.data[0] = data;
        }
    }

    public interface ISelectable
    {
        bool Muted { get; set; }
        bool Invisible { get; set; }
        string Message { get; set; }
    }

    public interface IDirectional
    {
        void Forward();
        void Backward();
    }

    public class MIHeadline : MenuItem
    {
        public string Text { get; set; }

        public MIHeadline(string text)
            : base(new DataNull())
        { Text = text; }

        public override string ToString() { return Text; }
    }

    public class MISpace : MenuItem
    {
        public MISpace() : base(new DataNull()) { }

        public override string ToString() { return ""; }
    }

    public class MILink : MenuItem, ISelectable
    {
        public readonly string Text;
        public readonly SubMenu TheSubMenu;

        public bool Muted { get; set; }
        public bool Invisible { get; set; }
        public string Message { get; set; }

        public MILink(string text, SubMenu subMenu)
            : base(new DataNull())
        {
            Text = text;
            TheSubMenu = subMenu;
        }

        public override string ToString() { return Text; }
    }

    public class MISwitch : MenuItem, ISelectable, IDirectional
    {
        protected const int DESCRIPTION = 0;
        protected const int SETTING_ONE = 1;
        protected const int SETTING_TWO = 2;

        protected readonly string[] _text;

        protected bool Toggle
        {
            get
            {
                if (data[0] is DataBool)
                    return (data[0] as DataBool).Value;
                else throw new System.Exception("Error - Data must be of type DataBool.");
            }
            set
            {
                if (data[0] is DataBool)
                    (data[0] as DataBool).Value = value;
                else throw new System.Exception("Error - data must be a DataBool type.");
            }
        }

        public bool Muted { get; set; }
        public bool Invisible { get; set; }
        public string Message { get; set; }

        public string Description { get { return _text[DESCRIPTION]; } }
        public string SelectedSetting { get { return Toggle ? _text[1] : _text[2]; } }

        public MISwitch(DataBool data, string description, string settingOne, string settingTwo)
            : base(data)
        {
            _text = new string[3];
            _text[DESCRIPTION] = description;
            _text[SETTING_ONE] = settingOne;
            _text[SETTING_TWO] = settingTwo;
        }

        public void Flip() { Toggle = !Toggle; }

        public void Forward() { Flip(); }
        public void Backward() { Flip(); }

        public override string ToString()
        {
            if (Toggle)
                return _text[0] + _text[1];
            return _text[0] + _text[2];
        }
    }

    public class MIDial : MenuItem, ISelectable, IDirectional
    {
        public readonly string Description;

        protected readonly string[] _options;

        protected DataInt DataAsInt
        {
            get
            {
                if (!(data[0] is DataInt))
                    throw new System.Exception("Error - Data must of type DataInt.");

                return (data[0] as DataInt);
            }
        }

        public bool Muted { get; set; }
        public bool Invisible { get; set; }
        public string Message { get; set; }

        public string SelectedOption { get { return _options[DataAsInt.Value]; } }

        public MIDial(DataInt data, string description, string[] options)
            : base(data)
        {
            Description = description;
            _options = options;

            if (DataAsInt.Value >= options.Length || DataAsInt.Value < 0)
                DataAsInt.Value = 0;
        }

        public void CycleClockwise()
        {
            if (++DataAsInt.Value >= _options.Length) DataAsInt.Value = 0;
        }

        public void CycleCounterClockwise()
        {
            if (--DataAsInt.Value < 0) DataAsInt.Value = _options.Length - 1;
        }

        public void Forward() { CycleClockwise(); }
        public void Backward() { CycleCounterClockwise(); }

        public override string ToString() { return Description + _options[DataAsInt.Value]; }
    }

    public class MISlider : MenuItem, ISelectable, IDirectional
    {
        public readonly string Description;

        protected readonly bool _reportAsRatio;
        protected readonly uint _max;
        protected readonly uint _standardInterval;

        protected DataDouble DataAsDouble
        {
            get
            {
                if (!(data[0] is DataDouble))
                    throw new System.Exception("Error - Data must of type DataInt.");

                return (data[0] as DataDouble);
            }
        }

        public bool Muted { get; set; }
        public bool Invisible { get; set; }
        public string Message { get; set; }

        public int ValueAsInt { get { return (int)(DataAsDouble.Value * _max); } }
        public double ValueAsDouble { get { return DataAsDouble.Value * _max; } }

        public MISlider(DataDouble data, string description, uint max, uint standardInterval, bool reportAsRatio = true)
            : base(data)
        {
            Description = description;
            _reportAsRatio = reportAsRatio;
            _max = max;
            _standardInterval = standardInterval;

            if (DataAsDouble.Value < 0.0 || DataAsDouble.Value > 1.0)
                DataAsDouble.Value = 0.0;
        }

        public void Increase(double amount)
        {
            double ratio = amount / _max;

            DataAsDouble.Value += ratio;

            if (DataAsDouble.Value > 1.0) DataAsDouble.Value = 1.0;
        }

        public void Decrease(double amount)
        {
            double ratio = amount / _max;

            DataAsDouble.Value -= ratio;

            //To prevent negative values and very small roundings above zero.
            if (DataAsDouble.Value < 0.001) DataAsDouble.Value = 0.0;
        }

        public void Increase() { Increase(_standardInterval); }

        public void Decrease() { Decrease(_standardInterval); }

        public void Forward() { Increase(); }
        public void Backward() { Decrease(); }

        public override string ToString()
        {
            if (!_reportAsRatio)

                return Description + (DataAsDouble.Value * _max);

            return Description + DataAsDouble.Value;
        }
    }

    public class MICommand : MenuItem, ISelectable
    {
        public delegate void Command(Data[] data);

        protected Command handler;

        public bool Muted { get; set; }
        public bool Invisible { get; set; }
        public string Message { get; set; }

        public string Description { get; set; }

        public MICommand(string description, Command handler, Data[] data) : base(data)
        { SubInit(description, handler); }
        public MICommand(string description, Command handler, Data data) : base(data)
        { SubInit(description, handler); }
        public MICommand(string description, Command handler) : base(new DataNull())
        { SubInit(description, handler); }

        private void SubInit(string description, Command handler)
        {
            Description = description;
            this.handler = handler;
        }

        public void Execute() { handler(data); }

        public override string ToString() { return Description; }
    }

    public class MICommandLink : MICommand //Needs testing.
    {
        public readonly SubMenu TheSubMenu;

        public MICommandLink(string description, Command handler, SubMenu subMenu, Data[] data)
            : base(description, handler, data)
        { TheSubMenu = subMenu; }
        public MICommandLink(string description, Command handler, SubMenu subMenu, Data data)
            : base(description, handler, data)
        { TheSubMenu = subMenu; }
        public MICommandLink(string description, Command handler, SubMenu subMenu)
            : base(description, handler)
        { TheSubMenu = subMenu; }
    }
}
