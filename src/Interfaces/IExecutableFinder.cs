namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces {
    public interface IExecutableFinder {
        string FindMsTestExe(int toolsVersionNumber);
        string FindVsTestExe(int toolsVersionNumber);
        bool HaveVs7();
    }
}
