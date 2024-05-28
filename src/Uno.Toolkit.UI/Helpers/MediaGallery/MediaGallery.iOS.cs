#if __IOS__
using System;
using System.IO;
using System.Threading.Tasks;

namespace Uno.Toolkit.UI;

public partial class MediaGallery
{
	private Task<bool> CheckAccessPlatformAsync()
	{
		throw new NotImplementedException();
	}

	public Task SavePlatformAsync(MediaFileType type, Stream stream, string targetFileName)
	{
		throw new NotImplementedException();
		//var tempFile = Path.Combine(Path.GetTempPath(), targetFileName);
		//try
		//{
		//	// Write stream copy to temp
		//	using var fileStream = File.Create(tempFile);
		//	await stream.CopyToAsync(fileStream);

		//	// get the file uri
		//	var fileUri = new NSUrl(tempFile);

		//	await PhotoLibraryPerformChanges(() =>
		//	{
		//		using var request = type == MediaFileType.Video
		//		? PHAssetChangeRequest.FromVideo(fileUri)
		//		: PHAssetChangeRequest.FromImage(fileUri);
		//	}).ConfigureAwait(false);

		//}
		//finally
		//{
		//	// Attempt to delete the file
		//	File.Delete(tempFile);
		//}
	}

	//static async Task PhotoLibraryPerformChanges(Action action)
	//{
	//	var tcs = new TaskCompletionSource<Exception>(TaskCreationOptions.RunContinuationsAsynchronously);

	//	PHPhotoLibrary.SharedPhotoLibrary.PerformChanges(
	//		() =>
	//		{
	//			try
	//			{
	//				action.Invoke();
	//			}
	//			catch (Exception ex)
	//			{
	//				tcs.TrySetResult(ex);
	//			}
	//		},
	//		(success, error) =>
	//			tcs.TrySetResult(error != null ? new NSErrorException(error) : null));

	//	var exception = await tcs.Task;
	//	if (exception != null)
	//		throw exception;
	//}
}
#endif
