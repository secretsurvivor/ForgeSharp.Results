namespace ForgeSharp.Results.Integration;

/// <summary>
/// Result-returning wrappers for file operations.
/// </summary>
public static class FileExtension
{
    extension(FileInfo fileInfo)
    {
        /// <summary>
        /// Opens the file as a Result.
        /// </summary>
        /// <param name="fileInfo">The file to open.</param>
        /// <param name="mode">The file mode.</param>
        /// <param name="access">The file access.</param>
        /// <param name="share">The file share mode.</param>
        /// <param name="useAsync">Whether to use asynchronous I/O.</param>
        /// <param name="bufferSize">The buffer size for the stream.</param>
        /// <returns>The file stream, or a failure.</returns>
        public Result<FileStream> OpenAsResult(FileMode mode, FileAccess access, FileShare share, bool useAsync, int bufferSize = 4096)
        {
            try
            {
                return Result.Ok(new FileStream(fileInfo.FullName, mode, access, share, bufferSize, useAsync));
            }
            catch (UnauthorizedAccessException)
            {
                return Result.Fail<FileStream>($"Access denied to file '{fileInfo.FullName}'.");
            }
            catch (Exception ex)
            {
                return Result.Fail<FileStream>(ex);
            }
        }

        /// <summary>
        /// Opens for reading.
        /// </summary>
        /// <param name="fileInfo">The file to open.</param>
        /// <param name="useAsync">Whether to use asynchronous I/O.</param>
        /// <returns>The file stream, or a failure.</returns>
        public Result<FileStream> OpenReadAsResult(bool useAsync = false) => OpenAsResult(fileInfo, FileMode.Open, FileAccess.Read, FileShare.Read, useAsync);

        /// <summary>
        /// Opens for writing.
        /// </summary>
        /// <param name="fileInfo">The file to open.</param>
        /// <param name="useAsync">Whether to use asynchronous I/O.</param>
        /// <returns>The file stream, or a failure.</returns>
        public Result<FileStream> OpenWriteAsResult(bool useAsync = false) => OpenAsResult(fileInfo, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, useAsync);
    }

    extension(File)
    {
        /// <summary>
        /// Opens the file as a Result.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="mode">The file mode.</param>
        /// <param name="access">The file access.</param>
        /// <param name="share">The file share mode.</param>
        /// <param name="useAsync">Whether to use asynchronous I/O.</param>
        /// <param name="bufferSize">The buffer size for the stream.</param>
        /// <returns>The file stream, or a failure.</returns>
        public static Result<FileStream> OpenAsResult(string path, FileMode mode, FileAccess access, FileShare share, bool useAsync, int bufferSize = 4096)
        {
            try
            {
                return Result.Ok(new FileStream(path, mode, access, share, bufferSize, useAsync));
            }
            catch (UnauthorizedAccessException)
            {
                return Result.Fail<FileStream>($"Access denied to file '{path}'.");
            }
            catch (Exception ex)
            {
                return Result.Fail<FileStream>(ex);
            }
        }

        /// <summary>
        /// Deletes the file.
        /// </summary>
        /// <param name="path">The path to the file to delete.</param>
        /// <returns>Success or the failure reason.</returns>
        public static Result DeleteAsResult(string path)
        {
            try
            {
                File.Delete(path);
                return Result.Ok();
            }
            catch (UnauthorizedAccessException)
            {
                return Result.Fail($"Access denied to file '{path}'.");
            }
            catch (Exception ex)
            {
                return Result.Fail(ex);
            }
        }
    }
}
