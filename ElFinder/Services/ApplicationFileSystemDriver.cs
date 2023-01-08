using elFinder.Net.Core;
using elFinder.Net.Core.Extensions;
using elFinder.Net.Core.Models.Command;
using elFinder.Net.Core.Models.Response;
using elFinder.Net.Core.Services;
using elFinder.Net.Core.Services.Drawing;
using elFinder.Net.Drivers.FileSystem;
using elFinder.Net.Drivers.FileSystem.Services;

namespace RezUtility.ElFinder.Services;

public class ApplicationFileSystemDriver : FileSystemDriver
{
	public ApplicationFileSystemDriver(
		IPathParser pathParser,
		IPictureEditor pictureEditor,
		IVideoEditor videoEditor,
		IZipDownloadPathProvider zipDownloadPathProvider,
		IZipFileArchiver zipFileArchiver,
		IThumbnailBackgroundGenerator thumbnailBackgroundGenerator,
		ICryptographyProvider cryptographyProvider,
		IConnector connector,
		IConnectorManager connectorManager,
		ITempFileCleaner tempFileCleaner)
		: base(pathParser, pictureEditor, videoEditor,
			   zipDownloadPathProvider, zipFileArchiver,
			   thumbnailBackgroundGenerator, cryptographyProvider,
			   connector, connectorManager, tempFileCleaner)
	{
	}

	public async Task<SearchResponse> SearchMatchFolderOnly(SearchCommand cmd, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		SearchResponse searchResp = new();
		PathInfo? targetPath = cmd.TargetPath;
		IVolume? volume = targetPath.Volume;

		foreach (IDirectory? item in await targetPath.Directory.GetDirectoriesAsync(cmd.Q, searchOption: SearchOption.AllDirectories, cancellationToken: cancellationToken))
		{
			string? hash = item.GetHash(volume, pathParser);
			string? parentHash = item.Parent.Equals(targetPath.Directory) ? targetPath.HashedTarget : item.GetParentHash(volume, pathParser);
			searchResp.files.Add(await item.ToFileInfoAsync(hash, parentHash, volume, connector.Options, cancellationToken));
		}

		return searchResp;
	}
}