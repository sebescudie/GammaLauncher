using DeleteFolders.Utils;
using System;
using System.IO;
using System.Linq;

namespace DeleteFolders.Utils
{
    public class Utils
    {
        public static void DeleteFoldersRecursively(Spread<string> folders, IProgress<DeleteProgress> progress)
        {
            var deleteProgress = new DeleteProgress();
            foreach (var folder in folders)
            {
                if (Directory.Exists(folder))
                {
                    DeleteFolder(folder, deleteProgress, progress);
                }
            }
        }

        private static void DeleteFolder(string folder, DeleteProgress deleteProgress, IProgress<DeleteProgress> progress)
        {
            var files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);
            deleteProgress.TotalFiles += files.Length;
            deleteProgress.CurrentFolder = folder;

            foreach (var file in files)
            {
                File.Delete(file);
                deleteProgress.FilesDeleted++;
                progress.Report(deleteProgress);
            }

            var directories = new DirectoryInfo(folder).GetDirectories();
            foreach (var dir in directories)
            {
                Directory.Delete(dir.FullName, true);
            }

            Directory.Delete(folder, false);
        }
    }
}
