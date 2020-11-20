using System;

namespace Expertec.Sigeco.AuthServer.API.Infrastructure.Exceptions
{
    public class AuthDomainException : Exception
    {
        private int errorId;

        public int ErrorId => errorId;

        public AuthDomainException()
        { }

        public AuthDomainException(string message)
            : base(message)
        { }

        public AuthDomainException(string message, Exception innerException)
            : base(message, innerException)
        { }

        public AuthDomainException(string message, int errorId)
            : base(message)
        { 
            this.errorId = errorId;
        }
    }
}