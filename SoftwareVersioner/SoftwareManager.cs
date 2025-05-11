using SoftwareVersioner.Core;
using SoftwareVersioner.Core.Interfaces;

namespace SoftwareVersioner;

public class SoftwareManager : ISoftwareManager
{
    public IEnumerable<Software> GetAllSoftware()
    {
        return
            [
                new() { Name = "MS Word", Version = "13.2.1" },
                new() { Name = "AngularJS", Version = "1.7.1" },
                new() { Name = "Angular", Version = "13" },
                new() { Name = "React", Version = "0.0.5" },
                new() { Name = "Vue.js", Version = "2.6" },
                new() { Name = "Visual Studio", Version = "17.0.31919.166.0" },
                new() { Name = "Visual Studio", Version = "16.11.9.3.55" },
                new() { Name = "Visual Studio Code", Version = "1.63" },
                new() { Name = "Blazor", Version = "3.2.0" }
            ];
    }
}
