using System.Threading.Channels;

namespace ForgeSharp.Results.AspNetCore.Core;

public interface IJobProgress
{
    ValueTask UpdateProgress(string message);
}

internal sealed class JobProgressImpl(string jobToken, Channel<JobProgressUpdate> progressChannel) : IJobProgress
{
    public ValueTask UpdateProgress(string message) => progressChannel.Writer.WriteAsync(new JobProgressUpdate { JobToken = jobToken, Message = message });
}
