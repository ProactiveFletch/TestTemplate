using SoftwareVersioner.Core;
using SoftwareVersioner.Core.Interfaces;

namespace SoftwareVersioner;

public class DoSomething : IDoSomething
{
    public Task<List<PocoModel>> GetPocos() => Task.FromResult(new List<PocoModel>());
}
