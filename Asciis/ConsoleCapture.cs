using System;
using System.IO;
using System.Text;

namespace Asciis
{
    public class ConsoleCapture : IDisposable
    {
        public readonly  StringBuilder StringBuilder = new();
        private readonly TextWriter    _prevOut;
        private readonly TextWriter    _prevError;

        public ConsoleCapture()
        {
            _prevOut   = Console.Out;
            _prevError = Console.Error;

            var stringWriter = new StringWriter(StringBuilder);
            Console.SetOut(stringWriter);
            Console.SetError(stringWriter);
        }

        public string NextString()
        {
            var str = StringBuilder.ToString();
            StringBuilder.Clear();
            return str;
        }

        public void Dispose()
        {
            Console.SetOut(_prevOut);
            Console.SetError(_prevError);
        }
    }
}
