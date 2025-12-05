using Uno.Toolkit.Samples.ViewModels;

namespace Uno.Toolkit.Samples.Content.Controls
{
	[SamplePage(SampleCategory.Controls, "SkeletonView", SourceSdk.UnoToolkit, DataType = typeof(SkeletonViewViewModel))]
	public sealed partial class SkeletonViewSamplePage : Page
	{
		public SkeletonViewSamplePage()
		{
			this.InitializeComponent();
		}

		public class SkeletonViewViewModel : ViewModelBase
		{
			private bool _isLoading = true;

			public bool IsLoading
			{
				get => _isLoading;
				set
				{
					if (_isLoading != value)
					{
						_isLoading = value;
						OnPropertyChanged();
					}
				}
			}

			public AsyncCommand LoadDataCommand { get; }

			public SkeletonViewViewModel()
			{
				LoadDataCommand = new AsyncCommand(LoadDataAsync);
			}

			private async Task LoadDataAsync()
			{
				// Simulate loading data
				await Task.Delay(2000);
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
				finally
				{
					IsExecuting = false;
				}
			}
		}
	}
}
