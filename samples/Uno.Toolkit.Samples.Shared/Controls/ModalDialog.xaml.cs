using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Uno.Toolkit.Samples.Content.NestedSamples;
using Windows.Foundation;
using Windows.Foundation.Collections;
#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Uno.Toolkit.Samples
{
    public sealed partial class ModalDialog : ContentDialog
    {
        public Type FirstPage { get; set; }

        public ModalDialog()
        {
            this.InitializeComponent();
            this.Opened += ModalNavBarDialog_Opened;
        }

        private void ModalNavBarDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            ModalFrame.Navigate(FirstPage);
        }
    }
}
