using System.Threading.Tasks;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;

public interface IOnlineChecker {
    Task<bool> AreWeOnlineAsync();
}
