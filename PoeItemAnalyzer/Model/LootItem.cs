using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeItemAnalyzer.Model
{
    public class LootItem
    {
        public LootItem(string rawItemText)
        {
            RawItemText = rawItemText;
        }

        public string RawItemText { get; }
    }
}
