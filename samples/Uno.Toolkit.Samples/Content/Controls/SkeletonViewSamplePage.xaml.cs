using Uno.Toolkit.Samples.ViewModels;

namespace Uno.Toolkit.Samples.Content.Controls
{
	[SamplePage(SampleCategory.Controls, "SkeletonView", SourceSdk.UnoToolkit, DataType = typeof(SkeletonViewViewModel), SupportedDesigns = new[] { Design.Material, Design.Cupertino })]
	public sealed partial class SkeletonViewSamplePage : Page
	{
		public SkeletonViewSamplePage()
		{
			this.InitializeComponent();
		}

		public class SkeletonViewViewModel : ViewModelBase
		{
			public bool IsLoading
			{
				get => GetProperty<bool>();
				set
				{
					SetProperty(value);

					if (!value && Title is null)
					{
						PopulateData();
					}
				}
			}

			public string? Title { get => GetProperty<string>(); set => SetProperty(value); }
			public string? Subtitle { get => GetProperty<string>(); set => SetProperty(value); }
			public string? Description { get => GetProperty<string>(); set => SetProperty(value); }

			public AsyncCommand LoadDataCommand { get; }

			public SkeletonViewViewModel()
			{
				IsLoading = true;
				LoadDataCommand = new AsyncCommand(LoadDataAsync);
			}

			private async Task LoadDataAsync()
			{
				Title = Subtitle = Description = null;

				// Simulate loading data
				await Task.Delay(2000);

				PopulateData();
			}

			private void PopulateData()
			{
				Title = "John Doe";
				Subtitle = "Software Engineer";
				Description = "This is the actual content that appears once loading completes. While the data was being fetched, placeholders matching this layout were generated automatically.";
			}
		}

		public class AsyncCommand : ICommand, ILoadable
		{
			public event EventHandler? CanExecuteChanged;
			public event EventHandler? IsExecutingChanged;

			private readonly Func<Task> _executeAsync;
			private bool _isExecuting;

			public AsyncCommand(Func<Task> executeAsync)
			{
				_executeAsync = executeAsync;
			}

			public bool CanExecute(object? parameter) => !IsExecuting;

			public bool IsExecuting
			{
				get => _isExecuting;
				set
				{
					if (_isExecuting != value)
					{
						_isExecuting = value;
						IsExecutingChanged?.Invoke(this, EventArgs.Empty);
						CanExecuteChanged?.Invoke(this, EventArgs.Empty);
					}
				}
			}

			public async void Execute(object? parameter)
			{
				try
				{
					IsExecuting = true;
					await _executeAsync();
				}
				catch (Exception)
				{
					// Simulated load; nothing to surface. async void must not throw.
				}
				finally
				{
					IsExecuting = false;
				}
			}
		}
	}
}
