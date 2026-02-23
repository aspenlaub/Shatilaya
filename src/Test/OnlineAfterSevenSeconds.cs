using System;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test;

public class OnlineAfterSevenSeconds : IOnlineChecker {
    private readonly DateTime _OnlineFrom = DateTime.Now.AddSeconds(7);

    public async Task<bool> AreWeOnlineAsync() {
        return await Task.FromResult(DateTime.Now >= _OnlineFrom);
    }
}
