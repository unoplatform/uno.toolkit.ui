---
uid: Toolkit.Controls.ExtendedSplashScreen.HowTo
tags: [splash-screen, loading, startup, app-launch, extended-splash, initialization]
---

# Keep the native splash visible while your app finishes loading, and optionally overlay it with your own loading UI

**UnoFeatures:** `Toolkit` (add to `<UnoFeatures>` in your `.csproj`)

---

## How to keep the splash visible until startup work finishes

**When to use:** your app must load data or warm up services before showing the first screen.

**XAML**

```xml
<Page
    x:Class="MyApp.MainPage"
    xmlns:utu="using:Uno.Toolkit.UI">

    <utu:ExtendedSplashScreen x:Name="SplashHost">
        <!-- Real app UI goes here; it stays hidden while loading -->
        <Grid>
            <!-- your app content -->
        </Grid>
    </utu:ExtendedSplashScreen>
</Page>
```

What happens: the control recreates the app's splash screen with XAML and keeps it on top until `IsLoading` becomes `false`. See [Splash screen appearance](xref:Toolkit.Controls.ExtendedSplashScreen#splash-screen-appearance) for where the image and background color come from on each platform.

---

## How to show a progress ring over the splash

**XAML**

```xml
<Page
    x:Class="MyApp.MainPage"
    xmlns:utu="using:Uno.Toolkit.UI">

    <utu:ExtendedSplashScreen x:Name="SplashHost">

        <utu:ExtendedSplashScreen.LoadingContentTemplate>
            <DataTemplate>
                <Grid HorizontalAlignment="Center" VerticalAlignment="Center">
                    <StackPanel Spacing="12" HorizontalAlignment="Center">
                        <ProgressRing IsActive="True" Width="48" Height="48"/>
                        <TextBlock Text="Loading…" />
                    </StackPanel>
                </Grid>
            </DataTemplate>
        </utu:ExtendedSplashScreen.LoadingContentTemplate>

        <!-- App Content -->
        <Frame />
    </utu:ExtendedSplashScreen>
</Page>
```

This overlays your branding **during** the extended splash—without changing the *native* initial splash asset. ([dotnet new bald][4])

---

## How to ensure support on Android 12+ and WebAssembly

On Android, the operating system splash screen stays visible until the app renders its first frame, and then `ExtendedSplashScreen` takes over. If your activity theme derives from `Theme.SplashScreen`, install the AndroidX splash screen before `base.OnCreate`, as described in [Setup on Android](xref:Toolkit.Controls.ExtendedSplashScreen#setup-on-android). On WebAssembly, the splash screen comes from the app manifest.

---

## Notes & tips

* This control **extends** the native splash; it doesn't replace or restyle the initial OS splash. Customize the *initial* splash per platform (e.g., WASM via `AppManifest.js`) separately. ([dotnet new bald][4])
* It's a specialized "loading shell" that sits above your app content and below your optional loading overlay. Think of it as three layers: your app, the recreated splash screen, and your loading overlay.
* For videos and quick demos, see "ExtendedSplashScreen" in Uno Tech Bites. ([Uno Platform][6])

---

## Minimal checklist

1. Add **`Uno.Toolkit.WinUI`** and initialize Toolkit resources. ([GitHub][1])
2. Wrap your root content in **`ExtendedSplashScreen`**. ([Uno Platform][2])
3. Drive **`IsLoading`** from a startup task or view-model. ([Uno Platform][3])
4. (Optional) Provide **`LoadingContentTemplate`** with progress/branding. ([Uno Platform][3])

---

## FAQ

**Does this let me change the *native* splash?**

No. It only prolongs it. Customize the native splash per platform (e.g., WASM `AppManifest.js`). ([Uno Platform][7])

**Will this work on Android 12's SplashScreen API?**

Yes. The AndroidX splash screen is displayed until the first frame, and then `ExtendedSplashScreen` shows the splash screen recreated from the app's `UnoSplashScreen` definition. See [Setup on Android](xref:Toolkit.Controls.ExtendedSplashScreen#setup-on-android).

**Can I show anything while loading?**

Yes—use `LoadingContentTemplate` (e.g., `ProgressRing`, text, brand). ([Uno Platform][3])

---

**Source doc for reference:** ExtendedSplashScreen control. ([Uno Platform][2])

[1]: https://raw.githubusercontent.com/unoplatform/uno.toolkit.ui/refs/heads/main/doc/getting-started.md "https://raw.githubusercontent.com/unoplatform/uno...."
[2]: https://platform.uno/docs/articles/external/uno.toolkit.ui/doc/controls/ExtendedSplashScreen.html "ExtendedSplashScreen"
[3]: https://platform.uno/docs/articles/external/uno.chefs/doc/toolkit/ExtendedSplashScreen.html "Extending Splash Screen Duration for Custom Loading"
[4]: https://kazo0.dev/toolkit-tuesday/2024/03/12/toolkit-tuesday-extendedsplashscreen.html "Toolkit Tuesdays: ExtendedSplashScreen"
[6]: https://platform.uno/blog/uno-toolkit-ui-tech-bites/ "Uno Toolkit – an Uno Tech Bite series"
[7]: https://platform.uno/docs/articles/external/uno.wasm.bootstrap/doc/features-splash-screen.html "Splash screen customization"
