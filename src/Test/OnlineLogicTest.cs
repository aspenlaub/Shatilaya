using System;
using System.Net.Http;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Seoa.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Components;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Entities;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test;

[TestClass]
public class OnlineLogicTest {
    private DateTime _TestStartTime, _TestLogTime;
    private const string _onlineActionDescription = "Some online action";

    [TestInitialize]
    public void Initialize() {
        _TestStartTime = DateTime.Now;
        _TestLogTime = _TestStartTime;
    }

    [TestMethod]
    public async Task OnlineChecker_Succeeds() {
        if (!await AreWeOnlineAsync()) {
            Assert.Inconclusive();
        }
        var checker = new OnlineChecker();
        Assert.IsTrue(await checker.AreWeOnlineAsync());
    }

    [TestMethod]
    public async Task OnlineLogic_WithOnlineChecker_Succeeds() {
        if (!await AreWeOnlineAsync()) {
            Assert.Inconclusive();
        }
        var logic = new OnlineLogic(new OnlineChecker());
        var errorsAndInfos = new ErrorsAndInfos();
        await logic.ExecuteOnlineActionWithRetriesAsync(OnlineActionAsync, _onlineActionDescription, errorsAndInfos);
        Assert.That.ThereWereNoErrors(errorsAndInfos);
    }

    [TestMethod]
    public async Task OnlineLogic_WithOnlineAfter7Seconds_Retries() {
        var logic = new OnlineLogic(new OnlineAfterSevenSeconds());
        var errorsAndInfos = new ErrorsAndInfos();
        await logic.ExecuteOnlineActionWithRetriesAsync(LogSecondsAsync, _onlineActionDescription, errorsAndInfos);
        Assert.That.ThereWereNoErrors(errorsAndInfos);
        Assert.Contains(logic.ActionFailureInfo(_onlineActionDescription, 1), errorsAndInfos.Infos);
        Assert.DoesNotContain(logic.ActionFailureInfo(_onlineActionDescription, 2), errorsAndInfos.Infos);
    }

    private async Task LogSecondsAsync(IErrorsAndInfos errorsAndInfos) {
        DateTime now = DateTime.Now;
        double totalSeconds = now.Subtract(_TestStartTime).TotalSeconds;
        if (now.Second != _TestLogTime.Second) {
            _TestLogTime = now;
            errorsAndInfos.Infos.Add($"{totalSeconds} second/-s passed");
        }

        if (totalSeconds <= 6.5) {
            throw new HttpRequestException("Sorry, I have to fail for the time being");
        }

        await Task.CompletedTask;
    }

    private static async Task OnlineActionAsync(IErrorsAndInfos errorsAndInfos) {
        var client = new HttpClient();
        string contents = await client.GetStringAsync("https://www.viperfisch.de/echo");
        if (contents.Contains("This is an echo from viperfisch.de")) {
            return;
        }

        errorsAndInfos.Errors.Add("Action failed");
    }

    private static async Task<bool> AreWeOnlineAsync() {
        try {
            var errorsAndInfos = new ErrorsAndInfos();
            await OnlineActionAsync(errorsAndInfos);
            return !errorsAndInfos.AnyErrors();
        } catch (HttpRequestException) {
            return false;
        }
    }
}
