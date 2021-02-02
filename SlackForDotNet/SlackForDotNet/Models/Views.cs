using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using Newtonsoft.Json;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized

namespace SlackForDotNet
{
    /// <summary>
    /// Possible values: home, modal
    /// </summary>
    [JsonConverter(typeof(SlackBlockConverter))]
    public class View : Block
    {
        public List< Block >? blocks           { get; set; }
        public string?        private_metadata { get; set; }
        public string?        callback_id      { get; set; }
        public string?        external_id      { get; set; }
        public string?        root_view_id     { get; set; }
        public string?        app_id           { get; set; }
        public string?        bot_id           { get; set; }
        public string?        team_id          { get; set; }
        public State?         state            { get; set; }
        public string?        hash             { get; set; }

        public View Add( Block block )
        {
            blocks ??= new List< Block >();
            blocks.Add( block );

            return this;
        }

        public class State
        {
            public object values { get; set; }
        }
    }

    [ SlackBlock( "home" ) ]
    public class HometabView : View { }

    [ SlackBlock( "modal" ) ]
    public class ModalView : View
    {
        public PlainTextBlock title { get; set; }
        public PlainTextBlock? close { get; set; }
        public PlainTextBlock? submit { get; set; }
        public bool? clear_on_close { get; set; }
        public bool? notify_on_close { get; set; }
        public bool? submit_disabled { get; set; }  // Primary for configuration modals
    }
}
