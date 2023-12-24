using DeleteFolders.Utils;
using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;

namespace DeleteFolders.Utils
{
    public class Utils
    {
        public static void DeleteFoldersRecursively(Spread<string> folders)
        {
            foreach (var folder in folders)
            {
                if (Directory.Exists(folder))
                {
                    DeleteFolder(folder);
                }
            }
        }

        private static async Task DeleteFolder(string folder)
        {
            var files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);


            foreach (var file in files)
            {
                File.Delete(file);
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
