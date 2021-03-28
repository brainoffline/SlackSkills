using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
    public class Figlet
    {
        private const string FontFileMagicNumber = "flf2";
        private const int    DefaultOutputWidth  = 0;

        public static class Fonts
        {
            public const string _3D              = "3D";
            public const string _3DASCII         = "3DASCII";
            public const string _3ddiagonal      = "3ddiagonal";
            public const string Acrobatic        = "Acrobatic";
            public const string Alpha            = "Alpha";
            public const string AMCAAA01         = "AMCAAA01";
            public const string AMCRazor         = "AMCRazor";
            public const string AMCSlash         = "AMCSlash";
            public const string AMCSlider        = "AMCSlider";
            public const string ANSIRegular      = "ANSIRegular";
            public const string ANSIShadow       = "ANSIShadow";
            public const string banner           = "Banner";
            public const string Banner3D         = "Banner3D";
            public const string Basic            = "Basic";
            public const string Bell             = "Bell";
            public const string Big              = "Big";
            public const string BigChief         = "BigChief";
            public const string BigMoneyNE       = "BigMoneyNE";
            public const string BigMoneyNW       = "BigMoneyNW";
            public const string BigMoneySE       = "BigMoneySE";
            public const string BigMoneySW       = "BigMoneySW";
            public const string Block            = "Block";
            public const string Bloody           = "Bloody";
            public const string Braced           = "Braced";
            public const string Broadway         = "Broadway";
            public const string BroadwayKB       = "BroadwayKB";
            public const string Bubble           = "Bubble";
            public const string Bulbhead         = "Bulbhead";
            public const string Caligraphy2      = "Caligraphy2";
            public const string Coinstak         = "Coinstak";
            public const string Cola             = "Cola";
            public const string Bosmic           = "Cosmic";
            public const string Crazy            = "Crazy";
            public const string DancingFont      = "DancingFont";
            public const string Digital          = "Digital";
            public const string Doh              = "Doh";
            public const string DotMatrix        = "DotMatrix";
            public const string Double           = "Double";
            public const string Electronic       = "Electronic";
            public const string Elite            = "Elite";
            public const string Fraktur          = "Fraktur";
            public const string Georgia11        = "Georgia11";
            public const string Georgia16        = "Georgia16";
            public const string Graffiti         = "Graffiti";
            public const string Impossible       = "Impossible";
            public const string Isometric1       = "Isometric1";
            public const string Isometric2       = "Isometric2";
            public const string Isometric3       = "Isometric3";
            public const string Isometric4       = "Isometric4";
            public const string Lean             = "Lean";
            public const string Letters          = "Letters";
            public const string Maxiwi           = "Maxiwi";
            public const string Merlin1          = "Merlin1";
            public const string Mini             = "Mini";
            public const string Miniwi           = "Miniwi";
            public const string Mnemonic         = "Mnemonic";
            public const string NancyjFancy      = "NancyjFancy";
            public const string NancyjImproved   = "NancyjImproved";
            public const string NancyjUnderlined = "NancyjUnderlined";
            public const string NScript          = "NScript";
            public const string PatorjkHeX       = "PatorjkHeX";
            public const string Pebbles          = "Pebbles";
            public const string Roman            = "Roman";
            public const string SantaClara       = "SantaClara";
            public const string Script           = "Script";
            public const string Shadow           = "Shadow";
            public const string Shimrod          = "Shimrod";
            public const string Slant            = "Slant";
            public const string SmallIsometric1  = "SmallIsometric1";
            public const string Small            = "Small";
            public const string SmScript         = "SmScript";
            public const string SmShadow         = "SmShadow";
            public const string SmSlant          = "SmSlant";
            public const string Soft             = "Soft";
            public const string Speed            = "Speed";
            public const string Standard         = "Standard";
            public const string StarWars         = "StarWars";
            public const string SubZero          = "SubZero";
            public const string SwampLand        = "SwampLand";
            public const string Swan             = "Swan";
            public const string Sweet            = "Sweet";
            public const string Term             = "Term";
            public const string This             = "This";
            public const string Train            = "Train";
            public const string Univers          = "Univers";
            public const string Varsity          = "Varsity";
            public const string Whimsy           = "Whimsy";

        }

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
        private          List< FigletCommand > _commandlist = new();

        private FigletChar? _currentFiglet;
        private FigletChar? _previousFiglet;
        private FigletChar? _missingFiglet;

        // Not implemented
        private readonly int[] _gndbl = new int[4];       // _gndbl[n] is true if Gn is double-byte 
        private readonly int[] _gn    = { 0, 0x80, 0, 0 }; // Gn character sets: ASCII, Latin-1, none, none 
        private          int   _gl;                       // 0-3 specifies left-half Gn character set 
        private          int   _gr = 1;                   // 0-3 specifies right-half Gn character set 
        private          int   _charHeightIgnoreDescenders;

        private SmushMode _smushMode;

        public int OutLineLength =>
            _outLines[ 0 ].Length;

        private FigletChar MissingFigletChar =>
            _missingFiglet ??= new ( '\0', _charheight );

        public Figlet( 
            string?        fontFile       = null,
            string?        controlFile    = null,
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
            if (!string.IsNullOrWhiteSpace( controlFile ))
                ReadControlFile( controlFile );

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
        ///     Processes "g[0123]" character set specifier
        /// </summary>
        /// <example>
        ///     g 0 94 J
        ///     g 1 94 I
        /// </example>
        void CharSet( int setIndex, string[] tokens )
        {

            if (tokens[ 2 ][ 0 ] != '9')
                return;

            var ch = tokens[ 2 ][ 1 ];
            if (ch == '6')
            {
                _gn[ setIndex ]    = (int)( 65536L * ReadTChar( tokens[ 3 ] ) + 0x80 );
                _gndbl[ setIndex ] = 0;

                return;
            }

            if (ch != '4')
                return;

            ch = tokens[ 3 ][ 0 ];
            if (ch == 'x')
            {
                if (tokens[ 3 ][ 1 ] != '9')
                    return;

                if (tokens[ 3 ][ 2 ] != '4')
                    return;

                _gn[ setIndex ]    = (int)( 65536L * ReadTChar( tokens[ 4 ] ) );
                _gndbl[ setIndex ] = 1;

                return;
            }
            _gn[ setIndex ]    = (int)( 65536L * ReadTChar( tokens[ 3 ] ) );
            _gndbl[ setIndex ] = 0;
        }

        int ReadTChar( string line )
        {
            var index = 0;

            return ReadTChar( line, ref index );
        }

        /// <summary>
        ///   Reads a control file "T" command character specification.
        ///   Character is a single byte, an escape sequence, or an escaped numeric.
        /// </summary>
        int ReadTChar( string line, ref int index )
        {
            int thechar = line[ index++ ];

            if (thechar != '\\')
                return thechar;
            var next = line[ index++ ];
            switch (next)
            {
            case 'a':
                return 7;
            case 'b':
                return 8;
            case 'e':
                return 27;
            case 'f':
                return 12;
            case 'n':
                return 10;
            case 'r':
                return 13;
            case 't':
                return 9;
            case 'v':
                return 11;
            default:
                if (next == '-' || next == 'x' || ( next >= '0' && next <= '9' ))
                {
                    index--;

                    return ReadNumber( line, ref index );
                }

                return next;
            }
        }

        private int ReadNumber( string str )
        {
            var index = 0;

            return ReadNumber( str, ref index );
        }

        private int ReadNumber( string str, ref int index )
        {
            int    acc = 0;
            int    baseNum;
            int    sign   = 1;
            string digits = "0123456789ABCDEF";

            if (index == 0)
                str = str.Trim();

            if (str[ index ] == '-')
            {
                sign = -1;
                index++;
            }
            var c = str[ index++ ];
            if (c == '0')
            {
                c = str[ index++ ];
                if (c == 'x' || c == 'X')
                {
                    baseNum = 16;
                }
                else
                {
                    baseNum = 8;
                    index--;
                }
            }
            else
            {
                baseNum = 10;
                index--;
            }

            while (index < str.Length)
            {
                c = char.ToUpper( str[ index++ ] );

                if (!digits.Contains( c ))
                {
                    index--;

                    return acc * sign;
                }
                acc = ( acc * baseNum ) + ( c - '0' );
            }

            return acc * sign;
        }

        /// <summary>
        /// Allocates memory and reads in the given control file.
        /// </summary>
        private void ReadControlFile( string? filename )
        {
            if (string.IsNullOrWhiteSpace( filename ))
                return;

            var lines = ReadAllLines( filename );

            if (lines.Length <= 0)
                return;

            _commandlist = new();

            foreach (var str in lines)
            {
                var line = str.Trim();

                if (line.Length == 0 || line[ 0 ] == '#')
                    continue;

                var tokens  = line.Split( " \t\r\n\f\v", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries );
                var command = line[ 0 ];
                int firstch;
                int lastch;
                int offset;

                switch (command)
                {
                case 't': // Translate
                    // t a \0x05d0
                    // t a-z A-Z
                    // t \224-\246 \192-\214
                    // t \255 Y
                    var index        = 0;
                    firstch = lastch = ReadTChar( tokens[ 1 ], ref index );

                    if (tokens[ 1 ][ index ] == '-')
                    {
                        index++;
                        lastch = ReadTChar( tokens[ 1 ], ref index );
                    }

                    index  = 0;
                    offset = ReadTChar( tokens[ 2 ], ref index ) - firstch;

                    _commandlist.Add( new FigletCommand { Command = 1, RangeLo = firstch, RangeHi = lastch, Offset = offset } );

                    break;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '-':
                    // Mapping table entry
                    // 0x4A0020    0x20
                    // 0x4A005C    0xA5    # \ -> Yen
                    // 0x00	0x0000	# NULL (NUL)
                    firstch = ReadNumber( tokens[ 0 ] );
                    lastch  = ReadNumber( tokens[ 1 ] );

                    offset = lastch - firstch;
                    lastch = firstch;

                    _commandlist.Add( new FigletCommand { Command = 1, RangeLo = firstch, RangeHi = lastch, Offset = offset } );

                    break;
                case 'f': // freeze
                    _commandlist.Add( new FigletCommand { Command = 0 } );

                    break;
                case 'b': // DBCS input mode 
                case 'u': // UTF-8 input mode
                case 'h': // HZ input mode 
                case 'j': // Shift-JIS input mode 
                    // multi-byte not supported
                    break;

                case 'g': // ISO 2022 character set choices 
                    // g 0 94 J
                    // g 1 94 I
                    // g L 0
                    // g R 1

                    command = tokens[ 1 ][ 0 ];
                    switch (command)
                    {
                    case '0': // define G0 charset
                        CharSet( 0, tokens );

                        break;
                    case '1': // set G1 charset 
                        CharSet( 1, tokens );

                        break;
                    case '2': // set G2 charset 
                        CharSet( 2, tokens );

                        break;
                    case '3': // set G3 charset 
                        CharSet( 3, tokens );

                        break;
                    case 'l':
                    case 'L': // define left half
                        _gl = tokens[ 2 ][ 0 ] - '0';

                        break;
                    case 'r':
                    case 'R': // define right half
                        _gr = tokens[ 2 ][ 0 ] - '0';

                        break;
                    }

                    break;
                }
            }
        }

        /// <summary>
        ///     Reads a font character from the font file, and places it in a newly-allocated entry in the list.
        /// </summary>
        private void ReadFontChar( string[] lines, ref int index, char ch )
        {
            if (index > lines.Length)
                return;

            var fc = new FigletChar( ch, _charheight );

            for (int row = 0; row < _charheight; row++)
            {
                var line = lines[ index++ ].TrimEnd();
                if (line.Length > 0)
                {
                    var endchar = line[ ^1 ];
                    if (line[ ^2 ] == endchar)
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
            var resourceNames = GetType().Assembly.GetManifestResourceNames().ToList();
            if (resourceNames.Contains( resourceName ))
            {
                using var stream = GetType().Assembly.GetManifestResourceStream( resourceName );
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

            if (!fontFilename.EndsWith( ".flf" ))
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
            // a <a>            should always be `a', for now
            // $ <hardblank>    prints as a blank, but can't be smushed (optional)
            //  6 - <height of a character>
            //  5 - <height of a character, not including descenders> - ignored
            // 20 - <max character width> - ignored
            // 15 - <default SmushMode for this font>
            // 16 - <number of comment lines>
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
            if (items.Length >= 6)
                _ = Convert.ToInt32( items[ 6 ] ); // Print direction value is not implemented
            if (items.Length >= 7)
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
        /// Given an input character, executes re-mapping commands
        /// read from control files.  Returns re-mapped character (inchr).
        /// </summary>
        private char HandleMapping( char c )
        {
            for (int i = 0; i < _commandlist.Count; i++)
            {
                var cm = _commandlist[ i ];
                if (cm.Command != 0 && ( c >= cm.RangeLo && c <= cm.RangeHi ))
                {
                    c = (char)( c + cm.Offset );
                    while (++i < _commandlist.Count)
                    {
                        cm = _commandlist[ i ];

                        if (cm.Command == 0)
                            break;
                    }
                }
            }

            return c;
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
        public char SmushEm( char lch, char rch )
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
        public int SmushAmt()
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
                var ch2    = _currentFiglet.Lines[ row ][ charbd ];
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

    public class FigletCommand
    {
        public int Command;
        public int RangeLo;
        public int RangeHi;
        public int Offset;
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
