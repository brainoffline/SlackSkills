// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

using System;
using System.Collections.Generic;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

namespace SlackForDotNet
{
    public class SlackAction
    {
        public string     type            { get; set; }
        public string?    action_id       { get; set; }
        public string?    block_action_id { get; set; }
        public string?    block_id        { get; set; }
        public TextBlock? text            { get; set; }
        public string?    value           { get; set; }
        public string?    action_ts       { get; set; }

        // common fields
        private PlainTextBlock?     placeholder { get; set; }
        private ConfirmationDialog? confirm     { get; set; }

        // button
        private string? url { get; set; }

        // static_select
        private BlockOption? initial_option  { get; set; }
        private BlockOption? selected_option { get; set; } // overflow

        // users_select
        private string? selected_user { get; set; }
        private string? initial_user  { get; set; }

        // conversations_select
        private string? selected_conversation { get; set; }
        private string? initial_conversation  { get; set; }

        // channels_select
        private string? selected_channel { get; set; }
        private string? initial_channel  { get; set; }

        // external_select
        private int? min_query_length { get; set; }

        // datepicker
        private string? selected_date { get; set; }
        private string? initial_date  { get; set; }

        // timepicker
        private string? selected_time { get; set; }
        private string? initial_time  { get; set; }

        // multi_static_select
        // multi_external_select
        private List<BlockOption>? initial_options  { get; set; }
        private List<BlockOption>? selected_options { get; set; }

        // multi_users_select
        private List<string>? initial_users  { get; set; }
        private List<string>? selected_users { get; set; }

        // multi_conversations_select
        private List<string>? initial_conversations  { get; set; }
        private List<string>? selected_conversations { get; set; }

        // multi_channels_select
        private List<string>? initial_channels  { get; set; }
        private List<string>? selected_channels { get; set; }
    }
}
