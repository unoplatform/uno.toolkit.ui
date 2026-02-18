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
