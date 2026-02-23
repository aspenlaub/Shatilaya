using System;
using System.Net.Http;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Components;

public class OnlineChecker : IOnlineChecker {
    public async Task<bool> AreWeOnlineAsync() {
        var client = new HttpClient();
        try {
            string contents = await client.GetStringAsync("https://www.viperfisch.de/echo");
            return contents.Contains("This is an echo from viperfisch.de");
        } catch (HttpRequestException) {
            return false;
        }
    }
}
