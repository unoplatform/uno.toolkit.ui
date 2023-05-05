// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Uno.Toolkit.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Uno.Toolkit.Samples.Content.Controls
{
    public sealed partial class ShadowContainerSampleContent : StackPanel
    {
        public ShadowContainerSampleContent()
        {
            this.InitializeComponent();
        }

		private void AddShadow(object sender, RoutedEventArgs e)
		{
			var defaultShadow = (Shadow)Resources["DefaultShadow"];

			Shadows.Add(defaultShadow.Clone());
		}

		private void RemoveShadow(object sender, RoutedEventArgs e)
		{
			if (Shadows.Count == 0)
			{
				return;
			}

			Shadows.RemoveAt(Shadows.Count - 1);
		}
	}
}
