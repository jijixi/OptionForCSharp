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
            Assert.AreEqual(default(string), none.value);
            var some = Options.make(() => "This" + " is" + " test.");
            Assert.IsInstanceOfType(some, typeof(Options.Some<string>));
            Assert.IsTrue(some.IsAvailable);
            Assert.AreEqual("This is test.", some.value);

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

        [TestMethod]
        public void TestLinqQuery()
        {
            try {
                var result = from s in Options.make(() => "foo")
                             select (s + "bar");
                Assert.AreEqual("foobar", result.First());
            } catch (AssertFailedException) {
                throw;
            } catch (Exception e) {
                Assert.Fail("should not be thrown any exceptions, but thrown {0}", e.ToString());
            }

            bool shouldThrow = true;
            Func<string, string> f = (s) =>
            {
                if (shouldThrow) {
                    throw new NullReferenceException();
                }
                return null;
            };
            var none = from s in Options.make(() => "foo")
                       select f(s);
            Assert.IsTrue(none is Options.None<string>);
        }

        [TestMethod]
        public void TestLinqMethod()
        {
            try {
                var result = Options.make(() => "foo")
                    .Select(s => s + "bar")
                    .Select(s => s + "baz");
                Assert.AreEqual("foobarbaz", result.First());
            } catch (AssertFailedException) {
                throw;
            } catch (Exception e) {
                Assert.Fail("should not be thrown any exceptions, but thrown {0}", e.ToString());
            }
        }

        [TestMethod]
        public void TestIgnoreWhenNone()
        {
            bool shouldThrow = true;
            var result = Options.make(() => "")
                .Select(s => "test start")
                .Select(s => {
                    if (shouldThrow) {
                        throw new NullReferenceException();
                    }
                    return "foo";
                })
                .Select(s => { Assert.Fail("This assertion should not be checked."); return "bar"; })
                .Select(s => { Assert.Fail("This assertion should not be checked."); return "baz"; });
            Assert.IsNotNull(result);
            InvalidOperationException noValue = null;
            try {
                var none = result.First();
            } catch (InvalidOperationException e) {
                noValue = e;
            }
            Assert.IsNotNull(noValue);
        }
    }
}
