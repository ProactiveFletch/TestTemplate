using JinnDev.Utilities.Monad;
using SoftwareVersioner.Core;

namespace SoftwareVersioner.Client.Services;

public interface IHttpSvc
{
    Task<Maybe<List<Software>>> GetAllSoftwareAsync();
}
