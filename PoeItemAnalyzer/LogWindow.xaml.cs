﻿using PoeItemAnalyzer.ViewModel;
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

            var windowClipboardManager = new WindowClipboard(this);
            windowClipboardManager.ClipboardChanged += ClipboardChanged;
        }

        private void ClipboardChanged(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                // TODO: Can apparently throw
                //     System.Runtime.InteropServices.COMException
                //     {"OpenClipboard Failed (Exception from HRESULT: 0x800401D0 (CLIPBRD_E_CANT_OPEN))"}
                // See issue: https://github.com/ColinDabritz/PoeItemAnalyzer/issues/1
                var clippedText = Clipboard.GetText().Trim();

                if(string.IsNullOrWhiteSpace(clippedText))
                {
                    return;
                }

                var item = new LootItemViewModel(clippedText);

                items.Add(item);

                itemListbox.Items.MoveCurrentToLast();
                itemListbox.ScrollIntoView(itemListbox.Items.CurrentItem);
            }
        }
    }
}
