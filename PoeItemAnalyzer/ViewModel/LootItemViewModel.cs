using PoeItemAnalyzer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeItemAnalyzer.ViewModel
{
    public class LootItemViewModel
    {
        private LootItem Item { get; }

        public LootItemViewModel(string rawItemText)
        {
            Item = new LootItem(rawItemText);
        }

        public LootItemViewModel(LootItem item)
        {
            Item = item;
        }

        public string RawItemText => Item.RawItemText;
    }
}
