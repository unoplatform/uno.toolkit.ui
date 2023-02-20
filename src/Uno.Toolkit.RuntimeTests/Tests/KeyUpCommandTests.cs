using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Uno.Toolkit.RuntimeTests.Helpers;
using Uno.Toolkit.UI;
using Uno.UI.RuntimeTests;
using Windows.System;
using Windows.UI.Input.Preview.Injection;
#if IS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
#endif

namespace Uno.Toolkit.RuntimeTests.Tests;

[TestClass]
[RunsOnUIThread]
internal class KeyUpCommandTests
{
	[TestMethod]
	public async Task OnKeyUp()
	{
		var viewModel = new ViewModel();
		var page = new Page();
		page.DataContext = viewModel;

		page.SetBinding(InputExtensions.KeyUpCommandProperty, new Binding
		{
			Path = new PropertyPath(nameof(viewModel.KeyUpCommand)),
			Mode = BindingMode.OneWay
		});

		await UnitTestUIContentHelperEx.SetContentAndWait(page);

		InputInjector? inputInjector = InputInjector.TryCreate();

		if (inputInjector != null)
		{
			var number0 = new InjectedInputKeyboardInfo
			{
				VirtualKey = (ushort)(VirtualKey.Number0),
				KeyOptions = InjectedInputKeyOptions.KeyUp
			};

			page.Focus(FocusState.Pointer);
			inputInjector.InjectKeyboardInput(new[] { number0 });
		}

		Assert.AreEqual("Number0 pressed", viewModel.Text);
	}
}

public class ViewModel
{
	public ICommand KeyUpCommand { get; }

	public string Text { get; set; } = "Nothing pressed";

	public ViewModel()
	{
		KeyUpCommand = new RelayCommand<VirtualKey>(key => Text = $"{key} pressed");
	}
}

public class RelayCommand<T> : ICommand
{
	private readonly Action<T> _execute;
	private readonly Func<T, bool> _canExecute;

	public RelayCommand(Action<T> execute, Func<T, bool>? canExecute = null)
	{
		_execute = execute ?? throw new ArgumentNullException(nameof(execute));
		_canExecute = canExecute ?? (_ => true);
	}

	public bool CanExecute(object? parameter)
	{
		RaiseCanExecuteChanged();

		return _canExecute((T)parameter!);
	}

	public void Execute(object? parameter)
	{
		_execute((T)parameter!);
	}

	public void RaiseCanExecuteChanged()
	{
		CanExecuteChanged?.Invoke(this, EventArgs.Empty);
	}

	public event EventHandler? CanExecuteChanged;
}

