using System.Threading;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Interfaces;

// ReSharper disable UnusedMember.Global

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;

public interface IDotNetCakeInstaller {
    Task<bool> IsProvenGlobalDotNetCakeInstalledAsync(IErrorsAndInfos errorsAndInfos, CancellationToken cancellationToken);
    Task<bool> DoesGlobalCakeToolVersionMatchTargetFrameworkAsync(bool doNotLogErrorMessage,
        IErrorsAndInfos errorsAndInfos, CancellationToken cancellationToken);
    // ReSharper disable once UnusedMemberInSuper.Global
    Task<bool> IsGlobalDotNetCakeInstalledAsync(string version, IErrorsAndInfos errorsAndInfos, CancellationToken cancellationToken);
    Task InstallOrUpdateGlobalDotNetCakeIfNecessaryAsync(IErrorsAndInfos errorsAndInfos, Inconclusive inconclusive, CancellationToken cancellationToken);
    Task UpdateGlobalDotNetCakeToMatchTargetFrameworkIfNecessaryAsync(IErrorsAndInfos errorsAndInfos, CancellationToken cancellationToken);
}