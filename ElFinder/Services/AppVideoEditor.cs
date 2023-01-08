using elFinder.Net.Core;
using elFinder.Net.Core.Services.Drawing;
using Xabe.FFmpeg;

namespace RezUtility.ElFinder.Services;

public class AppVideoEditor : IVideoEditor
{
	private const string FFmpegDirectory = nameof(FFmpeg);
	private const string DefaultImageExt = ".png";

	private static readonly IEnumerable<string> VideoExts = new[]
															{
																".mp4", ".avi", ".mxf", ".webm", ".mkv", ".flv", ".mpeg", ".mov"
															};

	private readonly IPictureEditor _pictureEditor;

	static AppVideoEditor()
	{
		FFmpeg.SetExecutablesPath(Path.Combine(AppContext.BaseDirectory, FFmpegDirectory));
	}

	public AppVideoEditor(IPictureEditor pictureEditor)
	{
		_pictureEditor = pictureEditor;
	}

	public bool CanProcessFile(string fileExtension)
	{
		return VideoExts.Contains(fileExtension, StringComparer.InvariantCultureIgnoreCase);
	}

	public async Task<ImageWithMimeType> GenerateThumbnailAsync(IFile file, int size,
																bool keepAspectRatio, CancellationToken cancellationToken = default)
	{
		string output = Path.GetTempFileName() + DefaultImageExt;
		try
		{
			IConversion conversion = (await FFmpeg.Conversions.FromSnippet.Snapshot(file.FullName, output, TimeSpan.Zero))
				.SetPreset(ConversionPreset.UltraFast);
			_ = await conversion.Start(cancellationToken);

			await using FileStream inputImage = new(output, FileMode.Open);
			return await _pictureEditor.GenerateThumbnailAsync(inputImage, size, keepAspectRatio);
		}
		finally
		{
			if (File.Exists(output)) File.Delete(output);
		}
	}

	public async Task<ImageWithMimeType> GenerateThumbnailInBackgroundAsync(IFile file, int size,
																			bool keepAspectRatio, CancellationToken cancellationToken = default)
	{
		string output = Path.GetTempFileName() + DefaultImageExt;
		try
		{
			IMediaInfo? mInfo = await FFmpeg.GetMediaInfo(file.FullName, cancellationToken);
			IConversion conversion = (await FFmpeg.Conversions.FromSnippet.Snapshot(file.FullName, output, mInfo.Duration / 2))
				.SetPreset(ConversionPreset.UltraFast);
			_ = await conversion.Start(cancellationToken);

			await using FileStream inputImage = new(output, FileMode.Open);
			return await _pictureEditor.GenerateThumbnailAsync(inputImage, size, keepAspectRatio);
		}
		finally
		{
			if (File.Exists(output)) File.Delete(output);
		}
	}
}