#if __IOS__ || __ANDROID__
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno.Toolkit.UI;

/// <summary>
/// Allows interaction with the device's media gallery.
/// </summary>
public static partial class MediaGallery
{
	/// <summary>
	/// Checks the user permission to access the device's gallery.
	/// Will trigger the permission request if not already granted.
	/// </summary>
	/// <returns>A value indicating whether the user has access.</returns>
	public static async Task<bool> CheckAccessAsync() => await CheckAccessPlatformAsync();

	/// <summary>
	/// Saves a media file to the device's gallery.
	/// </summary>
	/// <param name="type">Media file type.</param>
	/// <param name="data">Byte array representing the file.</param>
	/// <param name="targetFileName">Target file name.</param>
	/// <returns>Task representing the progress of the operation.</returns>
	public static async Task<MediaGallerySaveResult> SaveAsync(MediaFileType type, byte[] data, string targetFileName, bool overwrite)
	{
		using var memoryStream = new MemoryStream(data);
		return await SaveAsync(type, memoryStream, targetFileName, overwrite);
	}

	/// <summary>
	/// Saves a media file to the device's gallery.
	/// </summary>
	/// <param name="type">Media file type.</param>
	/// <param name="stream">Stream representing the file.</param>
	/// <param name="targetFileName">Target file name.</param>
	/// <returns>Task representing the progress of the operation.</returns>
	public static async Task<MediaGallerySaveResult> SaveAsync(MediaFileType type, Stream stream, string targetFileName, bool overwrite) =>
		await SavePlatformAsync(type, stream, targetFileName, overwrite);
}
#endif
