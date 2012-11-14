using System;
using Griffin.Networking.Messaging;

namespace BasicDemo
{
    public class MyService : MessagingService
    {
        /// <summary>
        /// A new message have been received from the remote end.
        /// </summary>
        /// <param name="message"></param>
        /// <remarks>We'll deserialize messages for you. What you receive here depends on the used <see cref="IMessageFormatterFactory"/>.</remarks>
        public override void HandleReceive(object message)
        {
            // We can only receive this kind of command
            var msg = (OpenDoor)message;

            Console.WriteLine("Should open door: {0}.", msg.Id);

            // Send a reply
            Context.Write(new DoorOpened(msg.Id));
        }
    }
}