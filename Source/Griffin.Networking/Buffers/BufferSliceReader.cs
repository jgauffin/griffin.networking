using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Griffin.Networking.Buffers
{
    /// <summary>
    /// Read text from a buffer slice.
    /// </summary>
    public class BufferSliceReader
    {
        private readonly Encoding _encoding;
        private BufferSlice _slice;

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferSliceReader"/> class.
        /// </summary>
        public BufferSliceReader()
        {
            _encoding = Encoding.ASCII;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferSliceReader"/> class.
        /// </summary>
        /// <param name="encoding">Encoding to use when converting byte array to strings.</param>
        public BufferSliceReader(Encoding encoding)
        {
            _encoding = encoding;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferSliceReader"/> class.
        /// </summary>
        /// <param name="buffer">Buffer to read from.</param>
        /// <param name="offset">Where in buffer to start reading</param>
        /// <param name="count">Number of bytes that can be read.</param>
        /// <param name="encoding">Encoding to use when converting byte array to strings.</param>
        public BufferSliceReader(byte[] buffer, int offset, int count, Encoding encoding)
        {
            _slice = new BufferSlice(buffer, offset, count, count);
            _encoding = encoding;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferSliceReader"/> class.
        /// </summary>
        public BufferSliceReader(BufferSlice slice)
        {
            _slice = slice;
            _encoding = Encoding.ASCII;
        }

        public byte[] Buffer
        {
            get { return _slice.Buffer; }
        }

        /// <summary>
        /// Gets current character
        /// </summary>
        /// <value><see cref="char.MinValue"/> if end of buffer.</value>
        public char Current
        {
            get { return HasMore ? (char) _slice.Buffer[_slice.CurrentOffset] : char.MinValue; }
        }

        /// <summary>
        /// Gets if end of buffer have been reached
        /// </summary>
        /// <value></value>
        public bool EndOfFile
        {
            get { return _slice.RemainingLength <= 0; }
        }

        /// <summary>
        /// Gets if more bytes can be processed.
        /// </summary>
        /// <value></value>
        public bool HasMore
        {
            get { return _slice.RemainingLength > 0; }
        }

        /// <summary>
        /// Gets or sets current position in buffer.
        /// </summary>
        /// <remarks>
        /// THINK before you manually change the position since it can blow up
        /// the whole parsing in your face.
        /// </remarks>
        public int Index
        {
            get { return _slice.CurrentOffset; }
            set { _slice.CurrentOffset = value; }
        }

        /// <summary>
        /// Gets total length of buffer.
        /// </summary>
        /// <value></value>
        public int Length
        {
            get { return _slice.Count; }
        }

        /// <summary>
        /// Gets next character
        /// </summary>
        /// <value><see cref="char.MinValue"/> if end of buffer.</value>
        public char Peek
        {
            get { return _slice.RemainingLength > 0 ? (char) _slice.Buffer[Index + 1] : char.MinValue; }
        }

        /// <summary>
        /// Gets number of bytes left.
        /// </summary>
        public int RemainingLength
        {
            get { return _slice.RemainingLength; }
        }

        /// <summary>
        /// Consume current character.
        /// </summary>
        public void Consume()
        {
            ++Index;
        }

        /// <summary>
        /// Consume specified characters
        /// </summary>
        /// <param name="chars">One or more characters.</param>
        public void Consume(params char[] chars)
        {
            while (HasMore && chars.Contains(Current))
                ++Index;
        }

        /// <summary>
        /// Consume all characters until the specified one have been found.
        /// </summary>
        /// <param name="delimiter">Stop when the current character is this one</param>
        /// <returns>New offset.</returns>
        public int ConsumeUntil(char delimiter)
        {
            if (EndOfFile)
                return Index;

            var startIndex = Index;

            while (true)
            {
                if (EndOfFile)
                {
                    Index = startIndex;
                    return Index;
                }

                if (Current == delimiter)
                    return Index;

                // Delimiter is not new line and we got one.
                if (delimiter != '\r' && delimiter != '\n' && Current == '\r' || Current == '\n')
                    throw new InvalidOperationException("Unexpected new line: " + GetString(startIndex, Index) +
                                                        "[CRLF].");

                ++Index;
            }
        }

        /// <summary>
        /// Consumes horizontal white spaces (space and tab).
        /// </summary>
        public void ConsumeWhiteSpaces()
        {
            Consume(' ', '\t');
        }

        /// <summary>
        /// Consume horizontal white spaces and the specified character.
        /// </summary>
        /// <param name="extraCharacter">Extra character to consume</param>
        public void ConsumeWhiteSpaces(char extraCharacter)
        {
            Consume(' ', '\t', extraCharacter);
        }

        /// <summary>
        /// Checks if one of the remaining bytes are a specified character.
        /// </summary>
        /// <param name="ch">Character to find.</param>
        /// <returns>
        /// 	<c>true</c> if found; otherwise <c>false</c>.
        /// </returns>
        public bool Contains(char ch)
        {
            var index = Index;
            while (Current != ch && HasMore)
                ++Index;
            var found = Current == ch;
            Index = index;
            return found;
        }

        /// <summary>
        /// Read a character.
        /// </summary>
        /// <returns>
        /// Character if not EOF; otherwise <c>null</c>.
        /// </returns>
        public char Read()
        {
            return (char) _slice.Buffer[Index++];
        }

        /// <summary>
        /// Get a text line. 
        /// </summary>
        /// <returns></returns>
        /// <remarks>Will merge multi line headers and rewind of end of line was not found.</remarks> 
        public string ReadLine()
        {
            var startIndex = Index;
            while (HasMore && Current != '\n')
                Consume();

            // EOF? Then we havent enough bytes.
            if (EndOfFile)
            {
                Index = startIndex;
                return null;
            }


            var thisLine = _encoding.GetString(_slice.Buffer, startIndex, Index - startIndex - 1);

            Consume(); // eat \n too.

            // Multi line message?
            if (Current == '\t' || Current == ' ')
            {
                Consume();
                var extra = ReadLine();

                // Multiline isn't complete, wait for more bytes.
                if (extra == null)
                {
                    Index = startIndex;
                    return null;
                }

                return thisLine + " " + extra.TrimStart(' ', '\t');
            }

            return thisLine;
        }

        /// <summary>
        /// Read quoted string
        /// </summary>
        /// <returns>string if current character (in buffer) is a quote; otherwise <c>null</c>.</returns>
        public string ReadQuotedString()
        {
            if (Current != '\"')
                return null;

            Consume();
            var startPos = Index;
            while (HasMore)
            {
                switch (Current)
                {
                    case '\\':
                        if (Peek == '"') // escaped quote
                        {
                            Consume();
                            Consume();
                            continue;
                        }
                        break;
                    case '"':
                        var value = _encoding.GetString(_slice.Buffer, startPos, Index - startPos);
                        ++Index; // skip qoute
                        return value;
                    default:
                        Consume();
                        break;
                }
            }

            Index = startPos;
            return null;
        }

        /// <summary>
        /// Read until end of string, or to one of the delimiters are found.
        /// </summary>
        /// <param name="delimiters">characters to stop at</param>
        /// <returns>
        /// A string (can be <see cref="string.Empty"/>).
        /// </returns>
        /// <remarks>
        /// Will not consume the delimiter.
        /// </remarks>
        /// <exception cref="InvalidOperationException"><c>InvalidOperationException</c>.</exception>
        public string ReadToEnd(string delimiters)
        {
            if (EndOfFile)
                return string.Empty;

            var startIndex = Index;

            var isDelimitersNewLine = delimiters.IndexOfAny(new[] {'\r', '\n'}) != -1;
            while (true)
            {
                if (EndOfFile)
                    return GetString(startIndex, Index);

                if (delimiters.IndexOf(Current) != -1)
                    return GetString(startIndex, Index, true);

                // Delimiter is not new line and we got one.
                if (isDelimitersNewLine && Current == '\r' || Current == '\n')
                    throw new InvalidOperationException("Unexpected new line: " + GetString(startIndex, Index) +
                                                        "[CRLF].");

                ++Index;
            }
        }

        /// <summary>
        /// Read until end of string, or to one of the delimiters are found.
        /// </summary>
        /// <returns>A string (can be <see cref="string.Empty"/>).</returns>
        /// <remarks>
        /// Will not consume the delimiter.
        /// </remarks>
        public string ReadToEnd()
        {
            var str = _encoding.GetString(_slice.Buffer, Index, RemainingLength);
            Index = _slice.StartOffset + _slice.Count;
            return str;
        }

        /// <summary>
        /// Read to end of buffer, or until specified delimiter is found.
        /// </summary>
        /// <param name="delimiter">Delimiter to find.</param>
        /// <returns>
        /// A string (can be <see cref="string.Empty"/>).
        /// </returns>
        /// <remarks>
        /// Will not consume the delimiter.
        /// </remarks>
        /// <exception cref="InvalidOperationException"><c>InvalidOperationException</c>.</exception>
        public string ReadToEnd(char delimiter)
        {
            if (EndOfFile)
                return string.Empty;

            var startIndex = Index;

            while (true)
            {
                if (EndOfFile)
                    return GetString(startIndex, Index);

                if (Current == delimiter)
                    return GetString(startIndex, Index, true);

                // Delimiter is not new line and we got one.
                if (delimiter != '\r' && delimiter != '\n' && Current == '\r' || Current == '\n')
                    throw new InvalidOperationException("Unexpected new line: " + GetString(startIndex, Index) +
                                                        "[CRLF].");

                ++Index;
            }
        }

        /// <summary>
        /// Will read until specified delimiter is found.
        /// </summary>
        /// <param name="delimiter">Character to stop at.</param>
        /// <returns>
        /// A string if the delimiter was found; otherwise <c>null</c>.
        /// </returns>
        /// <remarks>
        /// Will trim away spaces and tabs from the end.</remarks>
        /// <exception cref="InvalidOperationException"><c>InvalidOperationException</c>.</exception>
        public string ReadUntil(char delimiter)
        {
            if (EndOfFile)
                return null;

            var startIndex = Index;

            while (true)
            {
                if (EndOfFile)
                {
                    Index = startIndex;
                    return null;
                }

                if (Current == delimiter)
                    return GetString(startIndex, Index, true);

                // Delimiter is not new line and we got one.
                if (delimiter != '\r' && delimiter != '\n' && Current == '\r' || Current == '\n')
                    throw new InvalidOperationException("Unexpected new line: " + GetString(startIndex, Index) +
                                                        "[CRLF].");

                ++Index;
            }
        }

        /// <summary>
        /// Read until one of the delimiters are found.
        /// </summary>
        /// <param name="delimiters">characters to stop at</param>
        /// <returns>
        /// A string if one of the delimiters was found; otherwise <c>null</c>.
        /// </returns>
        /// <remarks>
        /// Will not consume the delimiter.
        /// </remarks>
        /// <exception cref="InvalidOperationException"><c>InvalidOperationException</c>.</exception>
        public string ReadUntil(string delimiters)
        {
            if (EndOfFile)
                return null;

            var startIndex = Index;

            var isDelimitersNewLine = delimiters.IndexOfAny(new[] {'\r', '\n'}) != -1;
            while (true)
            {
                if (EndOfFile)
                {
                    Index = startIndex;
                    return null;
                }

                if (delimiters.IndexOf(Current) != -1)
                    return GetString(startIndex, Index, true);

                // Delimiter is not new line and we got one.
                if (isDelimitersNewLine && Current == '\r' || Current == '\n')
                    throw new InvalidOperationException("Unexpected new line: " + GetString(startIndex, Index) +
                                                        "[CRLF].");

                ++Index;
            }
        }

        /// <summary>
        /// Read until a horizontal white space occurs.
        /// </summary>
        /// <returns>A string if a white space was found; otherwise <c>null</c>.</returns>
        public string ReadWord()
        {
            return ReadUntil(" \t");
        }

        /// <summary>
        /// Assign a new buffer
        /// </summary>
        /// <param name="buffer">Buffer to process.</param>
        /// <param name="offset">Where to start process buffer</param>
        /// <param name="count">Buffer length</param>
        /// <exception cref="ArgumentException">Buffer needs to be a byte array</exception>
        public void Assign(byte[] buffer, int offset, int count)
        {
            _slice = new BufferSlice(buffer, offset, count, count);
        }

        public void Assign(BufferSlice buffer)
        {
            _slice = buffer;
        }

        private string GetString(int startIndex, int endIndex)
        {
            return _encoding.GetString(_slice.Buffer, startIndex, endIndex - startIndex);
        }

        private string GetString(int startIndex, int endIndex, bool trimEnd)
        {
            if (trimEnd)
            {
                --endIndex; // need to move one back to be able to trim.
                while (endIndex > 0 && _slice.Buffer[endIndex] == ' ' || _slice.Buffer[endIndex] == '\t')
                    --endIndex;
                ++endIndex;
            }
            return _encoding.GetString(_slice.Buffer, startIndex, endIndex - startIndex);
        }
    }
}