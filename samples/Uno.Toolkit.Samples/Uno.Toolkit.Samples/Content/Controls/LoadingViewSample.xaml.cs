using Uno.Toolkit.Samples.Entities;
using Uno.Toolkit.Samples.ViewModels;
using Uno.Toolkit.UI;


namespace Uno.Toolkit.Samples.Content.Controls
{

	[SamplePage(SampleCategory.Controls, nameof(LoadingView), SourceSdk.UnoToolkit, DataType = typeof(ViewModel))]
	public sealed partial class LoadingViewSample : Page
	{
		public LoadingViewSample()
		{
			this.InitializeComponent();
			this.Loaded += (s, e) =>
			{
				if ((DataContext as Sample)?.Data is ViewModel vm)
				{
					vm.LoadContent0Command.Execute(default);
					vm.LoadContent1Command.Execute(default);
				}
			};
		}

		private class ViewModel : ViewModelBase
		{
			public string Text { get => GetProperty<string>(); set => SetProperty(value); }
			public IEnumerable<int> Source { get => GetProperty<IEnumerable<int>>(); set => SetProperty(value); }

			public AsyncCommand LoadContent0Command { get; }
			public AsyncCommand LoadContent1Command { get; }

			private readonly Random _random = new Random();

			public ViewModel()
			{
				LoadContent0Command = new AsyncCommand(LoadContent0);
				LoadContent1Command = new AsyncCommand(LoadContent1);
			}

			public Task LoadContent0() => SimulateFetchOrWork(500, () => Text = DateTime.Now.ToString());
			public Task LoadContent1() => SimulateFetchOrWork(2000, () => Source = Enumerable.Range(0, _random.Next(3, 12)));

			private async Task SimulateFetchOrWork(int delay, Action onCompleted)
			{
				await Task.Delay(delay);
				onCompleted();
			}
		}

		public class AsyncCommand : ICommand, ILoadable
		{
			public event EventHandler CanExecuteChanged;
			public event EventHandler IsExecutingChanged;

			private Func<Task> _executeAsync;
			private bool _isExecuting;

			public AsyncCommand(Func<Task> executeAsync)
			{
				_executeAsync = executeAsync;
			}

			public bool CanExecute(object parameter) => !IsExecuting;

			public bool IsExecuting
			{
				get => _isExecuting;
				set
				{
					if (_isExecuting != value)
					{
						_isExecuting = value;
						IsExecutingChanged?.Invoke(this, new());
						CanExecuteChanged?.Invoke(this, new());
					}
				}
			}

			public async void Execute(object parameter)
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
