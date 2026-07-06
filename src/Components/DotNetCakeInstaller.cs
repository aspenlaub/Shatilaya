using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Gitty.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Interfaces;

[assembly: InternalsVisibleTo("Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test")]

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Components;

public class DotNetCakeInstaller : IDotNetCakeInstaller {
    private const string _cakeToolId = "cake.tool";
    private const string _veryOldCakeToolVersion = "4.0.0";
    private const string _oldCakeToolVersion = "5.0.0";
    internal const string CakeToolVersionMatchingCompiledTargetFramework = "6.0.0";
    private const string _runnerUpCakeToolVersion = "7.0.0";
    internal const string ProvenCakeToolVersion = "6.0.0";
    private const string _dotNetExecutableFileName = "dotnet";
    private const string _dotNetToolListArguments = "tool list --global";
    private const string _dotNetInstallCakeToolArguments = "tool install Cake.Tool --version "
        + CakeToolVersionMatchingCompiledTargetFramework + " --global";
    private const string _dotNetUpdateCakeToolArguments = "tool update Cake.Tool --version "
        + CakeToolVersionMatchingCompiledTargetFramework + " --global";
    private const string _dotNetUninstallCakeToolArguments = "tool uninstall Cake.Tool --global";
    private const string _dotNetInstallProvenCakeToolArguments = "tool install Cake.Tool --version "
        + ProvenCakeToolVersion + " --global";
    private const string _dotNetInstallCakeToolMatchingTargetFrameworkArguments
        = "tool install Cake.Tool --version "
            + CakeToolVersionMatchingCompiledTargetFramework + " --global";

    private readonly IProcessRunner _ProcessRunner;
    private readonly IFolder _WorkingFolder;

    public DotNetCakeInstaller(IProcessRunner processRunner) {
        _ProcessRunner = processRunner;
        _WorkingFolder = new Folder(Path.GetTempPath()).SubFolder(nameof(DotNetCakeInstaller));
        _WorkingFolder.CreateIfNecessary();
    }

    public async Task<bool> IsProvenGlobalDotNetCakeInstalledAsync(IErrorsAndInfos errorsAndInfos, CancellationToken cancellationToken) {
        return await IsGlobalDotNetCakeInstalledAsync(ProvenCakeToolVersion, errorsAndInfos, cancellationToken);
    }

    public async Task<bool> DoesGlobalCakeToolVersionMatchTargetFrameworkAsync(bool doNotLogErrorMessage,
            IErrorsAndInfos errorsAndInfos, CancellationToken cancellationToken) {
        if (await IsGlobalDotNetCakeInstalledAsync(CakeToolVersionMatchingCompiledTargetFramework, errorsAndInfos, cancellationToken)) {
            return true;
        }
        if (doNotLogErrorMessage) { return false; }

        errorsAndInfos.Errors.Add(
            string.Format("The global dotnet cake version must be {0}",
                CakeToolVersionMatchingCompiledTargetFramework)
        );
        return false;
    }

    public async Task<bool> IsGlobalDotNetCakeInstalledAsync(string version, IErrorsAndInfos errorsAndInfos, CancellationToken cancellationToken) {
        await _ProcessRunner.RunProcessAsync(_dotNetExecutableFileName, _dotNetToolListArguments, _WorkingFolder, errorsAndInfos, cancellationToken);
        if (errorsAndInfos.AnyErrors()) { return false; }

        string line = errorsAndInfos.Infos.LastOrDefault(l => l.StartsWith(_cakeToolId));
        return line?.Substring(_cakeToolId.Length).TrimStart().StartsWith(version) == true;
    }

    public async Task InstallOrUpdateGlobalDotNetCakeIfNecessaryAsync(IErrorsAndInfos errorsAndInfos, Inconclusive inconclusive, CancellationToken cancellationToken) {
        inconclusive.IsInconclusive = false;
        if (await IsGlobalDotNetCakeInstalledAsync(CakeToolVersionMatchingCompiledTargetFramework, errorsAndInfos, cancellationToken)) {
            await RestoreProvenCakeToolVersionAsync(errorsAndInfos, cancellationToken);
            return;
        }
        if (errorsAndInfos.AnyErrors()) { return; }

        // ReSharper disable once RedundantAssignment
        bool isOldCakeToolVersionInstalled =
            await IsGlobalDotNetCakeInstalledAsync(_veryOldCakeToolVersion, errorsAndInfos, cancellationToken)
            || await IsGlobalDotNetCakeInstalledAsync(_oldCakeToolVersion, errorsAndInfos, cancellationToken);
        if (errorsAndInfos.AnyErrors()) { return; }

        if (await IsGlobalDotNetCakeInstalledAsync(_runnerUpCakeToolVersion, errorsAndInfos, cancellationToken)
            || CakeToolVersionMatchingCompiledTargetFramework != ProvenCakeToolVersion) {
            if (errorsAndInfos.AnyErrors()) { return; }

            bool skipTest;
            try {
                await _ProcessRunner.RunProcessAsync(_dotNetExecutableFileName, _dotNetUninstallCakeToolArguments,
                    _WorkingFolder, errorsAndInfos, cancellationToken);
                skipTest = errorsAndInfos.AnyErrors();
            } catch {
                skipTest = true;
            }
            if (skipTest) {
                inconclusive.IsInconclusive = true;
                errorsAndInfos.Infos.Clear();
                errorsAndInfos.Errors.Clear();
                return;
            }
        }

        await _ProcessRunner.RunProcessAsync(_dotNetExecutableFileName,
              // ReSharper disable once ConditionIsAlwaysTrueOrFalse
              isOldCakeToolVersionInstalled
                  ? _dotNetUpdateCakeToolArguments
                  : _dotNetInstallCakeToolArguments,
              _WorkingFolder, errorsAndInfos, cancellationToken);
        if (errorsAndInfos.AnyErrors()) { return; }

        if (!await IsGlobalDotNetCakeInstalledAsync(CakeToolVersionMatchingCompiledTargetFramework, errorsAndInfos, cancellationToken)) {
            errorsAndInfos.Errors.Add("Could not install cake tool");
        }

        await RestoreProvenCakeToolVersionAsync(errorsAndInfos, cancellationToken);
    }

    public async Task UpdateGlobalDotNetCakeToMatchTargetFrameworkIfNecessaryAsync(IErrorsAndInfos errorsAndInfos, CancellationToken cancellationToken) {
        if (await DoesGlobalCakeToolVersionMatchTargetFrameworkAsync(true, errorsAndInfos, cancellationToken)) {
            return;
        }
        if (errorsAndInfos.AnyErrors()) { return; }

        await _ProcessRunner.RunProcessAsync(_dotNetExecutableFileName, _dotNetUninstallCakeToolArguments,
            _WorkingFolder, errorsAndInfos, cancellationToken);
        await _ProcessRunner.RunProcessAsync(_dotNetExecutableFileName, _dotNetInstallCakeToolMatchingTargetFrameworkArguments,
            _WorkingFolder, errorsAndInfos, cancellationToken);
    }

    private async Task RestoreProvenCakeToolVersionAsync(IErrorsAndInfos errorsAndInfos, CancellationToken cancellationToken) {
        if (await IsGlobalDotNetCakeInstalledAsync(ProvenCakeToolVersion, errorsAndInfos, cancellationToken)) {
            return;
        }

        await _ProcessRunner.RunProcessAsync(_dotNetExecutableFileName, _dotNetUninstallCakeToolArguments,
                _WorkingFolder, errorsAndInfos, cancellationToken);
        await _ProcessRunner.RunProcessAsync(_dotNetExecutableFileName, _dotNetInstallProvenCakeToolArguments,
                _WorkingFolder, errorsAndInfos, cancellationToken);
    }
}