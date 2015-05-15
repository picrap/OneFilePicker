#region OneFilePicker
// OneFilePicker
// Abstract (extensible) file picker
// https://github.com/picrap/OneFilePicker
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace OneFilePickerTest
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OneFilePicker.File.Default;

    [TestClass]
    public class FileNodeProviderTest
    {
        [TestMethod]
        public void LocalPathParseTest()
        {
            var path = FileNodeProvider.Parse(@"C:\Windows");
            Assert.AreEqual(3, path.Length);
            Assert.AreEqual("", path[0]);
            Assert.AreEqual("C:", path[1]);
            Assert.AreEqual("Windows", path[2]);
        }
    }
}
