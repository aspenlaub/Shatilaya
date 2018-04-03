using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    public class CakeScriptSettingsChanger {
        public string ChangeCakeScriptSetting(string cakeScript, string setting, bool trueToFalse) {
            var from = trueToFalse ? "true" : "false";
            var to = trueToFalse ? "false" : "true";
            Assert.IsTrue(cakeScript.Contains($"{setting} = {from};"));
            cakeScript = cakeScript.Replace($"{setting} = {from};", $"{setting} = {to};");
            return cakeScript;
        }
    }
}
