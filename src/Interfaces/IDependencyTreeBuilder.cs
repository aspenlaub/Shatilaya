namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface IDependencyTreeBuilder {
        IDependencyNode BuildDependencyTree(string packagesFolder);
    }
}
