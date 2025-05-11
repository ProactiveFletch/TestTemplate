using SoftwareVersioner.Client.Services;

namespace SoftwareVersioner.Client.Tests;

public class FunctionalTests
{
    [Fact]
    public void GetCount_NoData_ZeroResult()
    {
        var result = FunctionalSvc.GetCount([]);
        Assert.Equal(0, result);
    }
}
