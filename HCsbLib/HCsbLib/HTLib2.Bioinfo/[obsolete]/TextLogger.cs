using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using TextWriter = System.IO.TextWriter;
    public interface ITextLogger
    {
        void Log(string message);
    }
    public class TextLogger : ITextLogger
    {
        object writter;
        HashSet<string> messages = new HashSet<string>();
        public TextLogger()
        {
            this.writter = System.Console.Error;
        }
        public TextLogger(object writter)
        {
            HDebug.AssertOr
                ( writter == null
                , writter is TextWriter
                , writter is StringBuilder
                , writter is List<string>
                );
            this.writter = writter;
        }
        public void Log(string message)
        {
            if(messages.Add(message) == false)
                return; // alread the message is printed
            if(writter == null)
                return;
            if(writter is TextWriter   ) { (writter as TextWriter   ).WriteLine (message); return; }
            if(writter is StringBuilder) { (writter as StringBuilder).AppendLine(message); return; }
            if(writter is List<string> ) { (writter as List<string> ).Add       (message); return; }
            HDebug.Assert(false);
        }
    }
}
