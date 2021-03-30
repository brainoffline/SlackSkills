using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace BrainSlack.Helpers
{
    public class BadWords
    {
        private static readonly List< string > BadWordList;

        static BadWords()
        {
            var ass = Assembly.GetAssembly( typeof(BadWords) );
            var names = ass!.GetManifestResourceNames();

            using var stream = ass.GetManifestResourceStream("BrainSlack.Helpers.BadWords.json");
            using var sr     = new StreamReader( stream ?? throw new InvalidOperationException() );
            var       json   = sr.ReadToEnd();

            BadWordList = JsonConvert.DeserializeObject< List< string > >( json );
        }

        public static bool ContainsBadWords(string? text)
        {
            if (string.IsNullOrEmpty(text))
                return false;

            foreach (var badWord in BadWordList)
            {
                if (text.Contains(badWord, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

    }
}
