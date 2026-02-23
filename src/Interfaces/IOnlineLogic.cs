using System;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;

public interface IOnlineLogic {
    Task ExecuteOnlineActionWithRetriesAsync(Func<IErrorsAndInfos, Task> asyncAction,
        string actionDescription, IErrorsAndInfos errorsAndInfos);

    string ActionFailureInfo(string actionDescription, int attempts);
}
