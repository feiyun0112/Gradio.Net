using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gradio.Net.Themes
{
    using System;
    using System.Collections.Generic;

    public class Font
    {
        public string Name { get; }

        public Font(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            string[] standardFonts = { "sans-serif", "serif", "monospace", "cursive", "fantasy" };
            return Array.Exists(standardFonts, font => font == Name) ? Name : $"'{Name}'";
        }

        public virtual Dictionary<string, string> Stylesheet()
        {
            return new Dictionary<string, string> { { "url", null }, { "css", null } };
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Font other = (Font)obj;
            return Name == other.Name && StylesheetEquals(other);
        }

        private bool StylesheetEquals(Font other)
        {
            var thisStylesheet = Stylesheet();
            var otherStylesheet = other.Stylesheet();

            // Simple comparison of dictionaries; assuming the keys and values are directly comparable  
            return thisStylesheet.Count == otherStylesheet.Count &&
                   !thisStylesheet.Except(otherStylesheet).Any();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
