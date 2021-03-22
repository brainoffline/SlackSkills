using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Asciis
{


    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    ///     #define DATE "31 May 2012"
    ///     #define VERSION "2.2.5"
    ///     #define VERSION_INT 20205
    /// </remarks>
    public class FigletApp
    {
        private const string DefaultFontDir = "fonts";
        private const string DefaultFontFile = "standard.flf";
        private const string FontFileSuffix = ".flf";
        private const string FontFileMagicNumber = "flf2";
        private const string ControlFileSuffix = ".flc";
        private const string ControlFileMagicNumber = "flc2"; /* no longer used in 2.2 */
        private const int DefaultColumns = 0;
        private const int Maxlen = 255; /* Maximum character width */

        string inchrline; /* Alloc'd inchr inchrline[inchrlinelenlimit+1]; */
        /* Note: not null-terminated. */
        int inchrlinelen, inchrlinelenlimit;
        int[] deutsch = { 196, 214, 220, 228, 246, 252, 223 };
        /* Latin-1 codes for German letters, respectively:
             LATIN CAPITAL LETTER A WITH DIAERESIS = A-umlaut
             LATIN CAPITAL LETTER O WITH DIAERESIS = O-umlaut
             LATIN CAPITAL LETTER U WITH DIAERESIS = U-umlaut
             LATIN SMALL LETTER A WITH DIAERESIS = a-umlaut
             LATIN SMALL LETTER O WITH DIAERESIS = o-umlaut
             LATIN SMALL LETTER U WITH DIAERESIS = u-umlaut
             LATIN SMALL LETTER SHARP S = ess-zed
          */

        int hzmode;   /* true if reading double-bytes in HZ mode */
        int[] gndbl = new int[4]; /* gndbl[n] is true if Gn is double-byte */
        int[] gn = new int[4];  /* Gn character sets: ASCII, Latin-1, none, none */
        int gl;       /* 0-3 specifies left-half Gn character set */
        int gr;       /* 0-3 specifies right-half Gn character set */


        struct fc
        {
            int ord;
            string[] thechar; /* Alloc'd char thechar[charheight][]; */
        }

        private List< fc > fcharlist = new List< fc >();
        outchr**           currchar;
        int                currcharwidth;
        int                previouscharwidth;
        List<string>       outputline = new(); /* Alloc'd char outputline[charheight][outlinelenlimit+1]; */
        int                outlinelen;

        /****************************************************************************
         Globals dealing with command file storage
        ****************************************************************************/

        struct cfn
        {
            string thename;
        }
        List<cfn> cfilelist = new();

        struct cm
        {
            int thecommand;
            int rangelo;
            int rangehi;
            int offset;
        }

        List<cm> commandlist = new();

        /****************************************************************************
          Globals affected by command line options
        ****************************************************************************/

        private const int SM_SMUSH     = 128;
        private const int SM_KERN      = 64;
        private const int SM_EQUAL     = 1;
        private const int SM_LOWLINE   = 2;
        private const int SM_HIERARCHY = 4;
        private const int SM_PAIR      = 8;
        private const int SM_BIGX      = 16;
        private const int SM_HARDBLANK = 32;

        int smushmode;

        enum SmushOverride
        {
            No,
            Yes,
            Force
        }

        enum Justification
        {
            Left, Centered, Right
        }

        SmushOverride         smushoverride;
        private Justification justification;

        int   outputwidth;
        int   outlinelenlimit;
        string fontDirName, fontName;

        /****************************************************************************
        
          Globals read from font file
        
        ****************************************************************************/

        char hardblank;
        int  charheight;





        public class FigletParameters
        {
            [Param( "f", "Select the font. If not specified, then the default font is applied")]
            public string? FontFile { get; set; }

            [Param("d|FIGLET_FONTDIR", "Select the font directory")]
            public string? FontDir { get; set; }

            [Param( "c", "Centred horizontally")]
            public bool CenteredHorz { get; set; }

            [Param( "l", "left justified")]
            public bool LeftJustified { get; set; }

            [Param("r", "Right Justified")]
            public bool RightJustified { get; set; }

            [ Param( "w", "Output Width. If no width is supplied then text does not flow" ) ]
            public int Width { get; set; }

            [Param("C", "Control file")]
            public string? ControlFile { get; set; }

            [Param( "s", "Smushing")]
            public bool Smushing { get; set; }

            [Param( "S", "Force Smushing")]
            public bool ForceSmushing { get; set; }

            [Param( "k", "Kerning")]
            public bool Kerning { get; set; }

            [Param( "o", "Overlapped")]
            public bool Overlapped { get; set; }

            [Param("W", "Full width characters")]
            public bool FullCharacter { get; set; }
        }

        void getparams( FigletParameters parms )
        {
            smushoverride = SmushOverride.No;
            justification = Justification.Left;
            outputwidth   = DefaultColumns;
            gn[1]         = 0x80;
            gr            = 1;

            fontDirName = parms.FontDir  ?? DefaultFontDir;
            fontName    = parms.FontFile ?? DefaultFontFile;
            var controlname = parms.ControlFile;

            if (parms.Kerning)
            {
                smushmode     = SM_KERN;
                smushoverride = SmushOverride.No;
            }
            if (parms.ForceSmushing)
            {
                smushmode     = SM_SMUSH;
                smushoverride = SmushOverride.Force;
            }
            if (parms.Overlapped)
            {
                smushmode     = SM_SMUSH;
                smushoverride = SmushOverride.Yes;
            }
            if (parms.Width > 0)
            {
                smushmode     = 0;
                smushoverride = SmushOverride.Yes;
            }
            if (parms.LeftJustified)
                justification = Justification.Left;
            else if (parms.CenteredHorz)
                justification = Justification.Centered;
            else if (parms.RightJustified)
                justification = Justification.Right;
        }

        /****************************************************************************

          readcontrol

          Allocates memory and reads in the given control file.
          Called in readcontrolfiles().

        ****************************************************************************/

        void readcontrolfile(string controlname)
        {
            int firstch, lastch;
            char dashcheck;
            int offset;
            char command;

            var data = File.ReadAllText( controlname );

            if (data == null)
                return;

            for (int i = 0; i < data.Length; i++)
            {
                command = data[ i ];
                switch (command)
                {
                    case 't':
                        SkipWhiteSpace( ref i );
                        break;
                }
            }

            void SkipWhiteSpace(ref int i)
            {
                while (i < data.Length && char.IsWhiteSpace( data[ i ] ))
                    i++;
            }

            char ReadTChar( ref int i )
            {
                var ch = data[ i ];

                if (ch == '\n' || ch == '\r')   // badly formatted file
                    return '\0';
                i++;
                if (ch != '\\')
                {
                    return ch;
                }
                var next = data[ i++ ];
                switch (next)
                {
                    case 'a': return (char)7;
                    case 'b': return (char)8;
                    case 'e': return (char)27;
                    case 'f': return (char)12;
                    case 'n': return (char)10;
                    case 'r': return (char)13;
                    case 't': return (char)9;
                    case 'v': return (char)11;
                    default:
                        if (next == '-' || next == 'x' || ( next >= 0 && next <= '9' ))
                        {
                            i--;
                            return (char)ReadNum( ref i );
                        }

                        return next;

                }
            }

            int ReadNum( ref int i )
            {
                int   acc = 0;
                int numBase;
                int sign = 1;
                char[] digits = {'0','1','2','3','4','5','6','7','8','9','A','B','C','D','E','F'};

                SkipWhiteSpace( ref i );
                char c = data[ i++ ];

                if (c == '-')
                    sign = -1;
                else
                    i--;
                c = data[i++];
                if (c == '0')
                {
                    c = data[i++];
                    if (c == 'x' || c == 'X')
                        numBase = 16;
                    else
                    {
                        numBase = 8;
                        i--;
                    }
                }
                else
                {
                    numBase = 10;
                    i--;
                }

                while (i < data.Length)
                {
                    c   = char.ToUpper( data[ i++ ] );
                    num = acc * sign;
                }
                while ((c = Zgetc(fp)) != EOF)
                {
                    c = toupper(c);
                    p = strchr(digits, c);
                    if (!p)
                    {
                        Zungetc(c, fp);
                        *nump = acc * sign;
                        return;
                    }
                    acc = acc * base + (p - digits);
                }
                *nump = acc * sign;
            }

  while (command = Zgetc(controlfile), command != EOF)
  {
    switch (command)
    {
    case 't': /* Translate */
      skipws(controlfile);
    firstch = readTchar(controlfile);
      if ((dashcheck = Zgetc(controlfile)) == '-')
      {
        lastch = readTchar(controlfile);
}
      else
{
    Zungetc(dashcheck, controlfile);
    lastch = firstch;
}
skipws(controlfile);
offset = readTchar(controlfile) - firstch;
skiptoeol(controlfile);
(*commandlistend) = (comnode*)myalloc(sizeof(comnode));
(*commandlistend)->thecommand = 1;
(*commandlistend)->rangelo = firstch;
(*commandlistend)->rangehi = lastch;
(*commandlistend)->offset = offset;
commandlistend = &(*commandlistend)->next;
(*commandlistend) = NULL;
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
      /* Mapping table entry */
      Zungetc(command, controlfile);
readnum(controlfile, &firstch);
skipws(controlfile);
readnum(controlfile, &lastch);
offset = lastch - firstch;
lastch = firstch;
skiptoeol(controlfile);
(*commandlistend) = (comnode*)myalloc(sizeof(comnode));
(*commandlistend)->thecommand = 1;
(*commandlistend)->rangelo = firstch;
(*commandlistend)->rangehi = lastch;
(*commandlistend)->offset = offset;
commandlistend = &(*commandlistend)->next;
(*commandlistend) = NULL;
break;
    case 'f': /* freeze */
      skiptoeol(controlfile);
(*commandlistend) = (comnode*)myalloc(sizeof(comnode));
(*commandlistend)->thecommand = 0;
commandlistend = &(*commandlistend)->next;
(*commandlistend) = NULL;
break;
    case 'b': /* DBCS input mode */
      multibyte = 1;
break;
    case 'u': /* UTF-8 input mode */
      multibyte = 2;
break;
    case 'h': /* HZ input mode */
      multibyte = 3;
break;
    case 'j': /* Shift-JIS input mode */
      multibyte = 4;
break;
    case 'g': /* ISO 2022 character set choices */
      multibyte = 0;
skipws(controlfile);
command = Zgetc(controlfile);
switch (command)
{
    case '0': /* define G0 charset */
        charset(0, controlfile);
        break;
    case '1': /* set G1 charset */
        charset(1, controlfile);
        break;
    case '2': /* set G2 charset */
        charset(2, controlfile);
        break;
    case '3': /* set G3 charset */
        charset(3, controlfile);
        break;
    case 'l':
    case 'L': /* define left half */
        skipws(controlfile);
        gl = Zgetc(controlfile) - '0';
        skiptoeol(controlfile);
        break;
    case 'r':
    case 'R': /* define right half */
        skipws(controlfile);
        gr = Zgetc(controlfile) - '0';
        skiptoeol(controlfile);
        break;
    default: /* meaningless "g" command */
        skiptoeol(controlfile);
}
    case '\r':
    case '\n': /* blank line */
      break;
default: /* Includes '#' */
      skiptoeol(controlfile);
    }
  }
  Zclose(controlfile);
}

        static Task Main(string[] args)
        {
            var app = new FigletApp();
            return app.DoIt(args);
        }

        Task DoIt(string[] args)
        {
            int c, c2;
            int i;
            int last_was_eol_flag;

            /*---------------------------------------------------------------------------
            wordbreakmode:
              -1: /^$/ and blanks are to be absorbed (when line break was forced
                by a blank or character larger than outlinelenlimit)
              0: /^ *$/ and blanks are not to be absorbed
              1: /[^ ]$/ no word break yet
              2: /[^ ]  *$/
              3: /[^ ]$/ had a word break
          ---------------------------------------------------------------------------*/
            int wordbreakmode;
            int char_not_added;

            var parms = ParamParser< FigletParameters >.Parse(args);

            getparams(parms);

            readcontrolfile();
            readfont();
            linealloc();

            wordbreakmode = 0;
            last_was_eol_flag = 0;

            while ((c = getint()) != EOF)
            {

                if (c == '\n' && paragraphflag && !last_was_eol_flag)
                {
                    ungetint(c2 = getint());
                    c = ((isascii(c2) && isspace(c2)) ? '\n' : ' ');
                }
                last_was_eol_flag = (isascii(c) && isspace(c) && c != '\t' && c != ' ');

                if (deutschflag)
                {
                    if (c >= '[' && c <= ']')
                    {
                        c = deutsch[c - '['];
                    }
                    else if (c >= '{' && c <= '~')
                    {
                        c = deutsch[c - '{' + 3];
                    }
                }

                c = handlemapping(c);

                if (isascii(c) && isspace(c))
                {
                    c = (c == '\t' || c == ' ') ? ' ' : '\n';
                }

                if ((c > '\0' && c < ' ' && c != '\n') || c == 127)
                    continue;

                /*
              Note: The following code is complex and thoroughly tested.
              Be careful when modifying!
            */

                do
                {
                    char_not_added = 0;

                    if (wordbreakmode == -1)
                    {
                        if (c == ' ')
                        {
                            break;
                        }
                        else if (c == '\n')
                        {
                            wordbreakmode = 0;
                            break;
                        }
                        wordbreakmode = 0;
                    }

                    if (c == '\n')
                    {
                        printline();
                        wordbreakmode = 0;
                    }

                    else if (addchar(c))
                    {
                        if (c != ' ')
                        {
                            wordbreakmode = (wordbreakmode >= 2) ? 3 : 1;
                        }
                        else
                        {
                            wordbreakmode = (wordbreakmode > 0) ? 2 : 0;
                        }
                    }

                    else if (outlinelen == 0)
                    {
                        for (i = 0; i < charheight; i++)
                        {
                            if (right2left && outputwidth > 1)
                            {
                                putstring(currchar[i] + STRLEN(currchar[i]) - outlinelenlimit);
                            }
                            else
                            {
                                putstring(currchar[i]);
                            }
                        }
                        wordbreakmode = -1;
                    }

                    else if (c == ' ')
                    {
                        if (wordbreakmode == 2)
                        {
                            splitline();
                        }
                        else
                        {
                            printline();
                        }
                        wordbreakmode = -1;
                    }

                    else
                    {
                        if (wordbreakmode >= 2)
                        {
                            splitline();
                        }
                        else
                        {
                            printline();
                        }
                        wordbreakmode = (wordbreakmode == 3) ? 1 : 0;
                        char_not_added = 1;
                    }

                } while (char_not_added);
            }

            if (outlinelen != 0)
            {
                printline();
            }
            return 0;
        }
    }
}