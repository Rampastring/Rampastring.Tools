using System.Linq;
using System.IO;
using System;

namespace Rampastring.Tools
{
    public static class SafePath
    {
        /// <summary>
        /// Safely combines multiple directory paths with or without a file name, or just a file name, for all platforms.
        /// The first path can be a relative or absolute path.
        /// </summary>
        /// <param name="paths">Ordered list of directory paths with a file name, or just a file name.</param>
        /// <returns>The combined path of all <paramref name="paths"/> without leading or trailing separator characters.</returns>
        public static string Combine(params string[] paths)
        {
            string combinedPath = paths.Aggregate((x, y) => y is null ? GetPath(x) : Path.Combine(GetPath(x), GetPath(y)))
                .TrimStart(Path.DirectorySeparatorChar)
                .TrimStart(Path.AltDirectorySeparatorChar)
                .TrimEnd(Path.DirectorySeparatorChar)
                .TrimEnd(Path.AltDirectorySeparatorChar);

            return combinedPath;
        }

        private static string GetPath(string path)
        {
            return FormattableString.Invariant($"{path?
                .Replace("///", "/")
                .Replace("//", "/")
                .Replace("\\\\\\", "\\")
                .Replace("\\\\", "\\")
                .TrimStart(Path.DirectorySeparatorChar)
                .TrimStart(Path.AltDirectorySeparatorChar)
                .TrimEnd(Path.DirectorySeparatorChar)
                .TrimEnd(Path.AltDirectorySeparatorChar)
                .Replace('\\', Path.DirectorySeparatorChar)
                .Replace('/', Path.DirectorySeparatorChar)}{Path.DirectorySeparatorChar}");
        }

        public static DirectoryInfo GetDirectory(params string[] paths)
        {
            var directoryInfo = new DirectoryInfo(Combine(paths));

            return directoryInfo;
        }

        public static FileInfo GetFile(params string[] paths)
        {
            var fileInfo = new FileInfo(Combine(paths));

            return fileInfo;
        }

        public static bool DeleteFileIfExists(params string[] paths)
        {
            var fileInfo = new FileInfo(Combine(paths));

            if (fileInfo.Exists)
            {
                fileInfo.Delete();

                return true;
            }

            return false;
        }

        public static string GetFileDirectoryName(params string[] paths)
        {
            var fileInfo = new FileInfo(Combine(paths));

            return fileInfo.DirectoryName;
        }
    }
}