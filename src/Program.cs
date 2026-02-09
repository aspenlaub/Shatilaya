using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya;

public static class Program {
    public static int Main(string[] args) {
        return new CakeHost()
            .UseContext<ShatilayaContext>()
            .Run(args);
    }
}