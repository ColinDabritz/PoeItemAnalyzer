using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
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

namespace TestPanel
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // used to test:
            // https://github.com/ColinDabritz/PoeItemAnalyzer/issues/1
            // can cause errors in THIS app or the main app.
            // See notes in thread.

            var sendTimer = Observable.Interval(TimeSpan.FromMilliseconds(10));

            sendTimer
                .ObserveOnDispatcher()
                .Where(_ => SendCheckbox.IsChecked.HasValue && SendCheckbox.IsChecked.Value)
                .Subscribe(_ => WriteToClipboard());

            var readTimer = Observable.Interval(TimeSpan.FromMilliseconds(10));

            readTimer
                .ObserveOnDispatcher()
                .Where(_ => ReadCheckbox.IsChecked.HasValue && ReadCheckbox.IsChecked.Value)
                .Subscribe(_ => ReadFromClipboard());
        }

        public void WriteToClipboard()
        {
            try
            {
                Clipboard.SetText("SPAMMING THE CLIPBOARD FOR TESTING - TestPanel Window <3");
            }
            catch (ExternalException)
            {
                Console.WriteLine("Write encountered external exception.");
                // Supress, we are testing OTHER applications..
            }
        }

        public void ReadFromClipboard()
        {
            try
            {
                Clipboard.GetText();
            }
            catch (ExternalException)
            {
                Console.WriteLine("Read encountered external exception.");
                // Supress, we are testing OTHER applications..
            }
            
            
        }
    }
}
