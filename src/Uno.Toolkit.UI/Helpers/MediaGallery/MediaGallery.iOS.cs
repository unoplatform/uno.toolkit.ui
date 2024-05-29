#if __IOS__
using Photos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UIKit;

namespace Uno.Toolkit.UI;

public partial class MediaGallery
{
	private static readonly string _requiredInfoPlistKey = HasOSVersion(14) ? "NSPhotoLibraryAddUsageDescription" : "NSPhotoLibraryUsageDescription";

	private static Task<bool> CheckAccessPlatformAsync()
	{
		if (NSBundle.MainBundle.InfoDictionary.ContainsKey(new NSString(usageKey)))
		{

		}
		var auth = HasOSVersion(14) ?
			PHPhotoLibrary.GetAuthorizationStatus(PHAccessLevel.AddOnly) :
			PHPhotoLibrary.AuthorizationStatus;

		return Task.FromResult(Convert(auth));
	}

	/// <summary>Request <see cref="SaveMediaPermission"/> from the user.</summary>
	/// <returns>The status of the permission that was requested.</returns>
	public override async Task<PermissionStatus> RequestAsync()
	{
		var status = await CheckStatusAsync();
		if (status == PermissionStatus.Granted)
			return status;

		var auth = HasOSVersion(14)
			? await PHPhotoLibrary.RequestAuthorizationAsync(PHAccessLevel.AddOnly)
			: await PHPhotoLibrary.RequestAuthorizationAsync();

		return Convert(auth);
	}

	PermissionStatus Convert(PHAuthorizationStatus status)
		=> status switch
		{
			PHAuthorizationStatus.Authorized => PermissionStatus.Granted,
			PHAuthorizationStatus.Limited => PermissionStatus.Granted,
			PHAuthorizationStatus.Denied => PermissionStatus.Denied,
			PHAuthorizationStatus.Restricted => PermissionStatus.Restricted,
			_ => PermissionStatus.Unknown,
		};


	public static Task<MediaGallerySaveResult> SavePlatformAsync(MediaFileType type, Stream sourceStream, string targetFileName, bool overwrite)
	{
		throw new NotImplementedException();
		var tempFile = Path.Combine(Path.GetTempPath(), targetFileName);
		try
		{
			// Write stream copy to temp
			using var fileStream = File.Create(tempFile);
			await stream.CopyToAsync(fileStream);

			// get the file uri
			var fileUri = new NSUrl(tempFile);

			await PhotoLibraryPerformChanges(() =>
			{
				using var request = type == MediaFileType.Video
				? PHAssetChangeRequest.FromVideo(fileUri)
				: PHAssetChangeRequest.FromImage(fileUri);
			}).ConfigureAwait(false);

		}
		finally
		{
			// Attempt to delete the file
			File.Delete(tempFile);
		}
	}

	static async Task PhotoLibraryPerformChanges(Action action)
	{
		var tcs = new TaskCompletionSource<Exception>(TaskCreationOptions.RunContinuationsAsynchronously);

		PHPhotoLibrary.SharedPhotoLibrary.PerformChanges(
			() =>
			{
				try
				{
					action.Invoke();
				}
				catch (Exception ex)
				{
					tcs.TrySetResult(ex);
				}
			},
			(success, error) =>
				tcs.TrySetResult(error != null ? new NSErrorException(error) : null));

		var exception = await tcs.Task;
		if (exception != null)
			throw exception;
	}

	private static bool HasOSVersion(int major) => UIDevice.CurrentDevice.CheckSystemVersion(major, 0);
}
#endif
