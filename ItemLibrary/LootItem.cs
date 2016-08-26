using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemModel
{
    public class LootItem
    {
        public LootItem(string rawItemText)
        {
            RawItemText = rawItemText;

            var sectionSeparators = new string[] { $"\r\n--------\r\n" };

            RawSections = RawItemText.Split(sectionSeparators, StringSplitOptions.RemoveEmptyEntries);
        }

        public string RawItemText { get; }

        public string[] RawSections { get; }
    }
}
