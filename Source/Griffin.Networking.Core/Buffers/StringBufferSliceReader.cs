using System;
using System.Linq;
using System.Text;

namespace Griffin.Networking.Buffers
{
    /// <summary>
    /// Read text from a buffer slice.
    /// </summary>
    public class StringBufferSliceReader : IStringBufferReader
    {
        private readonly Encoding _encoding;
        private readonly int _length;
        private int _position;
        private IBufferSlice _reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringBufferSliceReader"/> class.
        /// </summary>
        /// <remarks>You must use <see cref="Assign(IBufferSlice,int)"/> if you use this constructor<para>Initialied using ASCII as encoding.</para></remarks>
        public StringBufferSliceReader()
        {
            _encoding = Encoding.ASCII;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringBufferSliceReader"/> class.
        /// </summary>
        /// <param name="encoding">Encoding to use when converting byte array to strings.</param>
        /// <remarks>You must use <see cref="Assign(IBufferSlice, int)"/> if you use this constructor</remarks>
        public StringBufferSliceReader(Encoding encoding)
        {
            if (encoding == null) throw new ArgumentNullException("encoding");
            _encoding = encoding;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringBufferSliceReader"/> class.
        /// </summary>
        public StringBufferSliceReader(IBufferSlice slice, int count)
        {
            if (slice == null) throw new ArgumentNullException("slice");
            _reader = slice;
            _length = count;
            _encoding = Encoding.ASCII;
        }

        /// <summary>
        /// Gets buffer that we are reading from.
        /// </summary>
        public byte[] Buffer
        {
            get { return _reader.Buffer; }
        }

        /// <summary>
        /// Gets if end of buffer have been reached
        /// </summary>
        /// <value></value>
        public bool EndOfFile
        {
            get { return _position == _length; }
        }

        #region IStringBufferReader Members

        /// <summary>
        /// Gets current character
        /// </summary>
        /// <value><see cref="char.MinValue"/> if end of buffer.</value>
        public char Current
        {
            get { return HasMore ? (char) _reader.Buffer[_position] : char.MinValue; }
        }

        /// <summary>
        /// Gets if more bytes can be processed.
        /// </summary>
        /// <value></value>
        public bool HasMore
        {
            get { return _position < Length; }
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
            get { return _position; }
            set { _position = value; }
        }

        /// <summary>
        /// Gets total length of buffer.
        /// </summary>
        /// <value></value>
        public int Length
        {
            get { return _length; }
        }

        /// <summary>
        /// Gets next character
        /// </summary>
        /// <value><see cref="char.MinValue"/> if end of buffer.</value>
        public char Peek
        {
            get { return HasMore ? (char) _reader.Buffer[Index + 1] : char.MinValue; }
        }

        /// <summary>
        /// Gets number of bytes left.
        /// </summary>
        public int RemainingLength
        {
            get { return _length - _position; }
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
            return (char) _reader.Buffer[Index++];
        }

        /// <summary>
        /// Get a text line. 
        /// </summary>
        /// <returns></returns>
        /// <remarks>Will merge multi line headers and rewind of end of line was not found.</remarks> 
        public string ReadLine()
        {
            var startIndex = Index;
            while (HasMore && Current != '\n' && Current != '\r')
                Consume();


            // EOF? Then we havent enough bytes.
            if (EndOfFile)
            {
                Index = startIndex;
                return null;
            }

            var thisLine = _encoding.GetString(_reader.Buffer, startIndex, Index - startIndex);

            // \r\n
            if (Current == '\r')
                Consume();
            if (Current == '\n')
                Consume();

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
                        var value = _encoding.GetString(_reader.Buffer, startPos, Index - startPos);
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
            var str = _encoding.GetString(_reader.Buffer, Index, RemainingLength);
            Index = _position;
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
        /// Assigns the slice to read from
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="count"> </param>
        public void Assign(IBufferSlice buffer, int count)
        {
            _reader = buffer;
        }

        #endregion

        /// <summary>
        /// Assign a new buffer
        /// </summary>
        /// <param name="buffer">Buffer to process.</param>
        /// <param name="offset">Where to start process the buffer</param>
        /// <param name="count">Number if bytes to read</param>
        /// <exception cref="ArgumentException">Buffer needs to be a byte array</exception>
        public void Assign(byte[] buffer, int offset, int count)
        {
            _reader = new BufferSlice(buffer, offset, count);
        }

        private string GetString(int startIndex, int endIndex)
        {
            return _encoding.GetString(_reader.Buffer, startIndex, endIndex - startIndex);
        }

        private string GetString(int startIndex, int endIndex, bool trimEnd)
        {
            if (trimEnd)
            {
                --endIndex; // need to move one back to be able to trim.
                while (endIndex > 0 && _reader.Buffer[endIndex] == ' ' || _reader.Buffer[endIndex] == '\t')
                    --endIndex;
                ++endIndex;
            }
            return _encoding.GetString(_reader.Buffer, startIndex, endIndex - startIndex);
        }
    }
}