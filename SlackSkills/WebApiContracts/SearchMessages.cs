using System;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackSkills.WebApiContracts
{
    [ SlackMessage( "search.all",
                      apiType: Msg.FormEncoded | Msg.UserToken,
                      scopes: new[] { "search:read" } ) ]
    public class SearchAll : SlackMessage
    {
        
        public string query { get; set; }

        public int?  count     { get; set; }
        public bool? highlight { get; set; }
        public int?  page      { get; set; }

        /// <summary>
        /// Possible values: score, timestamp
        /// </summary>
        public string sort { get; set; }

        /// <summary>
        /// Possible values: desc, asc
        /// </summary>
        public string sort_dir { get; set; }
    }

    [ SlackMessage( "search.files",
                      apiType: Msg.FormEncoded | Msg.UserToken,
                      scopes: new[] { "search:read" } ) ]
    public class SearchFiles : SlackMessage
    {
        
        public string query { get; set; }

        public int?  count     { get; set; }
        public bool? highlight { get; set; }
        public int?  page      { get; set; }

        /// <summary>
        /// Possible values: score, timestamp
        /// </summary>
        public string sort { get; set; }

        /// <summary>
        /// Possible values: desc, asc
        /// </summary>
        public string sort_dir { get; set; }
    }

    [ SlackMessage( "search.messages",
                      apiType: Msg.FormEncoded | Msg.UserToken,
                      scopes: new[] { "search:read" } ) ]
    public class SearchMessages : SlackMessage
    {
        
        public string query { get; set; }

        public int?  count     { get; set; }
        public bool? highlight { get; set; }
        public int?  page      { get; set; }

        /// <summary>
        /// Possible values: score, timestamp
        /// </summary>
        public string sort { get; set; }

        /// <summary>
        /// Possible values: desc, asc
        /// </summary>
        public string sort_dir { get; set; }
    }

    public class SearchResponse : MessageResponse
    {
        public string         query    { get; set; }
        public FileResults    files    { get; set; }
        public MessageResults messages { get; set; }

        public class FileResults
        {
            public SlackFile[] matches    { get; set; }
            public Pagination  pagination { get; set; }
            public Paging      paging     { get; set; }
        }

        public class MessageResults
        {
            public MessageBase[] matches    { get; set; }
            public Pagination    pagination { get; set; }
            public Paging        paging     { get; set; }
        }
    }
}
