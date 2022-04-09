namespace DynamiteDexter
{
    public class Prompt
    {
        public readonly string Description;

        protected readonly string _defaultInput;

        protected string currentInput;
        protected DataString entry;

        public string CurrentInput { get { return currentInput; } }
        public string EntryName { get { return entry.Name; } }

        public Prompt(DataString entry, string defaultInput = "", string description = "")
        {
            this.entry = entry;
            _defaultInput = defaultInput;
            currentInput = defaultInput;
            Description = description;
        }

        public void Reset() { currentInput = _defaultInput; }

        public void AppendInput(char ch) { currentInput += ch; }

        public void Backspace() { if (currentInput.Length > 0) currentInput = currentInput.Substring(0, currentInput.Length - 1); }

        public void Confirm() { entry.Value = currentInput; }

        public override string ToString() { throw new System.Exception("Test."); }//{ return Description + currentInput; }
    }
}
