using System.Buffers.Text;
using System.Security.Cryptography;
using System.Threading.Channels;

namespace ForgeSharp.Results.AspNetCore.Core;

public sealed class LongRunningUtility(Channel<LongRunningRequest> requestChannel, Channel<JobProgressUpdate> progressChannel)
{
    public static void ProduceToken(out string securityToken) => securityToken = GenerateToken();

    public IJobProgress ProduceTokenWithProgress(out string securityToken)
    {
        securityToken = GenerateToken();
        return new JobProgressImpl(securityToken, progressChannel);
    }

    public async ValueTask<ResultEndpoint> AttachPipeline(LongRunningRequest request)
    {
        await requestChannel.Writer.WriteAsync(request);
        return new ResultEndpoint(new LongRunningResponse { JobToken = request.JobToken });
    }

    public async ValueTask<ResultEndpoint<T>> AttachPipeline<T>(LongRunningRequest request)
    {
        await requestChannel.Writer.WriteAsync(request);
        return new ResultEndpoint<T>(new LongRunningResponse { JobToken = request.JobToken });
    }

    public async ValueTask<ResultEndpoint<T, TError>> AttachPipeline<T, TError>(LongRunningRequest request)
    {
        await requestChannel.Writer.WriteAsync(request);
        return new ResultEndpoint<T, TError>(new LongRunningResponse { JobToken = request.JobToken });
    }

    private static string GenerateToken()
    {
        byte[] bytes = RandomNumberGenerator.GetBytes(32);
        return Base64Url.EncodeToString(bytes);
    }
}
