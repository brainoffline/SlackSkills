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
        private static List< string > _badWords;

        static BadWords()
        {
            var ass = Assembly.GetAssembly( typeof(BadWords) );
            var names = ass!.GetManifestResourceNames();

            using var stream = ass.GetManifestResourceStream("BrainSlack.Helpers.BadWords.json");
            using var sr     = new StreamReader( stream );
            var json = sr.ReadToEnd();

            _badWords = JsonConvert.DeserializeObject< List< string > >( json );
        }

        public static bool ContainsBadWords(string? text)
        {
            if (string.IsNullOrEmpty(text))
                return false;

            foreach (var badWord in _badWords)
            {
                if (text.Contains(badWord, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

    }
}
