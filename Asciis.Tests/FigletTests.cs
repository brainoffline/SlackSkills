using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

// Some tests borrowed from 
// https://github.com/lukesampson/figlet/blob/master/figletlib/layout_test.go

namespace Asciis.Tests
{
    [TestClass]
    public class FigletTests
    {
        [ TestInitialize ]
        public void Init()
        {
		}


        [TestMethod]
		public void Test_smush_with_lch_empty_always_returns_rch()
        {
            var rchs = new [] { 'a', '!', '$' };
            foreach (var rch in rchs)
                TestSmushemAllSmushModes( ' ', rch, rch );
        }

        [TestMethod]
		public void Test_smush_with_rch_empty_always_returns_lch()
        {
            var lchs = new [] { 'a', '!', '$' };
            foreach (var lch in lchs)
                TestSmushemAllSmushModes( lch, ' ', lch );
		}

		[TestMethod]
		public void Test_smush_with_smush_not_set_returns_null()
        {
            TestSmush('|', '|', SmushMode.Unknown, '\0');
		}

		[TestMethod]
		public void Test_smush_universal()
        {
			// smush mode of SMSmush but not SMKern is universal smushing
            TestSmush('|', '$', SmushMode.Smush, '|');
            TestSmush('$', '|', SmushMode.Smush, '|');
            TestSmush('l', 'r', SmushMode.Smush, 'r');
		}

		[TestMethod]
		public void Test_smush_combines_2_hardblanks_when_SMHardBlank()
		{
            TestSmush('$', '$', SmushMode.Kern | SmushMode.Smush | SmushMode.Hardblank, '$');
		}

		[TestMethod]
		public void Test_smush_equal()
		{
            TestSmush('x', 'x', SmushMode.Kern | SmushMode.Smush | SmushMode.Equal, 'x');
		}

		[TestMethod]
		public void Test_smush_lowline()
        {
            var replacements = "|/\\[]{}()<>";
            foreach (var ch in replacements)
            {
                TestSmushLowLine( '_', ch, ch );
                TestSmushLowLine( ch, '_', ch );
            }
        }

		[TestMethod]
		public void Test_smush_heirarchy()
        {
            TestSmushHierarchy( '|', '\\', '\\' );
            TestSmushHierarchy( '}', '|',  '}' );

            TestSmushHierarchy( '/', '>',  '>' );
            TestSmushHierarchy( '{', '\\', '{' );

            TestSmushHierarchy( '[', '(', '(' );
            TestSmushHierarchy( '>', ']', '>' );

            TestSmushHierarchy( '}', ')', ')' );
            TestSmushHierarchy( '<', '{', '<' );

            TestSmushHierarchy( '(', '<', '<' );
            TestSmushHierarchy( '>', '(', '>' );
        }

		[TestMethod]
		public void Test_smush_pairs()
        {
            TestSmushPair( '[', ']', '|' );
            TestSmushPair( ']', '[', '|' );

            TestSmushPair( '(', ')', '|' );
            TestSmushPair( ')', '(', '|' );

            TestSmushPair( '{', '}', '|' );
            TestSmushPair( '}', '{', '|' );
        }

		[TestMethod]
		public void Test_smush_bigX()
        {
            var mode = SmushMode.Smush | SmushMode.Kern | SmushMode.Bigx;
            TestSmush( '/',  '\\', mode, '|' );
            TestSmush( '\\', '/',  mode, 'Y' );
            TestSmush( '>',  '<',  mode, 'X' );
        }

		[TestMethod]
        public void Test_smush_hardblank()
		{
            var mode = SmushMode.Smush | SmushMode.Kern | SmushMode.Hardblank;
            TestSmush( '$', '$', mode, '$' );
            TestSmush( ' ', '$', mode, '$' );
        }

		[TestMethod]
		public void Test_smushamt()
        {
            TestSmushamtLine( "|_ ", "  _", 3 );
            TestSmushamtLine( "",    "__",  0 );
        }

		[TestMethod]
		public void Test_addChar()
        {
            TestAddCharLine( "",    "",     "" );
            TestAddCharLine( "",    "__",   "__" );
            TestAddCharLine( "",    " __",  "__");
            TestAddCharLine( "|_ ", "  _",  "|__" );
            TestAddCharLine( "|_ ", "   _", "|__" );
        }

        [TestMethod]
		public void Test_smushChar()
        {
            testSmushCharLine( "/ ", "  / ", 4, "/ " );
            testSmushCharLine( "",   " _",   1, "_" );
        }

        void TestSmushLowLine(char lch, char rch, char expect) =>
            TestSmush(lch, rch, SmushMode.Kern | SmushMode.Smush | SmushMode.Underscore, expect);

        void TestSmushHierarchy(char lch, char rch, char expect) =>
            TestSmush(lch, rch, SmushMode.Kern | SmushMode.Smush | SmushMode.Hierarchy, expect);

        void TestSmushPair(char lch, char rch, char expect) =>
            TestSmush(lch, rch, SmushMode.Kern | SmushMode.Smush | SmushMode.Pair, expect);

        void TestSmush(char lch, char rch, SmushMode mode, char expect)
        {
            var figlet = new TestFiglet(smushMode: mode);
            var result = figlet.SmushEm(lch, rch);
            Assert.AreEqual(expect, result);
        }

        void TestSmushemAllSmushModes(char lch, char rch, char expect)
        {
            for (SmushMode smushMode = SmushMode.Equal; smushMode < SmushMode.Smush; smushMode++)
            {
                var figlet = new TestFiglet(smushMode: smushMode);
                var result = figlet.SmushEm(lch, rch);
                Assert.AreEqual(expect, result);
            }
        }
        void TestSmushamtLine(string lhs, string rhs, int expected)
        {
            // TODO
        }

        void TestAddCharLine(string lhs, string c, string expect)
        {
            // TODO
        }

        void testSmushCharLine(string lhs, string c, int amount, string expect)
        {
            // TODO
        }

        [TestMethod]
        public void Test_render()
        {
            var lines = Figlet.Render( "a" );
            var expected = @"       
  __ _ 
 / _` |
| (_| |
 \__,_|
       
";
            Assert.AreEqual(expected, lines);
        }

        [TestMethod]
        public void Test_render_alphabet()
        {
            var lines = Figlet.Render("abcdefghijklmnopqrstuvwxyz", smushMode: SmushMode.Kern);
            var expected = 
@"        _               _         __         _      _   _  _     _                                                    _                                             
  __ _ | |__    ___  __| |  ___  / _|  __ _ | |__  (_) (_)| | __| | _ __ ___   _ __    ___   _ __    __ _  _ __  ___ | |_  _   _ __   ____      ____  __ _   _  ____
 / _` || '_ \  / __|/ _` | / _ \| |_  / _` || '_ \ | | | || |/ /| || '_ ` _ \ | '_ \  / _ \ | '_ \  / _` || '__|/ __|| __|| | | |\ \ / /\ \ /\ / /\ \/ /| | | ||_  /
| (_| || |_) || (__| (_| ||  __/|  _|| (_| || | | || | | ||   < | || | | | | || | | || (_) || |_) || (_| || |   \__ \| |_ | |_| | \ V /  \ V  V /  >  < | |_| | / / 
 \__,_||_.__/  \___|\__,_| \___||_|   \__, ||_| |_||_|_/ ||_|\_\|_||_| |_| |_||_| |_| \___/ | .__/  \__, ||_|   |___/ \__| \__,_|  \_/    \_/\_/  /_/\_\ \__, |/___|
                                      |___/          |__/                                   |_|        |_|                                               |___/      
";
            Assert.AreEqual(expected, lines);
        }

        [TestMethod]
        public void Test_render_alphabet_mini()
        {
            
            var lines = Figlet.Render("abcdefghijklmnopqrstuvwxyz", Figlet.Fonts.Mini, SmushMode.Kern);
            var expected =
@"                    _                                                                     
 _. |_   _  _|  _ _|_ _  |_  o o |  | ._ _  ._   _  ._   _. ._ _ _|_                   _  
(_| |_) (_ (_| (/_ | (_| | | | | |< | | | | | | (_) |_) (_| | _>  |_ |_| \/ \/\/ >< \/ /_ 
                      _|      _|                    |     |                         /     
";
            Assert.AreEqual(expected, lines);
        }

        [TestMethod]
        public void Test_a()
        {
            var figlet = new Figlet();
            figlet.Add('a');

            var lines = figlet.Render();
            var expected = @"       
  __ _ 
 / _` |
| (_| |
 \__,_|
       
";
            Assert.AreEqual(expected, lines);
        }

        [TestMethod]
        public void Test_abcde()
        {
            var figlet = new Figlet();
            figlet.Add("abcdefg");

            var lines = figlet.Render();
            var expected =
@"       _             _       __       
  __ _| |__   ___ __| | ___ / _| __ _ 
 / _` | '_ \ / __/ _` |/ _ \ |_ / _` |
| (_| | |_) | (_| (_| |  __/  _| (_| |
 \__,_|_.__/ \___\__,_|\___|_|  \__, |
                                |___/ 
";
            Assert.AreEqual(expected, lines);
        }

        [TestMethod]
        public void Test_abcde_kerning()
        {
            var figlet = new Figlet( smushMode: SmushMode.Kern );
            figlet.Add("abcdefg");

            var lines = figlet.Render();
            var expected =
@"        _               _         __        
  __ _ | |__    ___  __| |  ___  / _|  __ _ 
 / _` || '_ \  / __|/ _` | / _ \| |_  / _` |
| (_| || |_) || (__| (_| ||  __/|  _|| (_| |
 \__,_||_.__/  \___|\__,_| \___||_|   \__, |
                                      |___/ 
";
            Assert.AreEqual(expected, lines);
        }

        [TestMethod]
        public void Test_words()
        {
            var figlet = new Figlet();
            figlet.Add("Word One!");

            var lines = figlet.Render();
            var expected =
@"__        __            _    ___             _ 
\ \      / /__  _ __ __| |  / _ \ _ __   ___| |
 \ \ /\ / / _ \| '__/ _` | | | | | '_ \ / _ \ |
  \ V  V / (_) | | | (_| | | |_| | | | |  __/_|
   \_/\_/ \___/|_|  \__,_|  \___/|_| |_|\___(_)
                                               
";
            Assert.AreEqual(expected, lines);
        }

        [TestMethod]
        public void Test_words_Kern()
        {
            var figlet = new Figlet( smushMode: SmushMode.Kern);
            figlet.Add("Word One!");

            var lines = figlet.Render();
            var expected =
@"__        __               _    ___                _ 
\ \      / /___   _ __  __| |  / _ \  _ __    ___ | |
 \ \ /\ / // _ \ | '__|/ _` | | | | || '_ \  / _ \| |
  \ V  V /| (_) || |  | (_| | | |_| || | | ||  __/|_|
   \_/\_/  \___/ |_|   \__,_|  \___/ |_| |_| \___|(_)
                                                     
";
            Assert.AreEqual(expected, lines);
        }

        [TestMethod]
        public void Test_words_Mini_Font()
        {
            var figlet = new Figlet(fontFile: Figlet.Fonts.Mini, smushMode: SmushMode.Kern);
            figlet.Add("Word One!");

            var lines = figlet.Render();
            var expected =
@"                   _            
\    / _  ._ _|   / \ ._   _  | 
 \/\/ (_) | (_|   \_/ | | (/_ o 
                                
";
            Assert.AreEqual(expected, lines);
        }

        [TestMethod]
        public void Test_words_Big_Font()
        {
            var figlet = new Figlet(fontFile: Figlet.Fonts.Big, smushMode: SmushMode.Kern);
            figlet.Add("Word One!");

            var lines = figlet.Render();
            var expected =
@"__          __              _    ____                _ 
\ \        / /             | |  / __ \              | |
 \ \  /\  / /___   _ __  __| | | |  | | _ __    ___ | |
  \ \/  \/ // _ \ | '__|/ _` | | |  | || '_ \  / _ \| |
   \  /\  /| (_) || |  | (_| | | |__| || | | ||  __/|_|
    \/  \/  \___/ |_|   \__,_|  \____/ |_| |_| \___|(_)
                                                       
                                                       
";
            Assert.AreEqual(expected, lines);
        }

        [TestMethod]
        public void Test_words_Slant_Font()
        {
            var figlet = new Figlet(fontFile: Figlet.Fonts.Slant, smushMode: SmushMode.Kern);
            figlet.Add("Word One!");

            var lines = figlet.Render();
            var expected =
@" _       __                  __   ____                __
| |     / /____   _____ ____/ /  / __ \ ____   ___   / /
| | /| / // __ \ / ___// __  /  / / / // __ \ / _ \ / / 
| |/ |/ // /_/ // /   / /_/ /  / /_/ // / / //  __//_/  
|__/|__/ \____//_/    \__,_/   \____//_/ /_/ \___/(_)   
                                                        
";
            Assert.AreEqual(expected, lines);
        }

        [TestMethod]
        public void Test_words_Block_Font()
        {
            var figlet = new Figlet(fontFile: Figlet.Fonts.Block, smushMode: SmushMode.Kern);
            figlet.Add("Word One!");

            var lines = figlet.Render();
            var expected =
                @"                                                                                    
_|          _|                            _|        _|_|                        _|  
_|          _|    _|_|    _|  _|_|    _|_|_|      _|    _|  _|_|_|      _|_|    _|  
_|    _|    _|  _|    _|  _|_|      _|    _|      _|    _|  _|    _|  _|_|_|_|  _|  
  _|  _|  _|    _|    _|  _|        _|    _|      _|    _|  _|    _|  _|            
    _|  _|        _|_|    _|          _|_|_|        _|_|    _|    _|    _|_|_|  _|  
                                                                                    
                                                                                    
";
            Assert.AreEqual(expected, lines);
        }

        [TestMethod]
        public void Test_words_Lean_Font()
        {
            var figlet = new Figlet(fontFile: Figlet.Fonts.Lean, smushMode: SmushMode.Kern);
            figlet.Add("Word One!");

            var lines = figlet.Render();
            var expected =
                @"                                                                                       
  _/          _/                            _/        _/_/                        _/   
 _/          _/    _/_/    _/  _/_/    _/_/_/      _/    _/  _/_/_/      _/_/    _/    
_/    _/    _/  _/    _/  _/_/      _/    _/      _/    _/  _/    _/  _/_/_/_/  _/     
 _/  _/  _/    _/    _/  _/        _/    _/      _/    _/  _/    _/  _/                
  _/  _/        _/_/    _/          _/_/_/        _/_/    _/    _/    _/_/_/  _/       
                                                                                       
                                                                                       
";
            Assert.AreEqual(expected, lines);
        }

        [TestMethod]
        public void Test_words_Varsity_Font()
        {
            var figlet = new Figlet(fontFile: Figlet.Fonts.Varsity, smushMode: SmushMode.Kern);
            figlet.Add("Word One!");

            var lines = figlet.Render();
            var expected =
@" ____      ____                     __     ___                   _  
|_  _|    |_  _|                   |  ]  .'   `.                | | 
  \ \  /\  / / .--.   _ .--.   .--.| |  /  .-.  \ _ .--.  .---. | | 
   \ \/  \/ // .'`\ \[ `/'`\]/ /'`\' |  | |   | |[ `.-. |/ /__\\| | 
    \  /\  / | \__. | | |    | \__/  |  \  `-'  / | | | || \__.,|_| 
     \/  \/   '.__.' [___]    '.__.;__]  `.___.' [___||__]'.__.'(_) 
                                                                    
";
            Assert.AreEqual(expected, lines);
        }

        [TestMethod]
        public void Test_All_Fonts()
        {
            var names = Figlet.FontNames();
            var sb    = new StringBuilder();

            foreach (var fontName in names)
            {
                sb.AppendLine( fontName );
                sb.AppendLine( Figlet.Render( fontName, fontName, SmushMode.Kern ) );
            }

            Console.Write( sb.ToString() );
            Assert.IsTrue( true );
        }

    }

    public class TestFiglet : Figlet
    {
        public TestFiglet( string?        fontFile       = null,
                       Justification? justification  = null,
                       int?           maxOutputWidth = null,
                       SmushMode?     smushMode      = null )
            : base(fontFile, justification, maxOutputWidth, smushMode)
        { }

        public new char SmushEm( char lch, char rch ) =>
            base.SmushEm( lch, rch );
    }
}
