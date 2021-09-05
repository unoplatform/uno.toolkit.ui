using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Uno.Toolkit.Samples.Entities;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Uno.Toolkit.Samples.Content.Controls
{
    [SamplePage(SampleCategory.Controls, "SegmentedControls", DataType = typeof(TabBarViewModel))]
    public sealed partial class SegmentedControlsSamplePage : Page
    {
        public SegmentedControlsSamplePage()
        {
            this.InitializeComponent();
        }
    }
}
