using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

#if false

namespace Asciis
{
    public class FigletFont
    {
        private string         HardBlank      { get; set; } = "$";
        public  int            Height         { get; private set; }
        private int            BaseLine       { get; set; }
        private int            MaxWidth       { get; set; }
        private int            OldLayout      { get; set; }
        private int            CommentLines   { get; set; }
        private int            PrintDirection { get; set; }
        private int            FullLayout     { get; set; }
        private int            CodeTagCount   { get; set; }
        public  List< string > Lines          { get; } = new();

        public FigletFont(string? flfFontFile = null)
        {
            if (!string.IsNullOrWhiteSpace( flfFontFile ))
                LoadFont(flfFontFile);
            else
                LoadFont();
        }

        private void LoadLines(IReadOnlyCollection< string > fontLines)
        {
            Lines.Clear();

            // First line looks like this
            // flf2a$ 6 5 20 15 16
            // 
            // flf2 - "magic number" for file identification
            // a  - should always be `a', for now
            // $  - the "hardblank" -- prints as a blank, but can't be smushed
            // 6  - height of a character
            // 5  - height of a character, not including descenders
            // 20 - max character width (excluding comment lines) +a fudge factor
            // 15 - default smushmode for this font
            // 16 - number of comment lines
            // <print direction> - ignored
            // <full layout> - ignored
            // <code tag count> - ignored

            var configString = fontLines.First();
            var configArray  = configString.Split(' ');
            var signature    = configArray.First().Remove(configArray.First().Length - 1);

            if (signature != "flf2a")
                throw new FormatException();

            HardBlank      = configArray.First().Last().ToString();
            Height         = GetInt( 1 );
            BaseLine       = GetInt( 2 );
            MaxWidth       = GetInt( 3 );
            OldLayout      = GetInt( 4 );
            CommentLines   = GetInt( 5 );
            PrintDirection = GetInt( 6 );
            FullLayout     = GetInt( 7 );
            CodeTagCount   = GetInt( 8 );

            foreach (var str in fontLines.Skip(CommentLines + 1))
            {
                var line = str;
                var lineEnding = line[^1]; // each font line will have 1 or two line ending characters
                if (line.Length >= 2)
                {
                    if (line[^2] == lineEnding)
                        line = line.Substring(0, line.Length - 2);
                }
                if (line.Length > 1)
                {
                    if (line[^1] == lineEnding)
                        line = line.Substring(0, line.Length - 1);
                }

                Lines.Add( line.Replace( HardBlank, " "));
            }
             
            int GetInt(int pos)
            {
                if (pos < configArray.Length && int.TryParse(configArray[pos], out var val))
                    return val;

                return default;
            }
        }

        private void LoadFont()
        {
            using var stream = GetType()
                              .Assembly
                              .GetManifestResourceStream("Asciis.Figlet.fonts.standard.flf");
            LoadFont(stream!);
        }

        private void LoadFont(string flfFontFile)
        {
            using var fso = File.Open(flfFontFile, FileMode.Open);
            LoadFont(fso);
        }

        private void LoadFont(Stream fontStream)
        {
            var fontData = new List<string>();
            using (var reader = new StreamReader(fontStream))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line != null)
                        fontData.Add( line );
                }
            }
            LoadLines(fontData);
        }
    }

    public class Figlet
    {
        public static readonly FigletFont StandardFont = new ();
        private readonly       FigletFont _font;

        public Figlet(FigletFont? font = null)
        {
            _font = font ?? StandardFont;
        }

        public string Out(string strText)
        {
            var result = new StringBuilder();
            for (var i = 1; i <= _font.Height; i++)
            {
                foreach (var ch in strText)
                    result.Append( GetCharacterLine(ch, i) );

                result.AppendLine();
            }
            return result.ToString();
        }

        private string GetCharacterLine(char ch, int lineNumber)
        {
            var start = (Convert.ToInt32(ch) - 32) * _font.Height;
            return _font.Lines[start + lineNumber];
        }
    }
}

#endif
