﻿#if __ANDROID__
using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Provider;
using static Android.Provider.MediaStore;
using Environment = Android.OS.Environment;
using File = Java.IO.File;
using Path = System.IO.Path;
using Stream = System.IO.Stream;
using Uri = Android.Net.Uri;

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
//		var albumName = AppInfo.Name;

//		var context = Application.Context;
//		var dateTimeNow = DateTime.Now;

//		var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(targetFileName);
//		var extension = Path.GetExtension(targetFileName).ToLower();
//		var newFileName = $"{GetNewImageName(dateTimeNow, fileNameWithoutExtension)}{extension}";

//		using var values = new ContentValues();

//		values.Put(IMediaColumns.DateAdded, TimeSeconds(dateTimeNow));
//		values.Put(IMediaColumns.Title, fileNameWithoutExtension);
//		values.Put(IMediaColumns.DisplayName, newFileName);

//		var mimeType = MimeTypeMap.Singleton.GetMimeTypeFromExtension(extension.Replace(".", string.Empty));
//		if (!string.IsNullOrWhiteSpace(mimeType))
//			values.Put(IMediaColumns.MimeType, mimeType);

//		using var externalContentUri = type == MediaFileType.Image
//			? MediaStore.Images.Media.ExternalContentUri
//			: MediaStore.Video.Media.ExternalContentUri;

//		var relativePath = type == MediaFileType.Image
//			? Environment.DirectoryPictures
//			: Environment.DirectoryMovies;

//		if (Platform.HasSdkVersion(29))
//		{
//			values.Put(IMediaColumns.RelativePath, Path.Combine(relativePath, albumName));
//			values.Put(IMediaColumns.IsPending, true);

//			using var uri = context.ContentResolver.Insert(externalContentUri, values);
//			using var stream = context.ContentResolver.OpenOutputStream(uri);
//			await fileStream.CopyToAsync(stream);
//			stream.Close();

//			values.Put(IMediaColumns.IsPending, false);
//			context.ContentResolver.Update(uri, values, null, null);
//		}
//		else
//		{
//#pragma warning disable CS0618 // Type or member is obsolete
//			using var directory = new File(Environment.GetExternalStoragePublicDirectory(relativePath), albumName);

//			directory.Mkdirs();
//			using var file = new File(directory, newFileName);

//			using var fileOutputStream = System.IO.File.Create(file.AbsolutePath);
//			await fileStream.CopyToAsync(fileOutputStream);
//			fileOutputStream.Close();

//			values.Put(MediaColumns.Data, file.AbsolutePath);
//			context.ContentResolver.Insert(externalContentUri, values);

//			using var mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
//			mediaScanIntent.SetData(Uri.FromFile(file));
//			context.SendBroadcast(mediaScanIntent);
//#pragma warning restore CS0618 // Type or member is obsolete
//		}
	}
}
#endif
