using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace OptionTests
{
    using ObjectUtility;

    [TestClass]
    public class OptionTest
    {
        [TestMethod]
        public void TestMakeFunction()
        {
            var none = Options.make<string>(() => null);
            Assert.IsInstanceOfType(none, typeof(Options.None<string>));
            Assert.IsFalse(none.IsAvailable);
            var some = Options.make(() => "This" + " is" + "test.");
            Assert.IsInstanceOfType(some, typeof(Options.Some<string>));
            Assert.IsTrue(some.IsAvailable);
            string s = null;
            none = Options.make<string>(() =>
            {
                if (s == null) {
                    throw new Exception();
                } else {
                    return s;
                }
            });
            Assert.IsInstanceOfType(none, typeof(Options.None<string>));
            Assert.IsFalse(none.IsAvailable);
        }
    }
}
