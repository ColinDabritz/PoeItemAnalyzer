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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PoeItemAnalyzer.Controls
{
    /// <summary>
    /// Interaction logic for ItemDisplay.xaml
    /// </summary>
    public partial class ItemDisplay : UserControl
    {
        public ItemDisplay()
        {
            InitializeComponent();
        }

        private void CopyRawText_Click(object sender, RoutedEventArgs e)
        {
            var source = e.Source as MenuItem;
            var itemModel = source.DataContext as LootItemViewModel;
            Clipboard.SetText(itemModel.RawItemText);
            // TODO: supress outgoing text from incoming capture
        }

        private async void LookupPrice_Click(object sender, RoutedEventArgs e)
        {
            var source = e.Source as MenuItem;
            var itemModel = source.DataContext as LootItemViewModel;

            await itemModel.PopulatePriceInfoFromWeb();
        }
    }
}
