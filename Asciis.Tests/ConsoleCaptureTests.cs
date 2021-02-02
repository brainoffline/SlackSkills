using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Asciis.Tests
{
    [TestClass]
    public class ConsoleCaptureTests
    {

        [ TestMethod ]
        public void ConsoleCapture_Disposable()
        {
            using var console = new ConsoleCapture();

            Console.Write("Simple Test");

            Assert.AreEqual("Simple Test", console.StringBuilder.ToString());
        }

        [TestMethod]
        public void ConsoleCapture_Next()
        {
            using var console = new ConsoleCapture();

            Console.Write("First");
            var first = console.NextString();

            Console.Write("Second");
            var second = console.NextString();

            Assert.AreEqual("First", first);
            Assert.AreEqual("Second", second);
        }

    }
}
