namespace ForgeSharp.Results.Integration;

/// <summary>
/// Provides extension methods for <see cref="DirectoryInfo"/> and <see cref="Directory"/> to wrap directory operations in <see cref="Result"/> types.
/// </summary>
public static class DirectoryExtension
{
    extension(DirectoryInfo directoryInfo)
    {
        /// <summary>
        /// Creates a directory and returns a <see cref="Result"/> indicating success or failure.
        /// </summary>
        /// <param name="directoryInfo">The directory to create.</param>
        /// <returns>A result indicating whether the directory was created successfully.</returns>
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
        /// Deletes a directory and returns a <see cref="Result"/> indicating success or failure.
        /// </summary>
        /// <param name="directoryInfo">The directory to delete.</param>
        /// <returns>A result indicating whether the directory was deleted successfully.</returns>
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
        /// Creates a directory at the specified path and returns a <see cref="Result{T}"/> containing the created <see cref="DirectoryInfo"/>.
        /// </summary>
        /// <param name="path">The path of the directory to create.</param>
        /// <returns>A result containing the created directory information if successful, otherwise a failed result.</returns>
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
        /// Deletes a directory at the specified path and returns a <see cref="Result"/> indicating success or failure.
        /// </summary>
        /// <param name="path">The path of the directory to delete.</param>
        /// <returns>A result indicating whether the directory was deleted successfully.</returns>
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
