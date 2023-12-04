using System;
using System.Collections.Generic;
using Windows.Foundation;
using System.Linq;
using System.Text;
using Uno.Disposables;
using Uno.Extensions;
using Uno.Toolkit.UI;
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
		private const string VisualStateMaterial = nameof(Design.Material);
		private const string VisualStateCupertino = nameof(Design.Cupertino);
		private const string VisualStateFluent = nameof(Design.Fluent);
		private const string VisualStateAgnostic = nameof(Design.Agnostic);

		private const string MaterialRadioButtonPartName = "PART_MaterialRadioButton";
		private const string CupertinoRadioButtonPartName = "PART_CupertinoRadioButton";
		private const string FluentRadioButtonPartName = "PART_FluentRadioButton";
		private const string StickyMaterialRadioButtonPartName = "PART_StickyMaterialRadioButton";
		private const string StickyCupertinoRadioButtonPartName = "PART_StickyCupertinoRadioButton";
		private const string StickyFluentRadioButtonPartName = "PART_StickyFluentRadioButton";
		private const string ScrollingTabsPartName = "PART_ScrollingTabs";
		private const string StickyTabsPartName = "PART_StickyTabs";
		private const string ScrollViewerPartName = "PART_ScrollViewer";
		private const string TopPartName = "PART_MobileTopBar";
		private const string MaterialVersionComboBoxName = "MaterialVersionComboBox";

		private static Design _design = Design.Material;

		private IReadOnlyCollection<LayoutModeMapping> LayoutModeMappings => new List<LayoutModeMapping>
		{
			new LayoutModeMapping(Design.Material, () => !IsDesignAgnostic, _materialRadioButton, _stickyMaterialRadioButton, VisualStateMaterial, MaterialTemplate),
			new LayoutModeMapping(Design.Cupertino, () => !IsDesignAgnostic, _cupertinoRadioButton, _stickyCupertinoRadioButton, VisualStateCupertino, CupertinoTemplate),
			new LayoutModeMapping(Design.Fluent, () => !IsDesignAgnostic, _fluentRadioButton, _stickyFluentRadioButton, VisualStateFluent, FluentTemplate),
			new LayoutModeMapping(Design.Agnostic, () => IsDesignAgnostic, null, null, VisualStateAgnostic, DesignAgnosticTemplate),
		};

		private RadioButton _materialRadioButton;
		private RadioButton _cupertinoRadioButton;
		private RadioButton _fluentRadioButton;
		private RadioButton _stickyMaterialRadioButton;
		private RadioButton _stickyCupertinoRadioButton;
		private RadioButton _stickyFluentRadioButton;
		private FrameworkElement _scrollingTabs;
		private FrameworkElement _stickyTabs;
		private FrameworkElement _top;
		private ScrollViewer _scrollViewer;
		private ComboBox _materialVersionComboBox;

		private readonly SerialDisposable _subscriptions = new SerialDisposable();

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
					Source = sample.Source;
				}
			}
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_materialRadioButton = (RadioButton)GetTemplateChild(MaterialRadioButtonPartName);
			_cupertinoRadioButton = (RadioButton)GetTemplateChild(CupertinoRadioButtonPartName);
			_fluentRadioButton = (RadioButton)GetTemplateChild(FluentRadioButtonPartName);
			_stickyMaterialRadioButton = (RadioButton)GetTemplateChild(StickyMaterialRadioButtonPartName);
			_stickyCupertinoRadioButton = (RadioButton)GetTemplateChild(StickyCupertinoRadioButtonPartName);
			_stickyFluentRadioButton = (RadioButton)GetTemplateChild(StickyFluentRadioButtonPartName);
			_scrollingTabs = (FrameworkElement)GetTemplateChild(ScrollingTabsPartName);
			_stickyTabs = (FrameworkElement)GetTemplateChild(StickyTabsPartName);
			_scrollViewer = (ScrollViewer)GetTemplateChild(ScrollViewerPartName);
			_top = (FrameworkElement)GetTemplateChild(TopPartName);
			_materialVersionComboBox = (ComboBox)GetTemplateChild(MaterialVersionComboBoxName);

			// ensure previous subscriptions is removed before adding new ones, in case OnApplyTemplate is called multiple times
			var disposables = new CompositeDisposable();
			_subscriptions.Disposable = disposables;

			_scrollViewer.ViewChanged += OnScrolled;
			Disposable
				.Create(() => _scrollViewer.ViewChanged -= OnScrolled)
				.DisposeWith(disposables);

			_materialVersionComboBox.Loaded += OnMaterialVersionComboBoxLoaded;
			Disposable
				.Create(() => _materialVersionComboBox.Loaded -= OnMaterialVersionComboBoxLoaded)
				.DisposeWith(disposables);

			BindOnClick(_materialRadioButton);
			BindOnClick(_cupertinoRadioButton);
			BindOnClick(_fluentRadioButton);
			BindOnClick(_stickyMaterialRadioButton);
			BindOnClick(_stickyCupertinoRadioButton);
			BindOnClick(_stickyFluentRadioButton);

			UpdateLayoutRadioButtons();

			void BindOnClick(RadioButton radio)
			{
				radio.Click += OnLayoutRadioButtonChecked;
				Disposable
					.Create(() => radio.Click -= OnLayoutRadioButtonChecked)
					.DisposeWith(disposables);
			}

			void OnScrolled(object sender, ScrollViewerViewChangedEventArgs e)
			{
				var relativeOffset = GetRelativeOffset();
				if (relativeOffset < 0)
				{
					_stickyTabs.Visibility = Visibility.Visible;
				}
				else
				{
					_stickyTabs.Visibility = Visibility.Collapsed;
				}
			}
		}

		private void OnMaterialVersionComboBoxLoaded(object sender, RoutedEventArgs e)
		{
			_materialVersionComboBox.SelectedIndex = 1;
		}

		private void RegisterEvent(RoutedEventHandler click)
		{
			click += OnLayoutRadioButtonChecked;
		}

		private void UpdateLayoutRadioButtons()
		{
			var mappings = LayoutModeMappings;
			var previouslySelected = default(LayoutModeMapping);

			bool IsAvailable(LayoutModeMapping mapping) => mapping.Predicate() && mapping.Template != null;

			foreach (var mapping in mappings)
			{
				var available = IsAvailable(mapping);
				var visibility = available ? Visibility.Visible : Visibility.Collapsed;
				mapping.RadioButton?.Apply(x => x.Visibility = visibility);
				mapping.StickyRadioButton?.Apply(x => x.Visibility = visibility);

				if (mapping.Design == _design && available)
				{
					previouslySelected = mapping;
				}
			}

			// selected mode is based on previous selection and availability (whether the template is defined)
			var selected = previouslySelected ?? mappings.FirstOrDefault(IsAvailable);
			if (selected != null)
			{
				UpdateLayoutMode(transitionTo: selected.Design);
			}
		}

		private void OnLayoutRadioButtonChecked(object sender, RoutedEventArgs e)
		{
			if (sender is RadioButton radio &&
				LayoutModeMappings.FirstOrDefault(x => x.RadioButton == radio || x.StickyRadioButton == radio) is LayoutModeMapping mapping)
			{
				_design = mapping.Design;
				UpdateLayoutMode();
			}
		}

		private void UpdateLayoutMode(Design? transitionTo = null)
		{
			var design = transitionTo ?? _design;

			var current = LayoutModeMappings.FirstOrDefault(x => x.Design == design);
			if (current != null)
			{
				current.RadioButton?.Apply(x => x.IsChecked = true);
				current.StickyRadioButton?.Apply(x => x.IsChecked = true);

				VisualStateManager.GoToState(this, current.VisualStateName, useTransitions: true);
			}
		}

		private double GetRelativeOffset()
		{
#if NETFX_CORE
			// On UWP we can count on finding a ScrollContentPresenter. 
			var scp = VisualTreeHelperEx.GetFirstDescendant<ScrollContentPresenter>(_scrollViewer);
			var content = scp?.Content as FrameworkElement;
			var transform = _scrollingTabs.TransformToVisual(content);
			return transform.TransformPoint(new Point(0, 0)).Y - _scrollViewer.VerticalOffset;
#elif __IOS__
			var transform = _scrollingTabs.TransformToVisual(_scrollViewer);
			return transform.TransformPoint(new Point(0, 0)).Y;
#else
			var transform = _scrollingTabs.TransformToVisual(this);
			return transform.TransformPoint(new Point(0, 0)).Y - _top.ActualHeight;
#endif
		}

		/// <summary>
		/// Get control inside the specified layout template.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="mode">The layout mode in which the control is defined</param>
		/// <param name="name">The 'x:Name' of the control</param>
		/// <returns></returns>
		/// <remarks>The caller must ensure the control is loaded. This is best done from <see cref="FrameworkElement.Loaded"/> event.</remarks>
		public T GetSampleChild<T>(Design mode, string name)
			where T : FrameworkElement
		{
			var presenter = mode switch
			{
				Design.Material => this
					.GetFirstDescendant<ContentPresenter>(x =>
						x.Name is "M2MaterialContentPresenter" or "M3MaterialContentPresenter" &&
						x.Visibility == Visibility.Visible),
				_ => GetTemplateChild($"{mode}ContentPresenter"),
			};

			return presenter.GetFirstDescendant<T>(x => x.Name == name);
		}

		/// <summary>
		/// Get the active presenter for the selected design system.
		/// </summary>
		/// <returns></returns>
		public ContentPresenter GetActivePresenter()
		{
			var mode = (IsDesignAgnostic ? Design.Agnostic : _design);
			return mode switch
			{
				Design.Material => this
					.GetFirstDescendant<ContentPresenter>(x =>
						x.Name is "M2MaterialContentPresenter" or "M3MaterialContentPresenter" &&
						x.Visibility == Visibility.Visible),
				_ => (ContentPresenter)GetTemplateChild($"{mode}ContentPresenter"),
			};
		}

		private class LayoutModeMapping
		{
			public Design Design { get; set; }
			public Func<bool> Predicate { get; set; }
			public RadioButton RadioButton { get; set; }
			public RadioButton StickyRadioButton { get; set; }
			public string VisualStateName { get; set; }
			public DataTemplate Template { get; set; }

			public LayoutModeMapping(
				Design design, Func<bool> predicate,
				RadioButton radioButton, RadioButton stickyRadioButton, string visualStateName, DataTemplate template
			)
			{
				Design = design;
				Predicate = predicate;
				RadioButton = radioButton;
				StickyRadioButton = stickyRadioButton;
				VisualStateName = visualStateName;
				Template = template;
			}
		}
	}
}
