using PoeItemAnalyzer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfClipboardMonitor;

namespace PoeItemAnalyzer
{
    /// <summary>
    /// Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class LogWindow : Window
    {
        ItemLogViewModel items = new ItemLogViewModel();

        public LogWindow()
        {
            InitializeComponent();

            itemListbox.ItemsSource = items;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var clipboardMonitor = new WindowClipboardMonitor(this);

            clipboardMonitor.ClipboardTextChanged += ClipboardTextChanged;
        }

        private async void ClipboardTextChanged(object sender, string text)
        {
            var clippedText = text.Trim();

            if (string.IsNullOrWhiteSpace(clippedText))
            {
                return;
            }

            var item = new LootItemViewModel(clippedText);

            items.Add(item);

            itemListbox.Items.MoveCurrentToLast();
            itemListbox.ScrollIntoView(itemListbox.Items.CurrentItem);

            await item.PopulatePriceInfoFromWeb();
        }
    }
}
