namespace SoftwareVersioner.Core.Interfaces;

public interface ISoftwareManager
{
    IEnumerable<Software> GetAllSoftware();
}