using System;
using System.Data;
using Griffin.Networking.Buffers;
using Griffin.Networking.Messaging;

namespace Griffin.Networking.Servers
{
    /// <summary>
    /// Configures the server
    /// </summary>
    public class ServerConfiguration
    {
        private int _bufferSize;
        private int _maximumNumberOfClients;
        private BufferSliceStack _bufferSliceStack;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerConfiguration" /> class.
        /// </summary>
        public ServerConfiguration()
        {
            MaximumNumberOfClients = 100;
            BufferSize = 65535;
        }

        /// <summary>
        /// Gets or sets the maximum number of clients that can be connected simultaneously
        /// </summary>
        /// <value>Default = 100</value>
        public int MaximumNumberOfClients
        {
            get { return _maximumNumberOfClients; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("value", value, "You should at least allow one connection.");

                _maximumNumberOfClients = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of the buffers should when sending/receiving data.
        /// </summary>
        /// <remarks>Too small buffers can make the selected <see cref="IMessageFormatterFactory"/> fail when serializing/deserializing messages.</remarks>
        /// <value>Default = 65535</value>
        public int BufferSize

        {
            get { return _bufferSize; }
            set
            {
                if (value < 1024)
                    throw new ArgumentException(
                        "Seriously, any buffer size under 1024 seems like a waste. Have you understood what the buffer is used for?");

                _bufferSize = value;
            }
        }

        /// <summary>
        /// Gets or sets stack used 
        /// </summary>
        public BufferSliceStack BufferSliceStack
        {
            get
            {
                return _bufferSliceStack ??
                       (_bufferSliceStack = new BufferSliceStack(MaximumNumberOfClients, BufferSize));
            }
            set { _bufferSliceStack = value; }
        }

        /// <summary>
        /// Validate that the configuration is correct and that it contains all required information
        /// </summary>
        public virtual void Validate()
        {
        }
    }
}