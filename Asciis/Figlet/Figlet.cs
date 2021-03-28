using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
// ReSharper disable CommentTypo
// ReSharper disable InvalidXmlDocComment

namespace Asciis
{
    /// <summary>
    ///     Figlet
    /// </summary>
    /// <remarks>
    ///     #define DATE "31 May 2012"
    ///     #define VERSION "2.2.5"
    ///     #define VERSION_INT 20205
    /// </remarks>
    public partial class Figlet
    {
        public static string Render(string str, string? fontName = null, SmushMode? smushMode = null)
        {
            var figlet = new Figlet( fontFile: fontName, smushMode: smushMode );
            figlet.Add( str );

            return figlet.Render();
        }


        private const string FontFileMagicNumber = "flf2";
        private const int    DefaultOutputWidth  = 0;

        /// <summary>
        ///     Latin-1 codes for German letters,       respectively:
        ///     LATIN                    CAPITAL LETTER A WITH DIAERESIS = A - umlaut
        ///     LATIN                    CAPITAL LETTER O WITH DIAERESIS = O - umlaut
        ///     LATIN                    CAPITAL LETTER U WITH DIAERESIS = U - umlaut
        ///     LATIN                    SMALL LETTER   A WITH DIAERESIS = a - umlaut
        ///     LATIN                    SMALL LETTER   O WITH DIAERESIS = o - umlaut
        ///     LATIN                    SMALL LETTER   U WITH DIAERESIS = u - umlaut
        ///     LATIN                    SMALL LETTER   SHARP S = ess-zed
        /// </summary>
        private static readonly char[] DutchChar = { (char)196, (char)214, (char)220, (char)228, (char)246, (char)252, (char)223 };

        private readonly int                   _maxOutputWidth;
        private readonly StringBuilder[]       _outLines;
        private readonly SmushOverride         _smushoverride;
        private readonly Justification         _justification;
        private          char                  _hardblank;
        private          int                   _charheight;
        private readonly List< FigletChar >    _figlets     = new();

        private FigletChar? _currentFiglet;
        private FigletChar? _previousFiglet;
        private FigletChar? _missingFiglet;

        private SmushMode _smushMode;

        private int OutLineLength =>
            _outLines[ 0 ].Length;

        private FigletChar MissingFigletChar =>
            _missingFiglet ??= new ( '\0', _charheight );

        public Figlet( 
            string?        fontFile       = null,
            Justification? justification  = null,
            int?           maxOutputWidth = null,
            SmushMode?     smushMode      = null
            )
        {
            _smushoverride  = (smushMode != null) ? SmushOverride.Yes : SmushOverride.No;
            _smushMode      = smushMode      ?? SmushMode.Smush | SmushMode.Kern;
            _justification  = justification  ?? Justification.Left;
            _maxOutputWidth = maxOutputWidth ?? DefaultOutputWidth;

            ReadFont( fontFile ?? Fonts.Standard );

            _outLines = new StringBuilder[ _charheight ];
            for (int i = 0; i < _charheight; i++)
                _outLines[ i ] = new StringBuilder();
        }

        /// <summary>
        ///     Clears output lines
        /// </summary>
        private void ClearLines()
        {
            for (int i = 0; i < _charheight; i++)
                _outLines[ i ].Clear();
        }

        /// <summary>
        ///     Reads a font character from the font file, and places it in a newly-allocated entry in the list.
        /// </summary>
        private void ReadFontChar( string[] lines, ref int index, char ch )
        {
            if (index >= lines.Length)
                return;

            var fc = new FigletChar( ch, _charheight );

            for (int row = 0; row < _charheight; row++)
            {
                var line = lines[ index++ ].TrimEnd();
                if (line.Length > 0)
                {
                    var endchar = line[ ^1 ];
                    if (line.Length > 2 && line[ ^2 ] == endchar)
                        fc.Lines[ row ] = line.Substring( 0, line.Length - 2 );
                    else
                        fc.Lines[ row ] = line.Substring( 0, line.Length - 1 );
                }
                else
                    fc.Lines[ row ] = string.Empty;
            }

            _figlets.Add( fc );
        }

        private string[] ReadAllLines( string filename )
        {
            var resourceName  = "Asciis.Figlet.fonts." + filename;
            var resourceNames = typeof(Figlet).Assembly.GetManifestResourceNames().ToList();
            if (resourceNames.Contains( resourceName ))
            {
                using var stream = typeof(Figlet).Assembly.GetManifestResourceStream( resourceName );
                using var sr     = new StreamReader( stream! );

                return sr.EnumerateLines().ToArray();
            }

            return File.ReadAllLines( filename ).ToArray();
        }

        /// <summary>
        ///     Read and parse the font file
        /// </summary>
        private void ReadFont( string fontFilename )
        {
            if (string.IsNullOrEmpty( fontFilename ))
                return;

            if (!fontFilename.EndsWith( ".flf", StringComparison.OrdinalIgnoreCase ))
                fontFilename += ".flf";

            SmushMode fullLayout;

            var lines = ReadAllLines( fontFilename );

            if (lines.Length <= 0)
                return;

            int index = 0;

            // First line looks like this
            // flf2a$ 6 5 20 15 16
            // 
            // flf2 - "magic number" for file identification
            // a <a>            should always be `a'
            // $ <hardblank>    prints as a blank, but can't be smushed (optional)
            // <height of a character>
            // <height of a character, not including descenders> - ignored
            // <max character width> - ignored
            // <default SmushMode for this font>
            // <number of comment lines>
            // <print direction> 
            // <full layout> 
            // <code tag count> 

            var line  = lines[ index++ ];
            var items = line.Split( ' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries );

            if (!items[ 0 ].StartsWith( FontFileMagicNumber ) || items.Length < 5)
                throw new FormatException( "Not a figlet font file" );
            if (items[ 0 ].Length >= 6)
                _hardblank = items[ 0 ][ 5 ];

            _charheight = Convert.ToInt32( items[ 1 ] );
            _           = Convert.ToInt32( items[ 2 ] ); // Figlet height without descender value is Ignore
            _           = Convert.ToInt32( items[ 3 ] ); // Max figlet width Ignored
            var smush        = (SmushMode)Convert.ToInt32( items[ 4 ] );
            var commentLines = Convert.ToInt32( items[ 5 ] );
            if (items.Length > 6)
                _ = Convert.ToInt32( items[ 6 ] ); // Print direction value is not implemented
            if (items.Length > 7)
                fullLayout = (SmushMode)Convert.ToInt32( items[ 7 ] );
            else
            {
                // if smush2 is not supplied, convert smush into smush2
                if (smush == SmushMode.Unknown)
                    fullLayout = SmushMode.Kern;
                else
                    fullLayout = ( smush & SmushMode.OldLayout ) | SmushMode.Smush;
            }

            if (_charheight < 1)
                _charheight = 1;

            if (_smushoverride == SmushOverride.No)
                _smushMode = fullLayout;
            else if (_smushoverride == SmushOverride.Force)
                _smushMode |= fullLayout;

            // Skip all the comments
            index += commentLines;

            for (char ch = ' '; ch <= '~'; ch++)
                ReadFontChar( lines, ref index, ch );

            for (var ch = 0; ch <= 6; ch++)
                ReadFontChar( lines, ref index, DutchChar[ ch ] );

            while (index < lines.Length)
            {
                var xtraLine = lines[ index++ ];

                if (int.TryParse( xtraLine.TrimStart(), out int theCh ))
                {
                    ReadFontChar( lines, ref index, (char)theCh );
                }
                else
                    break;
            }
        }

        /// <summary>
        ///     Given 2 characters, attempts to smush them into 1, according to
        ///     SmushMode.Returns smushed character or '\0' if no smushing can be done.
        ///
        ///     SmushMode values are sum of following(all values smush blanks):
        ///      1: Smush equal chars(not hardblanks)
        ///      2: Smush '_' with any char in hierarchy below
        ///      4: hierarchy: "|", "/\", "[]", "{}", "()", "<>"
        ///         Each class in hierarchy can be replaced by later class.
        ///      8: [ + ] -> |, { + } -> |, ( + ) -> |
        ///     16: / + \ -> X, "> + <" -> X(only in that order)
        ///     32: hardblank + hardblank -> hardblank
        ///
        /// </summary>
        protected virtual char SmushEm( char lch, char rch )
        {
            if (lch == ' ')
                return rch;
            if (rch == ' ')
                return lch;

            // Disallows overlapping if the previous character
            // or the current character has a width of 1 or zero.
            if (_previousFiglet != null && _previousFiglet.Width < 2)
                return '\0';
            if (_currentFiglet != null && _currentFiglet.Width < 2)
                return '\0';

            if (!_smushMode.HasFlag( SmushMode.Smush ))
                return '\0'; // kerning

            // This is smushing by universal overlapping
            if (( _smushMode & ( SmushMode.OldLayout | SmushMode.Hardblank ) ) == 0)
            {
                // ensure overlapping preference to visible characters.
                if (lch == _hardblank)
                    return rch;
                if (rch == _hardblank)
                    return lch;

                return rch;
            }

            if (_smushMode.HasFlag( SmushMode.Hardblank ))
            {
                if (lch == _hardblank && rch == _hardblank)
                    return lch;
            }

            if (lch == _hardblank || rch == _hardblank)
                return '\0';

            if (_smushMode.HasFlag( SmushMode.Equal ))
            {
                if (lch == rch)
                    return lch;
            }

            if (_smushMode.HasFlag( SmushMode.Underscore ))
            {
                if (lch == '_' && "|/\\[]{}()<>".Contains( rch )) return rch;
                if (rch == '_' && "|/\\[]{}()<>".Contains( lch )) return lch;
            }

            if (_smushMode.HasFlag( SmushMode.Hierarchy ))
            {
                if (lch == '|'            && "/\\[]{}()<>".Contains( rch )) return rch;
                if (rch == '|'            && "/\\[]{}()<>".Contains( lch )) return lch;
                if ("/\\".Contains( lch ) && "[]{}()<>".Contains( rch )) return rch;
                if ("/\\".Contains( rch ) && "[]{}()<>".Contains( lch )) return lch;
                if ("[]".Contains( lch )  && "{}()<>".Contains( rch )) return rch;
                if ("[]".Contains( rch )  && "{}()<>".Contains( lch )) return lch;
                if ("{}".Contains( lch )  && "()<>".Contains( rch )) return rch;
                if ("{}".Contains( rch )  && "()<>".Contains( lch )) return lch;
                if ("()".Contains( lch )  && "<>".Contains( rch )) return rch;
                if ("()".Contains( rch )  && "<>".Contains( lch )) return lch;
            }

            if (_smushMode.HasFlag( SmushMode.Pair ))
            {
                if (lch == '[' && rch == ']') return '|';
                if (lch == ']' && rch == '[') return '|';
                if (lch == '{' && rch == '}') return '|';
                if (lch == '}' && rch == '{') return '|';
                if (lch == '(' && rch == ')') return '|';
                if (lch == ')' && rch == '(') return '|';
            }

            if (_smushMode.HasFlag( SmushMode.Bigx ))
            {
                if (lch == '/'  && rch == '\\') return '|';
                if (lch == '\\' && rch == '/') return 'Y';
                if (lch == '>'  && rch == '<') return 'X';
            }

            return '\0';
        }

        /// <summary>
        ///     Returns the maximum amount that the current character can be smushed into the current line.
        /// </summary>
        private int SmushAmt()
        {
            if ((_smushMode & (SmushMode.Smush | SmushMode.Kern)) == 0)
                return 0;

            var maxsmush = _currentFiglet!.Width;

            for (int row = 0; row < _charheight; row++)
            {
                var lineLength = _outLines[ row ].Length;
                var linebd     = lineLength;
                var ch1        = _outLines[ row ].TryGet( linebd );
                while ((linebd > 0 && (ch1 == '\0' || ch1 == ' ')))
                {
                    linebd--;
                    ch1 = _outLines[row][linebd];
                }

                var charbd = 0;
                var ch2    = _currentFiglet.Lines[ row ].TryGet( charbd );
                while (ch2 == ' ')
                {
                    charbd++;
                    ch2 = _currentFiglet.Lines[ row ].TryGet( charbd );
                }
                int amt = charbd + lineLength - 1 - linebd;

                if (ch1 == 0 || ch1 == ' ')
                {
                    amt++;
                }
                else if (ch2 != 0)
                {
                    if (SmushEm(ch1, ch2) != '\0')
                        amt++;
                }
                if (amt < maxsmush)
                    maxsmush = amt;
            }
            return maxsmush;
        }

        /// <summary>
        ///     Sets _currentFiglet to point to the font entry for the given character.
        /// </summary>
        public FigletChar GetLetter( char c )
        {
            _previousFiglet = _currentFiglet;
            _currentFiglet  = _figlets.FirstOrDefault( f => f.Ch == c ) ?? MissingFigletChar;

            return _currentFiglet;
        }

        private void ReplaceChar( StringBuilder sb, int index, char ch )
        {
            if (ch == '\0')
                return;
            if (index < sb.Length)
                sb[ index ] = ch;
            else
                sb.Append( ch );
        }

        public bool Add( string str )
        {
            foreach (var ch in str)
            {
                if (!Add(ch))
                    return false;
            }

            return true;
        }

        /// <summary>
        ///     Attempts to add the given character onto the end of the current line.
        ///     Returns 1 if this can be done, 0 otherwise.
        /// </summary>
        public bool Add( char c )
        {
            int row;

            var figletChar = GetLetter( c );

            var smushamount = SmushAmt();

            if (_maxOutputWidth > 0 &&
                OutLineLength + figletChar.Width - smushamount > _maxOutputWidth)
            {
                return false;
            }

            for (row = 0; row < _charheight; row++)
            {
                for (int k = 0; k < smushamount; k++)
                {
                    var column = _outLines[row].Length - smushamount + k;
                    if (column < 0)
                        column = 0;

                    var outChar = _outLines[ row ].Length > column
                                      ? _outLines[ row ][ column ]
                                      : '\0';
                    var ch = SmushEm( outChar, figletChar.Lines[ row ][ k ] );
                    ReplaceChar( _outLines[ row ], column, ch );
                }
                var tmp = figletChar.Lines[ row ].Substring( smushamount );
                _outLines[ row ].Append( tmp );
            }

            return true;
        }

        /// <summary>
        ///     Formats the given string, substituting blanks for hardblanks.
        /// </summary>
        private string FormatString( string str )
        {
            if (_maxOutputWidth > 1)
            {
                var len = str.Length;
                if (len > _maxOutputWidth)
                    len = _maxOutputWidth;
                string left;
                if (_justification == Justification.Right)
                    left = new string( ' ', _maxOutputWidth - len );
                else if (_justification == Justification.Centered)
                    left = new string( ' ', ( _maxOutputWidth - len ) / 2 );
                else
                    left = "";

                return left + str.Replace( _hardblank, ' ' );
            }

            return str.Replace( _hardblank, ' ' );
        }

        /// <summary>
        ///     Renders all outLines as a single string
        /// </summary>
        public string Render(StringBuilder? sb = null)
        {
            sb ??= new StringBuilder();

            for (var i = 0; i < _charheight; i++)
                sb.AppendLine( FormatString( _outLines[ i ].ToString() ) );
            ClearLines();

            return sb.ToString();
        }

        public static string[] FontNames()
        {
            var names = new List< string >();

            var resourceNames = typeof(Figlet).Assembly.GetManifestResourceNames().ToList();
            foreach (var resourceName in resourceNames)
            {
                if (resourceName.StartsWith("Asciis.Figlet.fonts."))
                    names.Add( resourceName.Replace("Asciis.Figlet.fonts.", "" ) );
            }

            return names.ToArray();
        }

        /// <summary>
        ///     How to resolve specified SmushMode with font files SmushMode
        /// </summary>
        private enum SmushOverride
        {
            No,     // take Smush mode from font file
            Yes,    // take Smush mode from constructor parameter
            Force   // merge Smush mode from constructor parameter and font file
        }
    }

    public class FigletChar
    {
        public readonly char     Ch;
        public readonly string[] Lines;

        public FigletChar( char ch, int height )
        {
            Ch    = ch;
            Lines = new string[ height ];
            Array.Fill( Lines, string.Empty );
        }

        public int Width =>
            Lines[ 0 ].Length;
    }

    /// <summary>
    ///
    ///    HORIZONTAL SMUSHING RULES
    ///
    ///    Rule 1: EQUAL CHARACTER SMUSHING
    ///        Two sub-characters are smushed into a single sub-character if they are the same.
    ///        This rule does not smush hardblanks
    ///
    ///    Rule 2: UNDERSCORE SMUSHING
    ///        An underscore ("_") will be replaced by any of: "|/\[]{}()<>".
    ///
    ///    Rule 3: HIERARCHY SMUSHING
    ///        A hierarchy of six classes is used: "|", "/\", "[]", "{}", "()", " <> ".  
    ///        When two smushing sub-characters are from different classes,
    ///        the one from the latter class will be used.
    ///
    ///    Rule 4: OPPOSITE PAIR SMUSHING
    ///        Smushes opposing "[]"-"][", "{}"-"}{", "()"-")(" together,
    ///        replacing any such pair with a vertical bar ("|").
    ///
    ///    Rule 5: BIG X SMUSHING
    ///        Smushes "/\" into "|", "\/" into "Y", and "><" into "X".
    ///        Note that "<>" is not smushed in any way by this rule.
    ///        The name "BIG X" is historical; originally all three pairs were smushed into "X".
    ///
    ///    Rule 6: HARDBLANK SMUSHING
    ///        Smushes two hardblanks together, replacing them with a single hardblank
    ///
    ///
    ///    VERTICAL SMUSHING RULES
    ///
    ///    Rule 1: EQUAL CHARACTER SMUSHING
    ///        Two sub-characters are smushed into a single sub-character if they are the same.  
    ///        This rule does not smush hardblanks
    ///
    ///    Rule 2: UNDERSCORE SMUSHING
    ///        An underscore ("_") will be replaced by any of: "|/\[]{}()<>".
    ///
    ///    Rule 3: HIERARCHY SMUSHING
    ///        A hierarchy of six classes is used: "|", "/\", "[]", "{ }", "()", " <> ".  
    ///        When two smushing sub-characters are from different classes,
    ///        the one from the latter class will be used.
    ///
    ///    Rule 4: HORIZONTAL LINE SMUSHING
    ///        Smushes stacked pairs of "-" and "_", replacing them with a single "=" sub-character.
    ///        It does not matter which is found above the other.  
    ///        Note that vertical smushing rule 1 will smush IDENTICAL pairs of horizontal lines, while this
    ///        rule smushes horizontal lines consisting of DIFFERENT sub-characters.
    ///
    ///    Rule 5: VERTICAL LINE SUPERSMUSHING
    ///        This one rule is different from all others, in that it "supersmushes" vertical lines consisting of several
    ///        vertical bars ("|").  
    ///        This creates the illusion that FIG characters have slid vertically against each other.
    ///        Supersmushing continues until any sub-characters other than "|" would have to be smushed.  
    ///        Supersmushing can produce impressive results, but it is seldom possible, since other sub-characters would 
    ///        usually have to be considered for smushing as soon as any such stacked vertical lines are encountered.
    ///        
    /// </summary>
    [Flags ]
    public enum SmushMode
    {
        Unknown        = 0x0000,
        Equal          = 0x0001,
        Underscore     = 0x0002,
        Hierarchy      = 0x0004,
        Pair           = 0x0008,
        Bigx           = 0x0010,
        Hardblank      = 0x0020,
        Kern           = 0x0040, // Horizontal fitting by default
        Smush          = 0x0080, // Horizontal smushing by default (overrides Kern)
        VertEquals     = 0x0100,
        VertUnderscore = 0x0200,
        VertHierachy   = 0x0400,
        VertHorzLine   = 0x0800,
        VertSuper      = 0x1000,

        OldLayout = Equal | Underscore | Hierarchy | Pair | Bigx
    }

    public enum Justification
    {
        Left,
        Centered,
        Right
    }
}
