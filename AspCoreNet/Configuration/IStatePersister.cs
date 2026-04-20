namespace ForgeSharp.Results.AspNetCore.Configuration;

public interface IStatePersister
{
    ICapturedState Capture();
}

public interface ICapturedState
{
    void Restore(IServiceProvider provider);
}
