using System;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackForDotNet
{
    //[ AttributeUsage( AttributeTargets.Class, AllowMultiple = true, Inherited = false ) ]
    //public class SlackBlockAttribute : Attribute
    //{
    //    public string Type { get; }
        
    //    public bool? BlockIdRequired { get; }

    //    public SlackBlockAttribute( string type, bool blockIdRequired = false ) 
    //    { 
    //        Type = type;
    //        BlockIdRequired = blockIdRequired;
    //    }
    //}

    [ AttributeUsage( AttributeTargets.Class, AllowMultiple = true, Inherited = false ) ]
    public class SlackElementAttribute : Attribute
    {
        public string Type { get; }

        public bool? BlockIdRequired { get; }

        public string? SubType { get; }

        public SlackElementAttribute( string type, string? subtype = null, bool blockIdRequired = false )
        {
            Type            = type;
            SubType         = subtype;
            BlockIdRequired = blockIdRequired;
        }
    }
}
