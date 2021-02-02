using System;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackForDotNet.WebApiContracts
{
    /*
    public class MessageRequest : EventMessage
    {
        public string token { get; set; }
    }

    public class MessageResponse : EventResponse
    {
    }
    */

    [ SlackMessage( "api.test", apiType: Msg.UserToken ) ]
    public class ApiTestRequest : SlackMessage { }
}
