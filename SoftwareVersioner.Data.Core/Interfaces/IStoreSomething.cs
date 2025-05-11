namespace SoftwareVersioner.Data.Core.Interfaces;

public interface IStoreSomething
{
    Task<List<PocoEntity>> GetPocos();
}
