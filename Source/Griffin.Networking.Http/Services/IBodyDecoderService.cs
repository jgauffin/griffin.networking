using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Griffin.Networking.Http.Protocol;
using Griffin.Networking.Http.Services.BodyDecoders;

namespace Griffin.Networking.Http.Services
{
    /// <summary>
    /// Used to decode the HTTP body
    /// </summary>
    public interface IBodyDecoderService
    {
        void Parse(IRequest message);
    }

    public class BodyDecoderService : IBodyDecoderService
    {
        private Dictionary<string, IBodyDecoder> _decoders = new Dictionary<string, IBodyDecoder>();

        public BodyDecoderService()
        {
            _decoders.Add("application/x-www-form-urlencoded", new UrlFormattedDecoder());
        }

        public void Add(string mimeType, IBodyDecoder decoder)
        {
            _decoders[mimeType] = decoder;
        }

        public void Parse(IRequest message)
        {
            IBodyDecoder decoder;
            if (!_decoders.TryGetValue(message.ContentType, out decoder))
                throw new HttpException(HttpStatusCode.UnsupportedMediaType, "Unrecognized mime type: " + message.ContentType);

            decoder.Decode(message);
        }
    }
}
