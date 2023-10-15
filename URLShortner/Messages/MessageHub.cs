using Microsoft.AspNetCore.SignalR;
using System.ComponentModel;
using URLShortener.Common.Model;
using URLShortener.Model;

namespace URLShortener.Messages
{
    /// <summary>
    /// Message hub to send message to all subscriber.
    /// </summary>
    public class MessageHub : Hub
    {
        private static readonly Lazy<MessageHub> _Instance = new(() => new MessageHub());

        /// <summary>
        /// Gets a singleton instance for this class.
        /// </summary>
        public static MessageHub Instance => _Instance.Value;

        /// <summary>
        /// Sends the specified <paramref name="messageId"/> to all connected clients.
        /// </summary>
        /// <param name="messageId">The message to be sent.</param>
        /// <param name="value1">An optional DTO for the message.</param>
        /// <param name="value2">An optional DTO for the message.</param>
        /// <returns>A <see cref="Task"/> responsible for sending all the messages.</returns>
        public Task Send(string messageId, ShortUrlDTO? value1 = null, ShortUrlDTO? value2 = null)
        {
            if (Clients == null)
                return Task.CompletedTask;

            if (value1 == null) return Clients.All.SendAsync(messageId);
            if (value2 == null) return Clients.All.SendAsync(messageId, value1);
            return Clients.All.SendAsync(messageId, value1, value2);
        }


    }
}
