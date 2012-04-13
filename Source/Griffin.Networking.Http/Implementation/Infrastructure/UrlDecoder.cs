using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Griffin.Networking.Http.Implementation.Infrastructure
{
    public class ReaderResult
    {
        public ReaderResult()
        {
            Value = "";
        }
        public string Value { get; set; }
        public char Delimiter { get; set; }
    }
    public static class TextReaderExtensions
    {
        
        public static ReaderResult ReadToEnd(this TextReader reader, string delimiters)
        {
            var result = new ReaderResult();

            int intChar = reader.Peek();    
            while (intChar != -1 )
            {
                var ch = (char) intChar;
                result.Delimiter = ch;

                if (delimiters.IndexOf(ch) == -1)
                {
                    result.Value += (char) reader.Read();
                    continue;
                }

                break;
            }

            if (intChar == -1)
                result.Delimiter = char.MinValue;

            return result;
        }

    }

    /// <summary>
    /// Parses query string
    /// </summary>
    public class UrlDecoder
    {
        /// <summary>
        /// Parse a query string
        /// </summary>
        /// <param name="reader">string to parse</param>
        /// <param name="form"> </param>
        /// <returns>A collection</returns>
        /// <exception cref="ArgumentNullException"><c>reader</c> is <c>null</c>.</exception>
        public void Parse(TextReader reader, IParameterCollection parameters)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            int intChar;
            while ((intChar = reader.Read()) != -1)
            {
                var ch = (char) intChar;

                var result = reader.ReadToEnd("&=");
                string name = Uri.UnescapeDataString(result.Value);
                if (result.Delimiter != char.MinValue)
                    reader.Read();

                switch (result.Delimiter)
                {
                    case '&':
                        parameters.Add(name, string.Empty);
                        break;
                    case '=':
                        {
                            result = reader.ReadToEnd("&");
                            if (result.Delimiter != char.MinValue)
                                reader.Read();
                            parameters.Add(name, Uri.UnescapeDataString(result.Value));
                        }
                        break;
                    default:
                        parameters.Add(name, string.Empty);
                        break;
                }
            }
        }

        /// <summary>
        /// Parse a query string
        /// </summary>
        /// <param name="queryString">string to parse</param>
        /// <returns>A collection</returns>
        /// <exception cref="ArgumentNullException"><c>queryString</c> is <c>null</c>.</exception>
        public IParameterCollection Parse(string queryString)
        {
            if (queryString == null)
                throw new ArgumentNullException("queryString");
            if (queryString.Length == 0)
                return new ParameterCollection();

            var reader = new StringReader(queryString);
            var col = new ParameterCollection();
            Parse(reader, col);
            return col;
        }
    }
}
