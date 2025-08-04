---
uid: Toolkit.Controls.ExtendedSplashScreen
---
# ExtendedSplashScreen

Represents a control, derived from [`LoadingView`](xref:Toolkit.Controls.LoadingView) that displays a view that simulates the appearance and behavior of the target platform's native application loading screen.

Refer to [`LoadingView`](xref:Toolkit.Controls.LoadingView) for a list of inherited members.

## Remarks

The term _splash screen_ commonly refers to a view that is displayed when the user launches the application. Note that only a subset of the supported targets defines a native splash screen construct that is displayed by the platform.

A common use case for this control is to display an application loading element for extended durations after transitioning from the native splash screen. This is useful for scenarios where the application is performing a longer-running operation, such as loading data or performing a network request upon startup.

`ExtendedSplashScreen` is a custom implementation, and does not require the presence of a native splash screen. As such, it can serve as an application's standalone loading screen, regardless of whether the platform provides a native equivalent.

> [!TIP]
> Users expect applications to load quickly after they launch them, so it is important to minimize the duration that a splash screen is displayed. `ExtendedSplashScreen` is intended to support a consistent user experience across platforms where scenarios necessitate a longer startup process.
>
> [!Video https://www.youtube-nocookie.com/embed/jMI4E2e9gYE]

## Usage

### Setup the splash screen

The following code snippet demonstrates a suggested pattern for using the `ExtendedSplashScreen`. The first step is to define a custom `UserControl` that will be used as the main shell for the application content. This control will be used to host the `ExtendedSplashScreen` and the main application content.

`Shell.xaml`:

```xml
<UserControl x:Class="SplashScreenApp.Shell"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="using:SplashScreenApp"
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             xmlns:utu="using:Uno.Toolkit.UI"
             mc:Ignorable="d">

    <utu:ExtendedSplashScreen x:Name="Splash"
                              HorizontalAlignment="Stretch"
                              VerticalAlignment="Stretch"
                              HorizontalContentAlignment="Stretch"
                              VerticalContentAlignment="Stretch">
        <utu:ExtendedSplashScreen.LoadingContent>
            <Grid>
                <Grid.RowDefinitions>
                    <RowDefinition Height="2*" />
                    <RowDefinition />
                </Grid.RowDefinitions>

                <ProgressRing IsActive="True"
                              Grid.Row="1"
                              VerticalAlignment="Center"
                              HorizontalAlignment="Center"
                              Height="100"
                              Width="100" />
            </Grid>
        </utu:ExtendedSplashScreen.LoadingContent>
        <utu:ExtendedSplashScreen.Content>
            <Frame x:Name="ShellFrame" />
        </utu:ExtendedSplashScreen.Content>
    </utu:ExtendedSplashScreen>
</UserControl>
```

The `ExtendedSplashScreen` control's `LoadingContent` is used to display the splash screen and any custom content while the application is loading. The `Content` property is used to define the main application content that will be displayed after the loading state.

Next, the `Shell` control should be used as the root visual for the `Window` in the `App.cs`.

`App.cs`:

```csharp
protected override async void OnLaunched(LaunchActivatedEventArgs args)
{
    // Boilerplate code omitted for brevity

    if (MainWindow.Content is not Shell shell)
    {
        shell = new Shell();

        MainWindow.Content = shell;

        shell.RootFrame.NavigationFailed += OnNavigationFailed;
    }

    if (shell.RootFrame.Content == null)
    {
        shell.RootFrame.Navigate(typeof(MainPage), args.Arguments);
    }

    MainWindow.Activate();
}
```

With these changes, the splash screen will be displayed when the application first launches and the main application content will be displayed once the loading state is complete.

In order to prolong the splash screen display, you can set the `Source` property of the `ExtendedSplashScreen` control to any custom implementation of the `ILoadable` interface. More information on how to use the `ILoadable` interface can be found in the [`LoadingView`](xref:Toolkit.Controls.LoadingView#iloadable) documentation.
