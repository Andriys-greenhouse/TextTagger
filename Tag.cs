using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextTagger
{
    public delegate void WriteOutFunction(string TextToWrite);

    public class Tag
    {
        string name;
        public string Name //will be used to identify tag in text
        {
            get { return name; }
            set
            {
                if (value.Length > 0) { name = value; }
                else { throw new ArgumentException("Tag name must have at least one character!"); }
            }
        }

        public string Description { get; set; }

        public WriteOutFunction Write;

        public Tag(string aName, WriteOutFunction aFunction, string aDescription = "")
        {
            Name = aName;
            Write = new WriteOutFunction(aFunction);
            Description = aDescription;
        }
    }
}
