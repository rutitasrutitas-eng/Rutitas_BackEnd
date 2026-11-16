using Xunit;
using Rutitas.Domain.Entities;
using Rutitas.Application.Interfaces;
namespace Rutitas.ArchitectureTests;
public class LayerTests
{
    [Fact] public void DomainDoesNotReferenceMongoDriver()
    {
        var references=typeof(User).Assembly.GetReferencedAssemblies();
        Assert.DoesNotContain(references,x=>x.Name?.StartsWith("MongoDB",StringComparison.OrdinalIgnoreCase)==true);
    }
    [Fact] public void ApplicationDoesNotReferenceWebFramework()
    {
        var references=typeof(IUserRepository).Assembly.GetReferencedAssemblies();
        Assert.DoesNotContain(references,x=>x.Name?.StartsWith("Microsoft.AspNetCore",StringComparison.OrdinalIgnoreCase)==true);
    }
}
