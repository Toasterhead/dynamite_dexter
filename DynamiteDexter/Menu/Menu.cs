using System;
using System.Collections.Generic;

namespace DynamiteDexter
{
    public class Menu
    {
        public readonly string Title;

        protected List<SubMenu> subMenus;
        protected List<DataInt> dataIntegers;
        protected List<DataDouble> dataDoubles;
        protected List<DataBool> dataBooleans;
        protected List<DataString> dataStrings;
        protected List<Prompt> prompts;

        public SubMenu CurrentSubMenu { get; set; }
        public int CurrentSubMenuLength { get { return CurrentSubMenu.Length; } }

        public Menu(
            List<SubMenu> subMenus,
            List<DataBool> dataBooleans = null,
            List<DataInt> dataIntegers = null,
            List<DataDouble> dataDoubles = null,
            List<DataString> dataStrings = null,
            List<Prompt> prompts = null,
            string title = null)
        {
            this.subMenus = subMenus;
            this.dataBooleans = dataBooleans == null ? new List<DataBool>() : dataBooleans;
            this.dataIntegers = dataIntegers == null ? new List<DataInt>() : dataIntegers;
            this.dataDoubles = dataDoubles == null ? new List<DataDouble>() : dataDoubles;
            this.dataStrings = dataStrings == null ? new List<DataString>() : dataStrings;
            this.prompts = prompts == null ? new List<Prompt>() : prompts;
            Title = title;
            CurrentSubMenu = subMenus[0];

            foreach (SubMenu i in subMenus) i.Master = this;
        }

        public void AddSubMenu(SubMenu subMenu) { subMenus.Add(subMenu); }
        public void RemoveSubMenu(SubMenu subMenu) { subMenus.Remove(subMenu); }
        public void RemoveSubMenu(string title) { subMenus.Remove(GetSubMenu(title)); }
        public MenuItem GetCurrentAtIndex(int index) { return CurrentSubMenu.GetAtIndex(index); }
        public MenuItem GetCurrentSelected() { return CurrentSubMenu.GetSelection(); }
        public string GetCurrentTitle() { return CurrentSubMenu.Title; }
        public string[] GetCurrentStrings()
        {
            List<string> lines = new List<string>();

            for (int i = 0; i < CurrentSubMenu.Length; i++)
            {
                MenuItem currentItem = CurrentSubMenu.GetAtIndex(i);

                if (currentItem is ISelectable && (currentItem as ISelectable).Invisible)
                    lines.Add(null);
                else lines.Add(currentItem.ToString());
            }

            return lines.ToArray();
        }
        public SubMenu GetSubMenu(string title)
        {
            foreach (SubMenu i in subMenus)
                if (i.Title == title) return i;

            return null;
        }

        #region Data Management

        public void AddData(Data data)
        {
            if (data is DataBool)
                dataBooleans.Add(data as DataBool);
            else if (data is DataInt)
                dataIntegers.Add(data as DataInt);
            else if (data is DataDouble)
                dataDoubles.Add(data as DataDouble);
            else //if (data is DataString)
                dataStrings.Add(data as DataString);
        }

        public void AddData(Data[] data) { foreach (Data i in data) AddData(i); }

        public bool RemoveData(string name, Type type)
        {
            bool operationSuccessful = false;

            if (type == typeof(DataBool))
                operationSuccessful = dataBooleans.Remove(GetBool(name));
            else if (type == typeof(DataInt))
                operationSuccessful = dataIntegers.Remove(GetInt(name));
            else if (type == typeof(DataDouble))
                operationSuccessful = dataDoubles.Remove(GetDouble(name));
            else //if (type == typeof(DataString))
                operationSuccessful = dataStrings.Remove(GetString(name));

            return operationSuccessful;
        }

        public void AddPrompt(Prompt prompt) { prompts.Add(prompt); }

        #endregion

        #region Data Retrieval Methods

        public int? GetIntVal(string name)
        {
            DataInt data = GetInt(name);

            if (data != null)
                return data.Value;

            return null;
        }

        public double? GetDoubleVal(string name)
        {
            DataDouble data = GetDouble(name);

            if (data != null)
                return data.Value;

            return null;
        }

        public bool? GetBoolVal(string name)
        {
            DataBool data = GetBool(name);

            if (data != null)
                return data.Value;

            return null;
        }

        public string GetStringVal(string name)
        {
            DataString data = GetString(name);

            if (data != null)
                return data.Value;

            return null;
        }

        public DataInt GetInt(string name)
        {
            foreach (DataInt dataItem in dataIntegers)

                if (dataItem.Name == name)
                    return dataItem;

            return null;
        }

        public DataDouble GetDouble(string name)
        {
            foreach (DataDouble dataItem in dataDoubles)

                if (dataItem.Name == name)
                    return dataItem;

            return null;
        }

        public DataBool GetBool(string name)
        {
            foreach (DataBool dataItem in dataBooleans)

                if (dataItem.Name == name)
                    return dataItem;

            return null;
        }

        public DataString GetString(string name)
        {
            foreach (DataString dataItem in dataStrings)

                if (dataItem.Name == name)
                    return dataItem;

            return null;
        }

        public Prompt GetPrompt(string entryName)
        {
            foreach (Prompt i in prompts)
                if (i.EntryName == entryName) return i;

            return null;
        }

        #endregion

        #region Data Index Retrieval Methods

        public int? GetIntIndex(string name)
        {
            for (int i = 0; i < dataIntegers.Count; i++)

                if (dataIntegers[i].Name == name)
                    return i;

            return null;
        }

        public int? GetDoubleIndex(string name)
        {
            for (int i = 0; i < dataDoubles.Count; i++)

                if (dataDoubles[i].Name == name)
                    return i;

            return null;
        }

        public int? GetBoolIndex(string name)
        {
            for (int i = 0; i < dataBooleans.Count; i++)

                if (dataBooleans[i].Name == name)
                    return i;

            return null;
        }

        public int? GetStringIndex(string name)
        {
            for (int i = 0; i < dataStrings.Count; i++)

                if (dataStrings[i].Name == name)
                    return i;

            return null;
        }

        #endregion
    }
}