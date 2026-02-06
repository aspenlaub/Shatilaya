using Cake.Core;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting;

public static class Program {
    public static int Main(string[] args) {
        return new CakeHost()
            .UseContext<BuildContext>()
            .Run(args);
    }
}

public class BuildContext(ICakeContext context) : FrostingContext(context) {
    public bool Delay { get; set; } = context.Arguments.HasArgument("delay");
}