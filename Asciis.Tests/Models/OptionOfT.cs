using System;
using System.Collections.Generic;
using System.ComponentModel;

using JetBrains.Annotations;

namespace Asciis.Tests
{
    public enum OptionOfModes
    {
        NotSet,
        First,
        Second,
        Last
    }

    [Command("OptionOfT", Description = "Test class for testing different parameters")]
    public class OptionOfModel
    {
        [ Param( "f|flag", "Set a flag value" ) ]
        public bool Flag { get; set; }

        [ Param( "i|int", "Set an integer value" ) ]
        public int IntValue { get; set; }

        [ Param( "int16", "Set an integer value" ) ]
        public Int16 Int16Value { get; set; }

        [ Param( "int32", "Set an integer value" ) ]
        public Int32 Int32Value { get; set; }

        [ Param( "int64", "Set an integer value" ) ]
        public Int64 Int64Value { get; set; }

        [ Param( "float|single", "Set a float (single) value" ) ]
        public float FloatValue { get; set; }

        [ Param( "d|double", "Set a double value" ) ]
        public double DoubleValue { get; set; }

        [ Param( "decimal", "Set a decimal value" ) ]
        public decimal DecimalValue { get; set; }

        [ Param( "s|string", "Set a single string value" ) ]
        public string? StringValue { get; set; }

        [ Param( "date", "Set a date time value" ) ]
        public DateTime DateValue { get; set; }

        [ Param( "e|enum", "Set an enum value" ) ]
        public OptionOfModes EnumValue { get; set; }

        [ Param( "l|list", "Add value to List" ) ]
        public List< string >? ListValue { get; set; }

        [Action( "c|cmd", "Call a command")]
        public void Command()
        {
            StringValue = "Command called";
        }
        
        [ UsedImplicitly ]
        [ Action( "doit", "Call the DoIt command", DefaultAction = true ) ]
        public void DoItCommand(List<string> extras)
        {
            if (extras.Count > 0)
                StringValue = "Executed: " + string.Join(' ', extras);
            else
                StringValue = "Executed";

        }
    }
}
