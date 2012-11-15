using System;

namespace Griffin.Networking.Buffers
{
    public interface IStringBufferReader
    {
        /// <summary>
        /// Gets current character
        /// </summary>
        /// <value><see cref="char.MinValue"/> if end of buffer.</value>
        char Current { get; }

        /// <summary>
        /// Gets if more bytes can be processed.
        /// </summary>
        /// <value></value>
        bool HasMore { get; }

        /// <summary>
        /// Gets or sets current position in buffer.
        /// </summary>
        /// <remarks>
        /// THINK before you manually change the position since it can blow up
        /// the whole parsing in your face.
        /// </remarks>
        int Index { get; set; }

        /// <summary>
        /// Gets total length of buffer.
        /// </summary>
        /// <value></value>
        int Length { get; }

        /// <summary>
        /// Gets next character
        /// </summary>
        /// <value><see cref="char.MinValue"/> if end of buffer.</value>
        char Peek { get; }

        /// <summary>
        /// Gets number of bytes left.
        /// </summary>
        int RemainingLength { get; }

        /// <summary>
        /// Consume current character.
        /// </summary>
        void Consume();

        /// <summary>
        /// Consume specified characters
        /// </summary>
        /// <param name="chars">One or more characters.</param>
        void Consume(params char[] chars);

        /// <summary>
        /// Consume all characters until the specified one have been found.
        /// </summary>
        /// <param name="delimiter">Stop when the current character is this one</param>
        /// <returns>New offset.</returns>
        int ConsumeUntil(char delimiter);

        /// <summary>
        /// Consumes horizontal white spaces (space and tab).
        /// </summary>
        void ConsumeWhiteSpaces();

        /// <summary>
        /// Consume horizontal white spaces and the specified character.
        /// </summary>
        /// <param name="extraCharacter">Extra character to consume</param>
        void ConsumeWhiteSpaces(char extraCharacter);

        /// <summary>
        /// Checks if one of the remaining bytes are a specified character.
        /// </summary>
        /// <param name="ch">Character to find.</param>
        /// <returns>
        /// 	<c>true</c> if found; otherwise <c>false</c>.
        /// </returns>
        bool Contains(char ch);

        /// <summary>
        /// Read a character.
        /// </summary>
        /// <returns>
        /// Character if not EOF; otherwise <c>null</c>.
        /// </returns>
        char Read();

        /// <summary>
        /// Get a text line. 
        /// </summary>
        /// <returns></returns>
        /// <remarks>Will merge multi line headers and rewind of end of line was not found.</remarks> 
        string ReadLine();

        /// <summary>
        /// Read quoted string
        /// </summary>
        /// <returns>string if current character (in buffer) is a quote; otherwise <c>null</c>.</returns>
        string ReadQuotedString();

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
        string ReadToEnd(string delimiters);

        /// <summary>
        /// Read until end of string, or to one of the delimiters are found.
        /// </summary>
        /// <returns>A string (can be <see cref="string.Empty"/>).</returns>
        /// <remarks>
        /// Will not consume the delimiter.
        /// </remarks>
        string ReadToEnd();

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
        string ReadToEnd(char delimiter);

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
        string ReadUntil(char delimiter);

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
        string ReadUntil(string delimiters);

        /// <summary>
        /// Read until a horizontal white space occurs.
        /// </summary>
        /// <returns>A string if a white space was found; otherwise <c>null</c>.</returns>
        string ReadWord();

        /// <summary>
        /// Assigns the slice to read from
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="count"> </param>
        void Assign(IBufferSlice buffer, int count);
    }
}