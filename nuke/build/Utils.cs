using Microsoft.Build.Tasks;
using Nuke.Common.IO;
using Octokit;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace _build
{
    public static class Utils
    {
        public static void DeleteDirectoryContent(string directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath))
                throw new ArgumentException("Directory path cannot be null or empty.", nameof(directoryPath));

            var directoryInfo = new DirectoryInfo(directoryPath);

            if (!directoryInfo.Exists)
                throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");

            // Delete all files
            foreach (FileInfo file in directoryInfo.EnumerateFiles())
            {
                file.Delete();
            }

            // Delete all subdirectories
            foreach (DirectoryInfo dir in directoryInfo.EnumerateDirectories())
            {
                dir.Delete(true);
            }
        }

        public async static Task<(ReleaseAsset, Release)> DownloadReleaseAsset(GitHubClient client, string owner, string repo, string assetLookupString, AbsolutePath downloadDirectory, string tag = "")
        {
            Log.Information($"Fetching {repo} releases");
            Release release;

            // Fetch release

            if (string.IsNullOrEmpty(tag))
            {
                release = await client.Repository.Release.GetLatest(owner, repo);
            }
            else
            {
                release = await client.Repository.Release.Get(owner, repo, tag);
            }

            Log.Information($"Found release {release.Name} with {release.Assets.Count} assets");

            // Find matching assets
            var matchingAssets = release.Assets.Where(asset => asset.Name.Contains(assetLookupString.Trim())).ToList();

            if (!matchingAssets.Any())
            {
                Log.Error($"Could not find assets matching criteria! {repo} {release.Name} doesn't seem to have an asset which name contains {assetLookupString}");
                return (null, null);
            }

            var assetToDownload = matchingAssets.First();

            Log.Information($"Found matching asset: {assetToDownload.Name}");

            var downloadPath = downloadDirectory / assetToDownload.Name;

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "NukeBuild");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/octet-stream");

            var response = await httpClient.GetAsync(assetToDownload.Url);
            response.EnsureSuccessStatusCode();

            await using var fileStream = System.IO.File.Create(downloadPath);
            await response.Content.CopyToAsync(fileStream);

            Log.Information($"Asset downloaded to {downloadPath}");

            return (assetToDownload, release);
        }
    }
}
