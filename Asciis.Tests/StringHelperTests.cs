using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Asciis.Tests
{
    [TestClass]
    public class StringHelperTests
    {
        [TestMethod]
        public void ParseArguments_Null()
        {
            string? str    = null;
            var     result = str!.ParseArguments();

            Assert.IsNull( result );
        }

        [TestMethod]
        public void ParseArguments_Empty()
        {
            var     result = string.Empty.ParseArguments();

            Assert.AreEqual(0, result!.Count);
        }

        [TestMethod]
        public void ParseArguments_Single()
        {
            var result = "single".ParseArguments();

            Assert.AreEqual("single", result![0]);
        }

        [TestMethod]
        public void ParseArguments_SingleSeparatorEnd()
        {
            var result = "single\t".ParseArguments();

            Assert.AreEqual("single", result![0]);
        }

        [TestMethod]
        public void ParseArguments_Multiple()
        {
            var result = "one two three".ParseArguments();

            Assert.AreEqual(3,       result!.Count);
            Assert.AreEqual("three", result[2]);
        }
        
        [TestMethod]
        public void ParseArguments_MultipleSeparators()
        {
            var result = "one\ttwo\tthree".ParseArguments();

            Assert.AreEqual(3,       result!.Count);
            Assert.AreEqual("three", result[2]);
        }

        [TestMethod]
        public void ParseArguments_MultipleLines()
        {
            var result = "one\ntwo\nthree".ParseArguments();

            Assert.AreEqual(3,       result!.Count);
            Assert.AreEqual("three", result[2]);
        }

        [TestMethod]
        public void ParseArguments_SingleQuoted()
        {
            var result = "'single'".ParseArguments();

            Assert.AreEqual("single", result![0]);
        }
        
        [TestMethod]
        public void ParseArguments_MultipleQuoted()
        {
            var result = "'one two three'".ParseArguments();

            Assert.AreEqual("one two three", result![0]);
        }

        [TestMethod]
        public void ParseArguments_MultipleQuotedMultiple()
        {
            var result = "before 'one two three' after".ParseArguments();

            Assert.AreEqual( 3,               result!.Count );
            Assert.AreEqual( "before",        result[ 0 ] );
            Assert.AreEqual( "one two three", result[ 1 ] );
            Assert.AreEqual( "after",         result[ 2 ] );
        }

        [ TestMethod ]
        public void ParseArguments_Bad_SingleQuoted()
        {
            Assert.AreEqual( "single", "'single".ParseArguments()![ 0 ] );
            Assert.AreEqual( "single", "\"single".ParseArguments()![ 0 ] );
        }

        [TestMethod]
        public void ParseArguments_Bad_SingleQuotedEnd()
        {
            Assert.AreEqual("single", "single\'".ParseArguments()![0]);
        }

        [TestMethod]
        public void ParseArguments_MidWordQuoted()
        {
            var result = "sin'gle'".ParseArguments();

            Assert.AreEqual("sin", result![0]);
            Assert.AreEqual("gle", result[1]);
        }




        [TestMethod]
        public void ToParameters_Null()
        {
            OptionOfModel? model = null;

            var dict = new Parameters(model);

            Assert.AreEqual(0, dict.Count);
        }

        [TestMethod]
        public void ToParameters_NoReferenceTypes()
        {
            OptionOfModel model = new();

            var dict = new Parameters(model);

            Assert.IsFalse(dict.ContainsKey("StringValue"));
        }

        [TestMethod]
        public void ToParameters_HasValue()
        {
            var model = new OptionOfModel { StringValue = "" };

            var dict = new Parameters(model);

            Assert.IsTrue(dict.ContainsKey("StringValue"));
        }
    }
}

