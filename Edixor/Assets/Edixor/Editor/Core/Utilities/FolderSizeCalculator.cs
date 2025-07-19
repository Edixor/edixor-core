using System.IO;
using System.Text;

namespace ExTools
{
    public static class FolderSizeCalculator
    {
        public static string GetFolderSizeFormatted(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                return "Folder does not exist";

            long totalSize = 0;
            string[] files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                if (File.Exists(file))
                    totalSize += new FileInfo(file).Length;
            }

            return FormatSize(totalSize);
        }

        private static string FormatSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }
    }
}

