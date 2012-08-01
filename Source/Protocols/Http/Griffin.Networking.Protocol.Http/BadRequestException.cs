using System;
using System.Net;

namespace Griffin.Networking.Http
{
    /// <summary>
    /// Request is malformed.
    /// </summary>
    public class BadRequestException : HttpException
    {
        private readonly Exception _inner;

        /// <summary>
        /// Initializes a new instance of the <see cref="BadRequestException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public BadRequestException(string message)
            : base(HttpStatusCode.BadRequest, message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BadRequestException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public BadRequestException(string message, Exception inner)
            : base(HttpStatusCode.BadRequest, message)
        {
            _inner = inner;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" PathDiscovery="*AllFiles*"/>
        ///   </PermissionSet>
        public override string ToString()
        {
            return base.ToString() + "\r\nInner exception: " + _inner;
        }
    }
}