namespace SoftwareVersioner.Core.Interfaces;

public interface IDoSomething
{
    Task<List<PocoModel>> GetPocos();
}
