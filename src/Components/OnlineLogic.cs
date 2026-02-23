using System;
using System.Net.Http;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Entities;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Interfaces;
using LibGit2Sharp;
using NuGet.Packaging;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Components;

public class OnlineLogic(IOnlineChecker onlineChecker) : IOnlineLogic {
    public async Task ExecuteOnlineActionWithRetriesAsync(Func<IErrorsAndInfos, Task> asyncAction,
            string actionDescription, IErrorsAndInfos errorsAndInfos) {
        int attempts = 0;
        DateTime timeToGiveUp = DateTime.Now.AddMinutes(10);
        do {
            attempts++;
            try {
                var tryErrorsAndInfos = new ErrorsAndInfos();
                await asyncAction(tryErrorsAndInfos);
                errorsAndInfos.Errors.AddRange(tryErrorsAndInfos.Errors);
                errorsAndInfos.Infos.AddRange(tryErrorsAndInfos.Infos);
                return;
            } catch (HttpRequestException) {
                errorsAndInfos.Infos.Add(ActionFailureInfo(actionDescription, attempts));
            } catch (LibGit2SharpException) {
                errorsAndInfos.Infos.Add(ActionFailureInfo(actionDescription, attempts));
            } catch (Exception e) {
                errorsAndInfos.Errors.Add(e.GetType().Name);
            }

            do {
                await Task.Delay(TimeSpan.FromSeconds(1));
            } while (DateTime.Now < timeToGiveUp && !await onlineChecker.AreWeOnlineAsync());
        } while (DateTime.Now < timeToGiveUp);

        errorsAndInfos.Errors.Add("Online action failed");
    }

    public string ActionFailureInfo(string actionDescription, int attempts) {
        return $"Error executing '{actionDescription}' (attempt #{attempts})";
    }
}
