using JinnDev.Utilities.Monad;

namespace SoftwareVersioner.Client.Services;

public interface IHttpSvc
{
    Task<Maybe<string>> ProcessNameAsync(string? name);
}
