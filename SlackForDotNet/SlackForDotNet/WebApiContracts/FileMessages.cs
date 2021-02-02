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
    [ SlackMessage( "files.comment.delete",
                      apiType: Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "bot", "file:write:user" } ) ]
    public class FileCommentsDelete : SlackMessage
    {
        public string file { get; set; }
    }

    [ SlackMessage( "files.delete",
                      apiType: Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "bot", "file:write:user" } ) ]
    public class FileDelete : SlackMessage
    {
        
        public string file { get; set; }
    }

    [ SlackMessage( "files.info",
                      apiType: Msg.FormEncoded | Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "files:read" } ) ]
    public class FilesInfo : SlackMessage
    {
        
        public string file { get; set; }

        public int?   count  { get; set; }
        public int?   limit  { get; set; }
        public int?   page   { get; set; }
        public string cursor { get; set; }
    }

    [ SlackMessage( "files.list",
                      apiType: Msg.FormEncoded | Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "files:read" } ) ]
    public class FileList : SlackMessage
    {
        public string channel { get; set; }
        public int?   count   { get; set; }
        public int?   page    { get; set; }
        public int?   ts_from { get; set; }
        public int?   ts_to   { get; set; }

        /// <summary>
        /// Possible values: all, spaces, snippets, images, gdocs, zips, pdfs
        /// </summary>
        public string types { get; set; }

        public string user { get; set; }
    }

    [ SlackMessage( "files.revokePublicURL",
                      apiType: Msg.UserToken,
                      scopes: new[] { "files:write:user" } ) ]
    public class FileRevokePublicUrl : SlackMessage
    {
        
        public string file { get; set; }
    }

    [ SlackMessage( "files.sharePublicURL",
                      apiType: Msg.UserToken,
                      scopes: new[] { "files:write:user" } ) ]
    public class FileSharePublicUrl : SlackMessage
    {
        
        public string file { get; set; }
    }

    /// <summary>
    /// https://api.slack.com/methods/files.upload
    /// </summary>
    [ SlackMessage( "files.upload",
                      apiType: Msg.Multipart | Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "bot", "file:write:user" } ) ]
    public class FileUpload : SlackMessage
    {
        public string channels        { get; set; }
        public string filename        { get; set; }
        public string filetype        { get; set; }
        public string initial_comment { get; set; }
        public string thread_ts       { get; set; }
        public string title           { get; set; }
    }

    /// <summary>
    /// https://api.slack.com/methods/files.remote.add
    /// </summary>
    [ SlackMessage( "files.remote.add",
                      apiType: Msg.Multipart | Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "remote_files:write" } ) ]
    public class FilesRemoteAdd : SlackMessage
    {
        
        public string external_token { get; set; }

        
        public string external_url { get; set; }

        
        public string title { get; set; }

        public string filetype { get; set; }
    }

    [ SlackMessage( "files.remote.info",
                      apiType: Msg.FormEncoded | Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "remote_files:read" } ) ]
    public class FilesRemoteInfo : SlackMessage
    {
        public string external_id { get; set; }
        public string file        { get; set; }
    }

    [ SlackMessage( "files.remote.list",
                      apiType: Msg.FormEncoded | Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "remote_files:read" } ) ]
    public class FilesRemoteList : SlackMessage
    {
        public string channel { get; set; }
        public string cursor  { get; set; }
        public int?   limit   { get; set; }
        public int?   ts_from { get; set; }
        public int?   ts_to   { get; set; }
    }

    [ SlackMessage( "files.remote.remove",
                      apiType: Msg.FormEncoded | Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "remote_files:write" } ) ]
    public class FilesRemoteRemove : SlackMessage
    {
        public string external_id { get; set; }
        public string file        { get; set; }
    }

    [ SlackMessage( "files.remote.share",
                      apiType: Msg.FormEncoded | Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "remote_files:share" } ) ]
    public class FilesRemoteShare : SlackMessage
    {
        public string channels    { get; set; }
        public string external_id { get; set; }
        public string file        { get; set; }
    }

    /// <summary>
    /// https://api.slack.com/methods/files.remote.update
    /// </summary>
    [ SlackMessage( "files.remote.update",
                      apiType: Msg.FormEncoded | Msg.Multipart | Msg.BotToken | Msg.UserToken,
                      scopes: new[] { "remote_files:write" } ) ]
    public class FilesRemoteUpdate : SlackMessage
    {
        public string external_id  { get; set; }
        public string external_url { get; set; }
        public string file         { get; set; }
        public string filetype     { get; set; }
        public string title        { get; set; }
    }

    public class FileResponse : MessageResponse
    {
        public SlackFile file              { get; set; }
        public Comment[] comments          { get; set; }
        public MetaData  response_metadata { get; set; }
    }

    public class FilesResponse : MessageResponse
    {
        public SlackFile[] files  { get; set; }
        public Paging      paging { get; set; }
    }
}
