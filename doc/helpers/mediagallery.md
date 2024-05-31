---
uid: Toolkit.Helpers.MediaGallery
---
# MediaGallery

## Summary

`MediaGallery` is a static class that allows interaction with the device's media gallery, providing methods to check access permissions and save media files.

## Remarks

This class is designed to work on iOS and Android platforms, utilizing platform-specific implementations for its methods.

## Methods

### CheckAccessAsync

```csharp
public static async Task<bool> CheckAccessAsync()
```

#### Summary

Checks the user's permission to access the device's gallery. Will trigger the permission request if not already granted.

#### Returns

A `Task<bool>` that completes with `true` if access is granted, and `false` otherwise.

### SaveAsync (Stream)

```csharp
public static async Task SaveAsync(MediaFileType type, Stream stream, string targetFileName)
```

#### Summary

Saves a media file to the device's gallery using a stream.

#### Parameters

- `MediaFileType type`: The type of the media file (e.g., image, video).
- `Stream stream`: A stream representing the media file.
- `string targetFileName`: The desired file name for the saved media.

#### Returns

A `Task` that completes when the save operation is finished.

### SaveAsync (byte array)

```csharp
public static async Task SaveAsync(MediaFileType type, byte[] data, string targetFileName)
```

#### Summary

Saves a media file to the device's gallery using a byte array.

#### Parameters

- `MediaFileType type`: The type of the media file (e.g., image, video).
- `byte[] data`: A byte array representing the media file.
- `string targetFileName`: The desired file name for the saved media.

#### Returns

A `Task` that completes when the save operation is finished.

## Usage

Make sure to wrap the usage of `MediaGallery` in a `#if __ANDROID__ || __IOS__` ... `#endif` block, as the class is only available on these targets.

### Checking for gallery access

```csharp
#if __ANDROID__ || __IOS__
bool hasAccess = await MediaGallery.CheckAccessAsync();
#endif
```

### Saving an image to the gallery using a byte array

```csharp
#if __ANDROID__ || __IOS__
byte[] imageData = ...; // Image data
await MediaGallery.SaveAsync(MediaFileType.Image, imageData, "MyImage.jpg");
#endif
```

### Saving a video to the gallery using a stream

```csharp
#if __ANDROID__ || __IOS__
using Stream videoStream = ...; // Video stream
await MediaGallery.SaveAsync(MediaFileType.Video, videoStream, "MyVideo.mp4");
#endif
```

### Copying an application package file to gallery

```csharp
#if __ANDROID__ || __IOS__
if (await MediaGallery.CheckAccessAsync())
{
	var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/UnoLogo.png", UriKind.Absolute));
	using var stream = await file.OpenStreamForReadAsync();
	await MediaGallery.SaveAsync(MediaFileType.Image, stream, "UnoLogo.png");
}
else
{
	await new ContentDialog
	{
		Title = "Permission required",
		Content = "The app requires access to the device's gallery to save the image.",
		CloseButtonText = "OK",
		XamlRoot = XamlRoot
	}.ShowAsync();
}
#endif
```