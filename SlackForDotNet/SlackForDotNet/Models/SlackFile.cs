using System;
using System.Collections.Generic;

#pragma warning disable 8618 // Models are de-serialised, so shouldn't be null

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable StringLiteralTypo

namespace SlackForDotNet
{
    public class SlackFile
    {
        public string  id        { get; set; }
        public int?    created   { get; set; }
        public int?    timestamp { get; set; }
        public string? name      { get; set; }
        public string? title     { get; set; }
        public string? mimetype  { get; set; }
        /// <summary>Should match SlackFileType enum, but not guaranteed</summary>
        public string? filetype { get;                       set; }
        public string?           pretty_type          { get; set; }
        public string?           user                 { get; set; }
        public bool?             editable             { get; set; }
        public int?              size                 { get; set; }
        public string?           mode                 { get; set; }
        public bool?             is_external          { get; set; }
        public string?           external_type        { get; set; }
        public bool?             is_public            { get; set; }
        public bool?             public_url_shared    { get; set; }
        public bool?             display_as_bot       { get; set; }
        public string?           username             { get; set; }
        public string?           url_download         { get; set; }
        public string?           url_private          { get; set; }
        public string?           url_private_download { get; set; }
        public string?           thumb_64             { get; set; }
        public string?           thumb_80             { get; set; }
        public string?           thumb_360            { get; set; }
        public int?              thumb_360_w          { get; set; }
        public int?              thumb_360_h          { get; set; }
        public string?           thumb_160            { get; set; }
        public string?           thumb_360_gif        { get; set; }
        public int?              image_exif_rotation  { get; set; }
        public int?              original_w           { get; set; }
        public int?              original_h           { get; set; }
        public string?           deanimate_gif        { get; set; }
        public string?           pjpeg                { get; set; }
        public string?           permalink            { get; set; }
        public string?           permalink_public     { get; set; }
        public string?           edit_link            { get; set; }
        public string?           preview              { get; set; }
        public string?           preview_highlight    { get; set; }
        public int?              lines                { get; set; }
        public int?              lines_more           { get; set; }
        public Shares?           shares               { get; set; }
        public int?              comments_count       { get; set; }
        public bool?             is_starred           { get; set; }
        public int?              num_stars            { get; set; }
        public string[]?         channels             { get; set; }
        public string[]?         groups               { get; set; }
        public InstantMessage[]? ims                  { get; set; }
        public Comment?          initial_comment      { get; set; }
        public bool?             has_rich_preview     { get; set; }

        public class Shares
        {
            public Dictionary< string, SlackShare > Public { get; set; }
        }

        public class SlackShare
        {
            public string[] reply_users       { get; set; }
            public int?     reply_users_count { get; set; }
            public int?     reply_count       { get; set; }
            public string   ts                { get; set; }
            public string   thread_ts         { get; set; }
            public string   latest_reply      { get; set; }
            public string   channel_name      { get; set; }
            public string   team_id           { get; set; }
        }

        public enum SlackFileType
        {
            auto,         //    Auto Detect Type
            text,         //    Plain Text
            ai,           //  Illustrator File
            apk,          // APK
            applescript,  // AppleScript
            binary,       //  Binary
            bmp,          // Bitmap
            boxnote,      // BoxNote
            c,            //   C
            csharp,       //  C#
            cpp,          // C++
            css,          // CSS
            csv,          // CSV
            clojure,      // Clojure
            coffeescript, //    CoffeeScript
            cfm,          // ColdFusion
            d,            //   D
            dart,         //    Dart
            diff,         //    Diff
            doc,          // Word Document
            docx,         //    Word document
            dockerfile,   //  Docker
            dotx,         //    Word template
            email,        //   Email
            eps,          // EPS
            epub,         //    EPUB
            erlang,       //  Erlang
            fla,          // Flash FLA
            flv,          // Flash video
            fsharp,       //  F#
            fortran,      // Fortran
            gdoc,         //    GDocs Document
            gdraw,        //   GDocs Drawing
            gif,          // GIF
            go,           //  Go
            gpres,        //   GDocs Presentation
            groovy,       //  Groovy
            gsheet,       //  GDocs Spreadsheet
            gzip,         //    GZip
            html,         //    HTML
            handlebars,   //  Handlebars
            haskell,      // Haskell
            haxe,         //    Haxe
            indd,         //    InDesign Document
            java,         //    Java
            javascript,   //  JavaScript/JSON
            jpg,          // JPEG
            keynote,      // Keynote Document
            kotlin,       //  Kotlin
            latex,        //   LaTeX/sTeX
            lisp,         //    Lisp
            lua,          // Lua
            m4a,          // MPEG 4 audio
            markdown,     //    Markdown (raw)
            matlab,       //  MATLAB
            mhtml,        //   MHTML
            mkv,          // Matroska video
            mov,          // QuickTime video
            mp3,          // mp4
            mp4,          // MPEG 4 video
            mpg,          // MPEG video
            mumps,        // MUMPS
            numbers,      // Numbers Document
            nzb,          // NZB
            objc,         //    Objective-C
            ocaml,        //   OCaml
            odg,          // OpenDocument Drawing
            odi,          // OpenDocument Image
            odp,          // OpenDocument Presentation
            ods,          // OpenDocument Spreadsheet
            odt,          // OpenDocument Text
            ogg,          // Ogg Vorbis
            ogv,          // Ogg video
            pages,        // Pages Document
            pascal,       //  Pascal
            pdf,          // PDF
            perl,         //    Perl
            php,          // PHP
            pig,          // Pig
            png,          // PNG
            post,         //    Slack Post
            powershell,   // PowerShell
            ppt,          // PowerPoint presentation
            pptx,         //    PowerPoint presentation
            psd,          // Photoshop Document
            puppet,       //  Puppet
            python,       //Python
            qtz,          //Quartz Composer Composition
            r,            //R
            rtf,          //Rich Text File
            ruby,         //Ruby
            rust,         //Rust
            sql,          //SQL
            sass,         // Sass
            scala,        //Scala
            scheme,       //Scheme
            sketch,       //Sketch File
            shell,        //Shell
            smalltalk,    //Smalltalk
            svg,          //SVG
            swf,          //Flash SWF
            swift,        // Swift
            tar,          //Tarball
            tiff,         // TIFF
            tsv,          //TSV
            vb,           //VB.NET
            vbscript,     //    VBScript
            vcard,        //vCard
            velocity,     //    Velocity
            verilog,      //Verilog
            wav,          //Waveform audio
            webm,         //WebM
            wmv,          //Windows Media Video
            xls,          //Excel spreadsheet
            xlsx,         //Excel spreadsheet
            xlsb,         //Excel Spreadsheet(Binary, Macro Enabled)
            xlsm,         //Excel Spreadsheet(Macro Enabled)
            xltx,         //Excel template
            xml,          //XML
            yaml,         // YAML
            zip,          //Zip
        }
    }
}
