#if __ANDROID__
using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Webkit;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Extensions;
using static Android.Provider.MediaStore;
using Environment = Android.OS.Environment;
using File = Java.IO.File;
using Path = System.IO.Path;
using Stream = System.IO.Stream;
using Uri = Android.Net.Uri;

namespace Uno.Toolkit.UI;

partial class MediaGallery
{
	private static readonly DateTime _unixStartDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	private static async Task<bool> CheckAccessPlatformAsync()
	{
		if ((int)Build.VERSION.SdkInt < 29)
		{
			return await PermissionsHelper.CheckWriteExternalStoragePermission(default);
		}
		else
		{
			return true;
		}
	}

	private static async Task SavePlatformAsync(MediaFileType type, Stream sourceStream, string targetFileName)
	{
		var context = Application.Context;
		var contentResolver = context.ContentResolver ?? throw new InvalidOperationException("ContentResolver is not set.");

		var appFolderName = Package.Current.DisplayName;
		// Ensure folder name is file system safe
		appFolderName = string.Join("_", appFolderName.Split(Path.GetInvalidFileNameChars()));

		var dateTimeNow = DateTime.Now;

		var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(targetFileName);
		var extension = Path.GetExtension(targetFileName).ToLower();

		using var values = new ContentValues();

		values.Put(IMediaColumns.DateAdded, TimeSeconds(dateTimeNow));
		values.Put(IMediaColumns.Title, fileNameWithoutExtension);
		values.Put(IMediaColumns.DisplayName, targetFileName);

		var mimeTypeMap = MimeTypeMap.Singleton ?? throw new InvalidOperationException("MimeTypeMap is not set.");

		var mimeType = mimeTypeMap.GetMimeTypeFromExtension(extension.Replace(".", string.Empty));
		if (!string.IsNullOrWhiteSpace(mimeType))
			values.Put(IMediaColumns.MimeType, mimeType);

		using var externalContentUri = type == MediaFileType.Image
			? Images.Media.ExternalContentUri
			: Video.Media.ExternalContentUri;

		if (externalContentUri is null)
		{
			throw new InvalidOperationException($"External Content URI for {type} is not available.");
		}

		var relativePath = type == MediaFileType.Image
			? Environment.DirectoryPictures
			: Environment.DirectoryMovies;

		if (relativePath is null)
		{
			throw new InvalidOperationException($"Relative path for {type} is not available.");
		}

		if ((int)Build.VERSION.SdkInt >= 29)
		{
			values.Put(IMediaColumns.RelativePath, Path.Combine(relativePath, appFolderName));
			values.Put(IMediaColumns.IsPending, true);

			using var uri = contentResolver.Insert(externalContentUri, values);

			if (uri is null)
			{
				throw new InvalidOperationException("Could not generate new content URI");
			}

			using var stream = contentResolver.OpenOutputStream(uri);

			if (stream is null)
			{
				throw new InvalidOperationException("Could not open output stream");
			}

			await sourceStream.CopyToAsync(stream);
			stream.Close();

			values.Put(IMediaColumns.IsPending, false);
			context.ContentResolver.Update(uri, values, null, null);
		}
		else
		{
#pragma warning disable CS0618 // Type or member is obsolete
			using var directory = new File(Environment.GetExternalStoragePublicDirectory(relativePath), appFolderName);
			directory.Mkdirs();

			using var file = new File(directory, targetFileName);

			using var fileOutputStream = System.IO.File.Create(file.AbsolutePath);
			await sourceStream.CopyToAsync(fileOutputStream);
			fileOutputStream.Close();

			values.Put(IMediaColumns.Data, file.AbsolutePath);
			contentResolver.Insert(externalContentUri, values);

#pragma warning disable CA1422 // Validate platform compatibility
			using var mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
#pragma warning restore CA1422 // Validate platform compatibility
			mediaScanIntent.SetData(Uri.FromFile(file));
			context.SendBroadcast(mediaScanIntent);
#pragma warning restore CS0618 // Type or member is obsolete
		}
	}

	private static long TimeMillis(DateTime current) => (long)GetTimeDifference(current).TotalMilliseconds;

	private static long TimeSeconds(DateTime current) => (long)GetTimeDifference(current).TotalSeconds;

	private static TimeSpan GetTimeDifference(DateTime current) => current.ToUniversalTime() - _unixStartDate;
}
#endif
