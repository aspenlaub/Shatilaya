using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting;

public static class Program {
    public static int Main(string[] args) {
        return new CakeHost()
            .UseContext<BuildContext>()
            .Run(args);
    }
}