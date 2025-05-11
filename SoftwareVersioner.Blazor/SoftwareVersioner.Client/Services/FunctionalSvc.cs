using SoftwareVersioner.Shared;

namespace SoftwareVersioner.Client.Services;

public class FunctionalSvc
{
    public static int GetCount(List<PocoDTO>? pocos) 
        => pocos?.Count ?? 0;
}
