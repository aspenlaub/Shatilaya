using System.IO;
using System.Linq;
using System.Reflection;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test {
    public class GitTestUtilities {
        /// <summary>
        /// When tests are executed using cake, the lib sub folder does not make it to the working directory,
        /// so the execution fails and reports that a git2 assembly cannot be found
        /// </summary>
        public static void MakeSureGit2AssembliesAreInPlace() {
            var folder = Directory.GetCurrentDirectory();
            var zipStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Aspenlaub.Net.GitHub.CSharp.Shatilaya.Test.lib.zip");
            if (Directory.Exists(folder + @"\lib")) {

                /* Folder exists => make sure the file names are consistent with the embedded zip file, otherwise the lib.zip resource needs to be updated */
                var zipFile = new ZipFile(zipStream);
                foreach (var zipEntry in zipFile.Cast<ZipEntry>().Where(zipEntry => zipEntry.IsFile)) {
                    Assert.IsTrue(File.Exists(folder + @"\" + zipEntry.Name.Replace('/', '\\')));
                }
            } else {

                /* Folder exists => create it from the embedded zip file */
                var fastZip = new FastZip();
                fastZip.ExtractZip(zipStream, folder, FastZip.Overwrite.Never, s => { return true; }, null, null, true,
                    true);
                Assert.IsTrue(Directory.Exists(folder + @"\lib"));
            }
        }
    }
}
