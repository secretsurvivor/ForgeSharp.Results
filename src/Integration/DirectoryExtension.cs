namespace ForgeSharp.Results.Integration;

/// <summary>
/// Result-returning wrappers for directory operations.
/// </summary>
public static class DirectoryExtension
{
    extension(DirectoryInfo directoryInfo)
    {
        /// <summary>
        /// Creates the directory.
        /// </summary>
        /// <param name="directoryInfo">The directory to create.</param>
        /// <returns>Success or the failure reason.</returns>
        public Result CreateAsResult()
        {
            try
            {
                directoryInfo.Create();
                return Result.Ok();
            }
            catch (UnauthorizedAccessException)
            {
                return Result.Fail($"Access denied to directory '{directoryInfo.FullName}'.");
            }
            catch (Exception ex)
            {
                return Result.Fail(ex);
            }
        }

        /// <summary>
        /// Deletes the directory.
        /// </summary>
        /// <param name="directoryInfo">The directory to delete.</param>
        /// <returns>Success or the failure reason.</returns>
        public Result DeleteAsResult()
        {
            try
            {
                directoryInfo.Delete();
                return Result.Ok();
            }
            catch (UnauthorizedAccessException)
            {
                return Result.Fail($"Access denied to directory '{directoryInfo.FullName}'.");
            }
            catch (Exception ex)
            {
                return Result.Fail(ex);
            }
        }
    }

    extension(Directory)
    {
        /// <summary>
        /// Creates a directory at the given path.
        /// </summary>
        /// <param name="path">The path of the directory to create.</param>
        /// <returns>The created <see cref="DirectoryInfo"/>, or a failure.</returns>
        public static Result<DirectoryInfo> CreateAsResult(string path)
        {
            try
            {
                return Result.Ok(Directory.CreateDirectory(path));
            }
            catch (UnauthorizedAccessException)
            {
                return Result.Fail<DirectoryInfo>($"Access denied to directory '{path}'");
            }
            catch (Exception ex)
            {
                return Result.Fail<DirectoryInfo>(ex);
            }
        }

        /// <summary>
        /// Deletes a directory at the given path.
        /// </summary>
        /// <param name="path">The path of the directory to delete.</param>
        /// <returns>Success or the failure reason.</returns>
        public static Result DeleteAsResult(string path)
        {
            try
            {
                Directory.Delete(path);
                return Result.Ok();
            }
            catch (UnauthorizedAccessException)
            {
                return Result.Fail($"Access denied to directory '{path}'");
            }
            catch (Exception ex)
            {
                return Result.Fail(ex);
            }
        }
    }
}
