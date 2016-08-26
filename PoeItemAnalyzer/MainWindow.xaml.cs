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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfClipboardMonitor;

namespace PoeItemAnalyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            //var clipboardMonitor = new WindowClipboardMonitor(this);
            //clipboardMonitor.ClipboardTextChanged += ClipboardTextChanged;
        }

        private void ClipboardTextChanged(object sender, string text)
        {
            DisplayArea.Content = text;
        }

        private void openLogWindow_Click(object sender, RoutedEventArgs e)
        {
            var window = new LogWindow();
            window.Show();
        }
    }
}
