using System.Linq;
using System.IO;
using System;
using System.Runtime.InteropServices;

namespace Rampastring.Tools;

/// <summary>
/// Provides safe cross platform Path handling that tolerates mixing multiple absolute paths and mixing path separator styles.
/// </summary>
public static class SafePath
{
    /// <summary>
    /// Safely combines multiple directory paths for all platforms.
    /// The first path can be a relative or absolute path, while all later paths are treated as relative paths.
    /// </summary>
    /// <param name="paths">Ordered list of directory paths.</param>
    /// <returns>The combined directory path of all <paramref name="paths"/>, with trailing separator character and with leading separator character if the path has a root.</returns>
    public static string CombineDirectoryPath(params string[] paths)
    {
        return Combine(true, paths);
    }

    /// <summary>
    /// Safely combines multiple directory paths with a file name for all platforms.
    /// The first path can be a relative or absolute path, while all later paths are treated as relative paths.
    /// </summary>
    /// <param name="paths">Ordered list of directory paths with a file name.</param>
    /// <returns>The combined directory path and file name of all <paramref name="paths"/> with leading separator character if the path has a root.</returns>
    public static string CombineFilePath(params string[] paths)
    {
        return Combine(false, paths);
    }

    /// <summary>
    /// Safely combines multiple directory paths for all platforms.
    /// The first path can be a relative or absolute path.
    /// </summary>
    /// <param name="paths">Ordered list of directory paths.</param>
    /// <returns>A <see cref="DirectoryInfo"/> instance representing the combined directory path of all <paramref name="paths"/>.</returns>
    public static DirectoryInfo GetDirectory(params string[] paths)
    {
        return new DirectoryInfo(CombineDirectoryPath(paths));
    }

    /// <summary>
    /// Safely combines multiple directory paths with a file name for all platforms.
    /// The first path can be a relative or absolute path.
    /// </summary>
    /// <param name="paths">Ordered list of directory paths with a file name.</param>
    /// <returns>A <see cref="FileInfo"/> instance representing the combined directory path and file name of all <paramref name="paths"/>.</returns>
    public static FileInfo GetFile(params string[] paths)
    {
        return new FileInfo(CombineFilePath(paths));
    }

    /// <summary>
    /// Safely delete a file represented by multiple directory paths with a file name for all platforms.
    /// Does not throw an exception if the file or directory path does not exist.
    /// </summary>
    /// <param name="paths">Ordered list of directory paths with a file name.</param>
    public static void DeleteFileIfExists(params string[] paths)
    {
        var fileInfo = new FileInfo(CombineFilePath(paths));

        if (fileInfo.Exists)
            fileInfo.Delete();
    }

    /// <summary>
    /// Safely delete a directory represented by multiple directory paths for all platforms.
    /// Does not throw an exception if the directory path does not exist.
    /// </summary>
    /// <param name="recursive">true to remove directories, subdirectories, and files in path; otherwise, false.</param>
    /// <param name="paths">Ordered list of directory paths.</param>
    public static void DeleteDirectoryIfExists(bool recursive, params string[] paths)
    {
        var directoryInfo = new DirectoryInfo(CombineFilePath(paths));

        if (directoryInfo.Exists)
            directoryInfo.Delete(recursive);
    }

    /// <summary>
    /// Safely retrieves the name of a directory represented by multiple directory paths and a file name for all platforms.
    /// </summary>
    /// <param name="paths">Ordered list of directory paths and a file name.</param>
    /// <returns>The name (not path) of the directory a given file resides in.</returns>
    public static string GetFileDirectoryName(params string[] paths)
    {
        var fileInfo = new FileInfo(CombineFilePath(paths));

        return fileInfo.DirectoryName;
    }

    /// <remarks>
    /// Paths are combined as a base followed by names. Every segment after the first is folded
    /// into the base - it is treated as a name, never as a new root - and both separator styles
    /// are accepted on every platform.
    ///
    /// <code>
    /// CombineFilePath("folder",  "/file")      = "folder/file"
    /// CombineFilePath("folder/", "/file")      = "folder/file"
    /// CombineFilePath("folder/", "file")       = "folder/file"
    /// CombineFilePath("folder1", "C:\folder2") = "folder1/C/folder2" or "folder1/C:/folder2" depending on platform
    /// </code>
    ///
    /// The examples use "/" for readability; the result uses the platform separator ("\" on
    /// Windows, "/" elsewhere), as it always has.
    ///
    /// Folding matches the plain "/" concatenation the client used before this class existed,
    /// where a later segment could never replace the base. It also keeps a call producing the
    /// same shape on Windows and Linux when the segments come from a Windows-authored INI file.
    /// A caller that wants the opposite rule - a rooted later path replaces the base - should
    /// use <see cref="Path.Combine(string[])"/> directly.
    ///
    /// The first segment is the base; it may be relative or absolute and keeps its root. A
    /// directory result ends with a separator, a file result does not. Empty or whitespace-only
    /// segments are ignored, and a call without any segment returns <see langword="null"/>.
    ///
    /// UNC paths are not supported: any segment that starts with two separator characters
    /// (\\server\share, \\?\C:\path, \\.\device, or a POSIX leading double slash) throws
    /// <see cref="ArgumentException"/>.
    /// </remarks>
    private static string Combine(bool isDirectoryPath, params string[] paths)
    {
        if (paths is null)
            return null;

        paths = paths.Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();

        if (!paths.Any())
            return null;

        if (paths.Length == 1)
            paths = new[] { paths[0], null };

        // We intend to treat later segments relative paths, so for the Windows drive-rooted case we treat the drive letter as a name.
        for (int i = 1; i < paths.Length; i++)
            paths[i] = FoldDrivePrefix(paths[i]);

        string combinedPath = paths.Aggregate((x, y) => y is null ? GetPath(x) : Path.Combine(GetPath(x), GetPath(y)))
            .TrimStart([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar])
            .TrimEnd([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar]);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if (paths[0].Length > 1 && paths[0][1] == Path.VolumeSeparatorChar)
                return FormattableString.Invariant($"{combinedPath}{(isDirectoryPath ? Path.DirectorySeparatorChar : null)}");

            return FormattableString.Invariant($"{(Path.IsPathRooted(paths[0]) ? Path.DirectorySeparatorChar : null)}{combinedPath}{(isDirectoryPath ? Path.DirectorySeparatorChar : null)}");
        }

        return FormattableString.Invariant($"{(Path.IsPathRooted(paths[0]) ? Path.DirectorySeparatorChar : null)}{combinedPath}{(isDirectoryPath ? Path.DirectorySeparatorChar : null)}");
    }

    /// <summary>
    /// Turns a drive-rooted path ("C:\folder") into a name ("C/folder") so that
    /// it can be folded into the base like any other later segment. Other paths are
    /// returned unchanged.
    /// </summary>
    private static string FoldDrivePrefix(string path)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return path?.Replace(':', Path.DirectorySeparatorChar);

        return path;
    }

    /// <summary>
    /// Whether a path starts with two separator characters - a UNC path such as
    /// \\server\share, \\?\C:\path or \\.\device, or the POSIX double-slash form.
    /// </summary>
    private static bool IsUncPath(string path)
    {
        if (string.IsNullOrEmpty(path) || path.Length < 2)
            return false;

        return (path[0] == '/' || path[0] == '\\') && (path[1] == '/' || path[1] == '\\');
    }

    private static string GetPath(string path)
    {
        if (IsUncPath(path))
            throw new ArgumentException($"UNC paths are not supported: \"{path}\"");

        // TODO: handle situation of five or more consecutive separators by using a while loop.
        path = path?
            .Replace("///", "/")
            .Replace("//", "/")
            .Replace('/', Path.DirectorySeparatorChar)
            .Replace("\\\\\\", "\\")
            .Replace("\\\\", "\\")
            .Replace('\\', Path.DirectorySeparatorChar)
            .TrimStart([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar])
            .TrimEnd([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar]);

        return FormattableString.Invariant($"{path}{Path.DirectorySeparatorChar}");
    }
}