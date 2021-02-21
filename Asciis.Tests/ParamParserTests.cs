using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using static Asciis.Tests.Helpers.TestHelper;

namespace Asciis.Tests
{
    [ TestClass ]
    public class ParamParserTests
    {
        private readonly ParamParser< OptionOfModel > _pp = new();
        private          bool                         _updated;

        [ TestInitialize ]
        public void Setup() { }

        [ TestMethod ]
        public void TestNegatives()
        {
            var obj = new OptionOfModel();

            // Case sensitive
            AssertException( typeof(ArgumentException), "Parameter [Flag] has not been defined",   _pp, pp => pp.CallParam( obj, "Flag",   "true" ) );
            AssertException( typeof(ArgumentException), "Parameter [Int] has not been defined",    _pp, pp => pp.CallParam( obj, "Int",    "1" ) );
            AssertException( typeof(ArgumentException), "Parameter [Float] has not been defined",  _pp, pp => pp.CallParam( obj, "Float",  "1.0" ) );
            AssertException( typeof(ArgumentException), "Parameter [String] has not been defined", _pp, pp => pp.CallParam( obj, "String", "str" ) );
            AssertException( typeof(ArgumentException), "Parameter [Enum] has not been defined",   _pp, pp => pp.CallParam( obj, "Enum",   "First" ) );
            AssertException( typeof(ArgumentException), "Parameter [List] has not been defined",   _pp, pp => pp.CallParam( obj, "List",   "list" ) );
        }

        [ TestMethod ]
        public void TestBoolean()
        {
            var obj = new OptionOfModel();
            _updated = _pp.CallParam( obj, "flag", "true" );
            Assert.IsTrue( _updated );
            Assert.IsTrue( obj.Flag );

            _updated = _pp.CallParam( obj, "f", "False" );
            Assert.IsTrue( _updated );
            Assert.IsFalse( obj.Flag );
        }

        [ TestMethod ]
        public void TestInt()
        {
            var obj = new OptionOfModel();

            _updated = _pp.CallParam( obj, "int", "1" );
            Assert.IsTrue( _updated );
            Assert.AreEqual( 1, obj.IntValue );

            _updated = _pp.CallParam( obj, "i", "2" );
            Assert.IsTrue( _updated );
            Assert.AreEqual( 2, obj.IntValue );

            _updated = _pp.CallParam( obj, "int16", Int16.MinValue.ToString() );
            Assert.IsTrue( _updated );
            Assert.AreEqual( Int16.MinValue, obj.Int16Value );

            _updated = _pp.CallParam( obj, "int32", Int32.MaxValue.ToString() );
            Assert.IsTrue( _updated );
            Assert.AreEqual( Int32.MaxValue, obj.Int32Value );

            _updated = _pp.CallParam( obj, "int64", Int64.MaxValue.ToString() );
            Assert.IsTrue( _updated );
            Assert.AreEqual( Int64.MaxValue, obj.Int64Value );
        }

        [ TestMethod ]
        public void TestNumbers()
        {
            var obj = new OptionOfModel();

            _updated = _pp.CallParam( obj, "float", "1.0" );
            Assert.IsTrue( _updated );
            Assert.AreEqual( 1.0f, obj.FloatValue );

            _updated = _pp.CallParam( obj, "double", "1.1" );
            Assert.IsTrue( _updated );
            Assert.AreEqual( 1.1d, obj.DoubleValue );

            _updated = _pp.CallParam( obj, "decimal", "1.2" );
            Assert.IsTrue( _updated );
            Assert.AreEqual( 1.2m, obj.DecimalValue );
        }


        [ TestMethod ]
        public void TestString()
        {
            var obj = new OptionOfModel();

            _updated = _pp.CallParam( obj, "string", "1" );
            Assert.IsTrue( _updated );
            Assert.AreEqual( "1", obj.StringValue );

            _updated = _pp.CallParam( obj, "s", "2" );
            Assert.IsTrue( _updated );
            Assert.AreEqual( "2", obj.StringValue );
        }

        [ TestMethod ]
        public void TestDateTime()
        {
            var dt  = DateTime.Now;
            var obj = new OptionOfModel();

            _updated = _pp.CallParam( obj, "date", dt.ToString( "O" ) );
            Assert.IsTrue( _updated );
            Assert.AreEqual( dt, obj.DateValue );
        }

        [ TestMethod ]
        public void TestEnum()
        {
            var obj = new OptionOfModel();

            _updated = _pp.CallParam( obj, "enum", "First" );
            Assert.IsTrue( _updated );
            Assert.AreEqual( OptionOfModes.First, obj.EnumValue );

            _updated = _pp.CallParam( obj, "e", "Second" );
            Assert.IsTrue( _updated );
            Assert.AreEqual( OptionOfModes.Second, obj.EnumValue );
        }

        [ TestMethod ]
        public void TestList()
        {
            var obj = new OptionOfModel();

            _updated = _pp.CallParam( obj, "list", "First" );
            Assert.IsTrue( _updated );
            Assert.AreEqual( "First", obj.ListValue![ 0 ] );

            _updated = _pp.CallParam( obj, "l", "Second" );
            Assert.IsTrue( _updated );
            Assert.AreEqual( "Second", obj.ListValue[ 1 ] );
        }

        [ TestMethod ]
        public void TestCommandLines()
        {
            var obj = new OptionOfModel();

            _pp.ParseArguments( obj,
                               "",
                               new List< string >
                               {
                                   "-flag",
                                   "--int",
                                   "1",
                                   "/int16",
                                   "2",
                                   "-int32=3",
                                   "--int64:4",
                                   "/enum=Second"
                               } );

            Assert.IsTrue( obj.Flag );
            Assert.AreEqual( 1,                    obj.IntValue );
            Assert.AreEqual( 2,                    obj.Int16Value );
            Assert.AreEqual( 3,                    obj.Int32Value );
            Assert.AreEqual( 4,                    obj.Int64Value );
            Assert.AreEqual( OptionOfModes.Second, obj.EnumValue );

            Assert.AreEqual( "Executed", obj.StringValue );
        }

        [ TestMethod ]
        public void TestEmptyCommandLine()
        {
            var obj = new OptionOfModel();

            _pp.ParseArguments( obj, "", new List< string >() );
        }

        [ TestMethod ]
        public void TextExtras()
        {
            var obj = new OptionOfModel();

            _pp.ParseArguments( obj, "", new List< string > { "cmd" } ); // No extras
            Assert.IsNull( obj.ListValue );

            _pp.ParseArguments( obj, "", new List< string > { "cmd", "Extra", "Text" } );
            Assert.AreEqual( "Command called", obj.StringValue );

            _pp.ParseArguments( obj, "", new List< string > { "doit", "Extra", "Text" } );
            Assert.AreEqual( "Executed: Extra Text", obj.StringValue );
        }

        [ TestMethod ]
        public void TextHelp()
        {
            var obj = new OptionOfModel();

            var result = _pp.ParseArguments(obj, "?", null);
            Assert.IsFalse( result );
        }
    }
}
