using System;
using System.Collections.Generic;
using System.Text;
using Uno.Toolkit.Samples.Entities;
using Uno.Toolkit.Samples.Helpers;

#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Uno.Toolkit.Samples
{
	public partial class SamplePageLayout : ContentControl
	{
		#region Property: Title

		public static DependencyProperty TitleProperty { get; } = DependencyProperty.Register(
			nameof(Title),
			typeof(string),
			typeof(SamplePageLayout),
			new PropertyMetadata(default));

		public string Title
		{
			get => (string)GetValue(TitleProperty);
			set => SetValue(TitleProperty, value);
		}

		#endregion
		#region Property: Description

		public static DependencyProperty DescriptionProperty { get; } = DependencyProperty.Register(
			nameof(Description),
			typeof(string),
			typeof(SamplePageLayout),
			new PropertyMetadata(default));

		public string Description
		{
			get => (string)GetValue(DescriptionProperty);
			set => SetValue(DescriptionProperty, value);
		}

		#endregion
		#region Property: DocumentationLink

		public static DependencyProperty DocumentationLinkProperty { get; } = DependencyProperty.Register(
			nameof(DocumentationLink),
			typeof(string),
			typeof(SamplePageLayout),
			new PropertyMetadata(default));

		public string DocumentationLink
		{
			get => (string)GetValue(DocumentationLinkProperty);
			set => SetValue(DocumentationLinkProperty, value);
		}

		#endregion
		#region Property: HeaderTemplate
		/// <summary>
		/// The Header is the part above the design tabs (Material|Fluent|Native).
		/// It contains the Description and the Source in the default style.
		/// </summary>
		public DataTemplate HeaderTemplate
		{
			get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
			set { SetValue(HeaderTemplateProperty, value); }
		}

		public static readonly DependencyProperty HeaderTemplateProperty =
			DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(SamplePageLayout), new PropertyMetadata(null));
		#endregion
		#region Property: SampleTemplate

		public static DependencyProperty SampleTemplateProperty { get; } = DependencyProperty.Register(
			nameof(SampleTemplate),
			typeof(DataTemplate),
			typeof(SamplePageLayout),
			new PropertyMetadata(default));

		public DataTemplate SampleTemplate
		{
			get => (DataTemplate)GetValue(SampleTemplateProperty);
			set => SetValue(SampleTemplateProperty, value);
		}

		#endregion

		public SamplePageLayout()
		{
			DataContextChanged += OnDataContextChanged;

			void OnDataContextChanged(object sender, DataContextChangedEventArgs args)
			{
				if (args.NewValue is Sample sample)
				{
					Title = sample.Title;
					Description = sample.Description;
					DocumentationLink = sample.DocumentationLink;
				}
			}
		}

		/// <summary>
		/// Get control inside the data template.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name">The 'x:Name' of the control</param>
		/// <returns></returns>
		/// <remarks>The caller must ensure the control is loaded. This is best done from <see cref="FrameworkElement.Loaded"/> event.</remarks>
		public T GetSampleChild<T>(string name)
			where T : FrameworkElement
		{
			var presenter = (ContentPresenter)GetTemplateChild($"SampleContentPresenter");

			return VisualTreeHelperEx.GetFirstDescendant<T>(presenter, x => x.Name == name);
		}

	}
}
