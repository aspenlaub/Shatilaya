using Cake.Core;
using Cake.Frosting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.CakeFrosting;

public class BuildContext(ICakeContext context) : FrostingContext(context) {
    public bool Delay { get; set; } = context.Arguments.HasArgument("delay");
}